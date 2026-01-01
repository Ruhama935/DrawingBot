namespace server.DTOs
{
    public class GenerateDrawingResponse
    {
        public object Commands { get; set; } = new();
        public string Prompt { get; set; } = string.Empty;
    }
}
