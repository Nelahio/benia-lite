using BeniaLite.Api.Auth;
using BeniaLite.Api.Data;
using BeniaLite.Api.Entities;
using BeniaLite.Api.Symptoms.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeniaLite.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class SymptomsController : ControllerBase
{
    private readonly BeniaDbContext _db;

    public SymptomsController(BeniaDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<SymptomLogListItemDto>>> List([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var userId = CurrentUser.GetUserId(User);

        var query = _db.SymptomLogs.AsQueryable()
            .Where(x => x.UserId == userId);

        if (from is not null) query = query.Where(x => x.LoggedAtUtc >= from.Value);
        if (to is not null) query = query.Where(x => x.LoggedAtUtc <= to.Value);

        var data = await query
            .OrderByDescending(x => x.LoggedAtUtc)
            .Select(x => new SymptomLogListItemDto(x.Id, x.Category, x.Severity0to10, x.LoggedAtUtc))
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SymptomLogDetailDto>> Get(Guid id)
    {
        var userId = CurrentUser.GetUserId(User);

        var log = await _db.SymptomLogs
            .Include(x => x.Triggers).ThenInclude(t => t.TriggerTag)
            .Include(x => x.Photos)
            .SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (log is null) return NotFound();

        var dto = new SymptomLogDetailDto(
            log.Id,
            log.Category,
            log.Severity0to10,
            log.Notes,
            log.LoggedAtUtc,
            log.Triggers
                .Select(t => new TriggerTagDto(t.TriggerTagId, t.TriggerTag!.Name))
                .OrderBy(t => t.Name)
                .ToList(),
            log.Photos
                .OrderByDescending(p => p.CreatedAtUtc)
                .Select(p => new PhotoDto(p.Id, p.Url, p.CreatedAtUtc))
                .ToList()
        );

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<SymptomLogDetailDto>> Create(CreateSymptomLogRequest req)
    {
        var userId = CurrentUser.GetUserId(User);

        if (req.Severity0to10 < 0 || req.Severity0to10 > 10)
            return BadRequest("Severity must be between 0 and 10.");

        var log = new SymptomLog
        {
            UserId = userId,
            Category = req.Category.Trim(),
            Severity0to10 = req.Severity0to10,
            Notes = req.Notes?.Trim(),
            LoggedAtUtc = req.LoggedAtUtc ?? DateTime.UtcNow
        };

        // Trigger tags (création si inexistants)
        var names = (req.TriggerNames ?? new List<string>())
            .Select(n => n.Trim().ToLowerInvariant())
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct()
            .Take(10)
            .ToList();

        if (names.Count > 0)
        {
            var existing = await _db.TriggerTags
                .Where(t => t.UserId == userId && names.Contains(t.Name))
                .ToListAsync();

            foreach (var name in names)
            {
                var tag = existing.SingleOrDefault(x => x.Name == name);
                if (tag is null)
                {
                    tag = new TriggerTag { UserId = userId, Name = name };
                    _db.TriggerTags.Add(tag);
                    existing.Add(tag);
                }

                log.Triggers.Add(new SymptomLogTrigger { TriggerTag = tag });
            }
        }

        _db.SymptomLogs.Add(log);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = log.Id }, await BuildDetailDto(log.Id, userId));
    }

    private async Task<SymptomLogDetailDto> BuildDetailDto(Guid id, Guid userId)
    {
        var log = await _db.SymptomLogs
            .Include(x => x.Triggers).ThenInclude(t => t.TriggerTag)
            .Include(x => x.Photos)
            .SingleAsync(x => x.Id == id && x.UserId == userId);

        return new SymptomLogDetailDto(
            log.Id, log.Category, log.Severity0to10, log.Notes, log.LoggedAtUtc,
            log.Triggers.Select(t => new TriggerTagDto(t.TriggerTagId, t.TriggerTag!.Name)).OrderBy(t => t.Name).ToList(),
            log.Photos.OrderByDescending(p => p.CreatedAtUtc).Select(p => new PhotoDto(p.Id, p.Url, p.CreatedAtUtc)).ToList()
        );
    }

    [HttpPost("{id:guid}/photos")]
    public async Task<ActionResult<List<PhotoDto>>> UploadPhoto(Guid id, IFormFile file)
    {
        var userId = CurrentUser.GetUserId(User);

        if (file is null || file.Length == 0)
            return BadRequest("File is required.");

        var log = await _db.SymptomLogs.SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (log is null) return NotFound();

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (!allowed.Contains(ext))
            return BadRequest("Only jpg, jpeg, png, webp allowed.");

        var fileName = $"{Guid.NewGuid()}{ext}";
        var relPath = $"/uploads/{fileName}";
        var absPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

        await using (var stream = System.IO.File.Create(absPath))
        {
            await file.CopyToAsync(stream);
        }

        _db.Photos.Add(new Photo
        {
            UserId = userId,
            SymptomLogId = id,
            Url = relPath
        });

        await _db.SaveChangesAsync();

        var photos = await _db.Photos
            .Where(p => p.UserId == userId && p.SymptomLogId == id)
            .OrderByDescending(p => p.CreatedAtUtc)
            .Select(p => new PhotoDto(p.Id, p.Url, p.CreatedAtUtc))
            .ToListAsync();

        return Ok(photos);
    }

    [HttpDelete("{id:guid}/photos/{photoId:guid}")]
    public async Task<IActionResult> DeletePhoto(Guid id, Guid photoId)
    {
        var userId = CurrentUser.GetUserId(User);

        var photo = await _db.Photos.SingleOrDefaultAsync(p =>
            p.Id == photoId &&
            p.SymptomLogId == id &&
            p.UserId == userId);

        if (photo is null) return NotFound();

        // supprimer le fichier physique si possible
        var absPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photo.Url.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

        if (System.IO.File.Exists(absPath))
            System.IO.File.Delete(absPath);

        _db.Photos.Remove(photo);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("stats")]
    public async Task<ActionResult<SymptomStatsDto>> Stats([FromQuery] int days = 7, [FromQuery] string? category = null)
    {
        var userId = CurrentUser.GetUserId(User);

        if (days < 1 || days > 365) return BadRequest("days must be between 1 and 365.");

        var start = DateTime.UtcNow.Date.AddDays(-(days - 1));
        var end = DateTime.UtcNow.Date.AddDays(1);

        var query = _db.SymptomLogs
            .Where(x => x.UserId == userId && x.LoggedAtUtc >= start && x.LoggedAtUtc < end);

        if (!string.IsNullOrWhiteSpace(category))
        {
            var cat = category.Trim();
            query = query.Where(x => x.Category == cat);
        }

        var logs = await query.ToListAsync();

        if (logs.Count == 0)
        {
            return Ok(new SymptomStatsDto(
                days,
                category,
                0,
                0,
                0,
                0,
                new List<SymptomDailyPointDto>()
            ));
        }

        var avg = logs.Average(x => x.Severity0to10);
        var min = logs.Min(x => x.Severity0to10);
        var max = logs.Max(x => x.Severity0to10);

        // série journalière : moyenne par jour
        var series = logs
            .GroupBy(x => x.LoggedAtUtc.Date)
            .OrderBy(g => g.Key)
            .Select(g => new SymptomDailyPointDto(
                g.Key,
                Math.Round(g.Average(x => x.Severity0to10), 2),
                g.Count()
            ))
            .ToList();

        var seriesFilled = new List<SymptomDailyPointDto>();
        for (var d = start; d < end; d = d.AddDays(1))
        {
            var point = series.SingleOrDefault(p => p.DayUtc == d);
            seriesFilled.Add(point ?? new SymptomDailyPointDto(d, 0, 0));
        }

        return Ok(new SymptomStatsDto(
            days,
            category,
            logs.Count,
            Math.Round(avg, 2),
            min,
            max,
            seriesFilled
        ));
    }
}
