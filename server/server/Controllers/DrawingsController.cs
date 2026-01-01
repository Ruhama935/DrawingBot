using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Services;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrawingsController : ControllerBase
    {
        private readonly IDrawingService _drawingService;

        public DrawingsController(IDrawingService drawingService)
        {
            _drawingService = drawingService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateDrawing([FromBody] GenerateDrawingRequest request)
        {
            var result = await _drawingService.GenerateDrawingAsync(request);
            return Ok(result);
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveDrawing([FromBody] SaveDrawingRequest request)
        {
            var result = await _drawingService.SaveDrawingAsync(request);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.DrawingId },
                result
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var drawing = await _drawingService.GetDrawingByIdAsync(id);

            if (drawing == null)
                return NotFound(new { error = "Drawing not found" });

            return Ok(drawing);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var drawings = await _drawingService.GetDrawingsByUserIdAsync(userId);
            return Ok(drawings);
        }
    }
}