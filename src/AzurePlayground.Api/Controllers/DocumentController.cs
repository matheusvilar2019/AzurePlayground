using AzurePlayground.Application.DTOs;
using AzurePlayground.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AzurePlayground.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentDTO>>> GetDocuments()
        {
            var documents = await _documentService.GetDocuments();

            return Ok(documents);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DocumentDTO>> GetById(int id)
        {
            var document = await _documentService.GetById(id);

            if (document == null)
                return NotFound();

            return Ok(document);
        }

        [HttpPost]
        public async Task<ActionResult<DocumentDTO>> Add(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

            await using var stream = file.OpenReadStream();

            var documentUploadDTO = new DocumentUploadDTO
            {
                OriginalFileName = file.FileName,
                ContentType = file.ContentType,
                Content = stream
            };

            var document = await _documentService.Add(documentUploadDTO);

            return Ok(document);
        }

        [HttpGet("download/{id:int}")]
        public async Task<ActionResult> Download(int id)
        {
            var documentDownloadDTO = await _documentService.Download(id);

            if (documentDownloadDTO == null)
                return NotFound();

            return File(
                documentDownloadDTO.Content,
                documentDownloadDTO.ContentType,
                documentDownloadDTO.FileName
                );
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] DocumentDTO documentDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _documentService.Update(documentDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Remove(int id)
        {
            var removed = await _documentService.Remove(id);

            if (removed == false) return NotFound();

            return NoContent();
        }
    }
}
