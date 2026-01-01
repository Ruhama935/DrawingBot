namespace server.Models
{
    public class Drawing
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string PromptText { get; set; } = string.Empty;

        // נשמור את פקודות הציור כ־JSON
        public string CommandsJson { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
