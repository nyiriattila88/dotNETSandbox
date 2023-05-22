using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers;

[Route("api/files")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

    public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
    {
        _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ?? throw new ArgumentNullException(nameof(fileExtensionContentTypeProvider));
    }

    [HttpGet]
    public ActionResult GetFileNames()
    {
        string directoryPath = "Resources";

        IEnumerable<string> fileNames = Directory.Exists(directoryPath)
            ? Directory.GetFiles(directoryPath).Select(x => Path.GetFileName(x))
            : Enumerable.Empty<string>();

        return Ok(fileNames);
    }

    [HttpGet("{fileName}")]
    public ActionResult GetFile(string fileName)
    {
        string filePath = Path.GetFullPath($"Resources/{fileName}");

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        byte[] bytes = System.IO.File.ReadAllBytes(filePath);

        if (!_fileExtensionContentTypeProvider.TryGetContentType(filePath, out string? contentType))
        {
            contentType = MediaTypeNames.Application.Octet;
        }

        return File(bytes, contentType, Path.GetFileName(filePath));
    }
}
