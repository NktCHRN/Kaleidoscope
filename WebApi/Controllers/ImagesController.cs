using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Responses.Common;
using WebApi.Models.Responses.File;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImagesController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpGet("{fileName}")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> DownloadImage(string fileName)
    {
        var file = await _imageService.DownloadPhotoAsync(fileName);
        return File(file.Data.ToArray(), file.ContentType, file.Name);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(FileUploadedResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var fileDto = new MediaFileDto
        {
            ContentType = file.ContentType,
            Name = file.Name,
            Data = BinaryData.FromStream(file.OpenReadStream())
        };

        var resultName = await _imageService.UploadPhotoAsync(fileDto);

        return Ok(new FileUploadedResponse
        {
            FileName = resultName,
        });
    }
}
