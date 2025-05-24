namespace DrawPT.Data.Models
{
    public class OpenAIImageResponse
    {
        public long Created { get; set; }
        public List<ImageData> Data { get; set; }
    }

    public class ImageData
    {
        public string Url { get; set; }
    }
}
