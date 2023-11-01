using API.Services.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("/v1/itsds/storage")]
public class StorageController : BaseController
{
    private readonly IFirebaseService _firebaseService;

    public StorageController(IFirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    [Authorize]
    [HttpPost("Upload")]
    public async Task<IActionResult> UploadFirebase(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new BadRequestException("No file uploaded.");
        }

        var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        var linkImage = await _firebaseService.UploadFirebaseAsync(stream, file.FileName);
        return Ok(linkImage);
    }
}
