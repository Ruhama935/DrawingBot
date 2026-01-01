using System.ComponentModel.DataAnnotations;

namespace server.DTOs
{
    public class GenerateDrawingRequest
    {
        [Required(ErrorMessage = "Prompt is required")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Prompt must be between 1 and 1000 characters")]
        public string Prompt { get; set; } = string.Empty;
    }
}
