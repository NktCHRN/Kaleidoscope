using DataAccess.Abstractions;
using DataAccess.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    private readonly IBlobRepository _blobRepository;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ApplicationDbContext context, IBlobRepository blobRepository)
    {
        _logger = logger;
        _context = context;
        _blobRepository = blobRepository;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        //var bitmap = new System.Drawing.Bitmap(640, 480);

        //for (var x = 0; x < bitmap.Width; x++)
        //{
        //    for (var y = 0; y < bitmap.Height; y++)
        //    {
        //        bitmap.SetPixel(x, y, Color.BlueViolet);
        //    }
        //}

        //using var memoryStream = new MemoryStream();
        //bitmap.Save(memoryStream, ImageFormat.Png);
        //var photo = new BinaryData(memoryStream.ToArray());

        //await _blobRepository.UploadPhotoAsync(new DataAccess.Common.MediaFile()
        //{
        //    Data = new BinaryData(photo.ToArray()),
        //    ContentType = "image/png",
        //    Name = "hello.png"
        //});

        var temp = await _context.Posts.Where(p => p.IsModidied).ToListAsync();

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
