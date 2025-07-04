using OpenAI;
using OpenAI.Audio;
using DrawPT.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DrawPT.Api.Services
{
    public class TtsService
    {
        private readonly AudioClient _audioClient;
        private readonly IHubContext<GameHub, IGameClient> _hubContext;

        public TtsService(OpenAIClient aiClient, IHubContext<GameHub, IGameClient> hubContext)
        {
            _audioClient = aiClient.GetAudioClient("gpt-4o-mini-tts");
            _hubContext = hubContext;
        }

        /// <summary>
        /// Generates speech audio, chunks it, and streams to clients in the specified room.
        /// </summary>
        public async Task GenerateAudio(string text, IGameClient client)
        {
            var options = new SpeechGenerationOptions
            {
                ResponseFormat = "opus"
            };


            // 1) synthesize full PCM buffer
            BinaryData speech = await _audioClient.GenerateSpeechAsync(text, GeneratedSpeechVoice.Alloy, options);
            byte[] pcm = speech.ToArray();

            // 2) break into chunks
            const int chunkSize = 8192;
            for (int offset = 0; offset < pcm.Length; offset += chunkSize)
            {
                int count = Math.Min(chunkSize, pcm.Length - offset);
                byte[] slice = new byte[count];
                Array.Copy(pcm, offset, slice, 0, count);

                // 3) Base64‐encode that slice
                string base64 = Convert.ToBase64String(slice);

                // 4) push to the caller
                await client.ReceiveAudio(base64);

                // optional pacing so the stream doesn’t overwhelm the client
                await Task.Delay(20);
            }

            // 5) tell the client we’re done
            await client.AudioStreamCompleted();
        }

        public void Stop()
        {
            Console.WriteLine("Speech stopped.");
        }
    }
}
