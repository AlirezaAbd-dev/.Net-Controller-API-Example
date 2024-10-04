using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Cityinfo.API.Controllers {
    [Route("api/v{version:apiVersion}/files")]
    [ApiController]
    public class FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider) : ControllerBase {

        [HttpGet("{fileId}")]
        [ApiVersion(1)]
        public ActionResult GetFile(string fileId) {
            var pathToFile = "banner.rar";

            if( !System.IO.File.Exists(pathToFile) ) {
                return NotFound();
            }

            var bytes = System.IO.File.ReadAllBytes(pathToFile);

            if(!fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType)){
                contentType = "application/octet-stream";
            }

            return File(bytes, contentType, Path.GetFileName(pathToFile));
        }
    }
}
