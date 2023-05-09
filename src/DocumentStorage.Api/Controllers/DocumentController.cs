using DocumentStorage.Api.Model;
using DocumentStorage.Authentication;
using DocumentStorage.Core;
using DocumentStorage.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocumentStorage.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class DocumentController : BaseController
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService, 
            IAuthenticationService authenticationService) : base(authenticationService)
        {
            _documentService = documentService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request)
        {
            try
            {
                if (!Authorized(new Role[] { Role.Admin, Role.Manager }))
                {
                    return Unauthorized();
                }

                var userId = GetUserIdFromToken();

                if (request.File == null || request.File.Length == 0)
                {
                    return BadRequest("File is required");
                }

                var document = new DocumentMetadata {
                    Name = request.File.FileName ?? string.Empty,
                    Description = request.Description ?? string.Empty,
                    Category = request.Category,
                    CreatedByUser = userId,
                    PostedDate = DateTime.UtcNow
                };

                using (var memoryStream = new MemoryStream())
                {
                    await request.File.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();

                    await _documentService.UploadDocument(document, content);
                }

                return Ok(new {name = document.Name });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("metadata/{fileId}")]
        public async Task<IActionResult> Get(int fileId)
        {
            try
            {
                var userId = GetUserIdFromToken();

                var metadata = await _documentService.GetDocumentMetadata(fileId, userId);

                return Ok(metadata);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> GetFile(int fileId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                
                var (fileName, content) = await _documentService.DownloadDocument(fileId, userId);

                var stream = new MemoryStream(content);

                return new FileStreamResult(stream, "application/octet-stream")
                {
                    FileDownloadName = fileName
                };
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetFiles()
        {
            try
            {
                var userId = GetUserIdFromToken();
                
                var metadataList = await _documentService.GetDocumentsByUserId(userId);

                return Ok(metadataList.Select(p => new { id = p.Id, name = p.Name }));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private int GetUserIdFromToken()
        {
            var claims = GetClaims();

            return claims.id;
        }
    }
}
