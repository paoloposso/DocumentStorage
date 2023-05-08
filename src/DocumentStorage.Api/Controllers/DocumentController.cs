using DocumentStorage.Api.Model;
using DocumentStorage.Authentication;
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
        private readonly DocumentService _documentService;

        public DocumentController(DocumentService documentService, 
            IAuthenticationService authenticationService) : base(authenticationService)
        {
            _documentService = documentService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request)
        {
            try
            {
                var userId = GetUserIdFromToken();

                if (request.File == null || request.File.Length == 0)
                {
                    return BadRequest("File is required");
                }

                var document = new DocumentMetadata {
                    Name = request.Name ?? string.Empty,
                    Description = request.Description ?? string.Empty,
                    Category = request.Category ?? string.Empty,
                    CreatedByUser = userId
                };

                using (var memoryStream = new MemoryStream())
                {
                    await request.File.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();

                    await _documentService.UploadDocument(document, content);
                }

                return Ok(document);
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

        [HttpGet("metadata/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var userId = GetUserIdFromToken();
                
                byte[] content = await _documentService.DownloadDocument(id, userId);

                var stream = new MemoryStream(content);

                return new FileStreamResult(stream, "application/octet-stream")
                {
                    FileDownloadName = $"document_{id}.pdf"
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

        [HttpGet("doenload/{id}")]
        public async Task<IActionResult> GetFile(int id)
        {
            try
            {
                var userId = GetUserIdFromToken();
                
                byte[] content = await _documentService.DownloadDocument(id, userId);

                var stream = new MemoryStream(content);

                return new FileStreamResult(stream, "application/octet-stream")
                {
                    FileDownloadName = $"document_{id}.pdf"
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
    }
}
