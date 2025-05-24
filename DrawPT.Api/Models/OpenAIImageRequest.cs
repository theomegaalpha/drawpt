namespace DrawPT.Api.Models
{
    public class OpenAIImageRequest
    {
        public string Model { get; set; }
        public string Prompt { get; set; }
        public int N { get; set; }
        public string Size { get; set; }
    }
}

