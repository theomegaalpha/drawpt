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

            BinaryData speech = await _audioClient.GenerateSpeechAsync(text, "ballad", options);
            byte[] buffer = speech.ToArray();
            const byte O1 = 0x4F, O2 = 0x67, O3 = 0x67, O4 = 0x53;
            const int MaxChunkSize = 64 * 1024; // 64 KB threshold
            var chunkBuffer = new List<byte>();
            for (int pos = 0; pos < buffer.Length;)
            {
                int idx = Array.FindIndex(buffer, pos + 1, b => b == O1);
                int next = (idx > 0 && idx + 3 < buffer.Length
                            && buffer[idx + 1] == O2 && buffer[idx + 2] == O3 && buffer[idx + 3] == O4)
                           ? idx
                           : buffer.Length;
                int len = next - pos;
                var page = new byte[len];
                Array.Copy(buffer, pos, page, 0, len);
                chunkBuffer.AddRange(page);
                if (chunkBuffer.Count >= MaxChunkSize)
                {
                    await client.ReceiveAudio(Convert.ToBase64String(chunkBuffer.ToArray()));
                    chunkBuffer.Clear();
                }
                pos = next;
            }
            if (chunkBuffer.Count > 0)
                await client.ReceiveAudio(Convert.ToBase64String(chunkBuffer.ToArray()));

            await client.AudioStreamCompleted();
        }

        public void Stop()
        {
            Console.WriteLine("Speech stopped.");
        }
    }
}
