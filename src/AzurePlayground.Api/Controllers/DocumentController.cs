using AzurePlayground.Application.DTOs;
using AzurePlayground.Application.Services;
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
        public async Task<ActionResult> Add([FromBody] DocumentDTO documentDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _documentService.Add(documentDTO);

            return Ok();
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
            await _documentService.Remove(id);

            return NoContent();
        }
    }
}
