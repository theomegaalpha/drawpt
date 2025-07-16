using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrawPT.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BackgroundMusicController : ControllerBase
    {
        private readonly BlobContainerClient _containerClient;

        public BackgroundMusicController(BlobServiceClient blobServiceClient)
        {
            _containerClient = blobServiceClient.GetBlobContainerClient("music");
        }

        // GET to list all background music files
        [Authorize]
        [HttpGet]
        public ActionResult<List<string>> GetBackgroundMusic()
        {
            // List only files in the "game-background" folder
            var blobs = _containerClient.GetBlobs(prefix: "game-background/");
            var musicFiles = new List<string>();

            foreach (var blob in blobs)
            {
                musicFiles.Add(blob.Name);
            }

            return Ok(musicFiles);
        }
    }
}
