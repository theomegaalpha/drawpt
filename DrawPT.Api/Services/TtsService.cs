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
        public async Task GenerateAudio(string text, string roomCode)
        {
            BinaryData speech = await _audioClient.GenerateSpeechAsync(text, GeneratedSpeechVoice.Alloy);
            byte[] data = speech.ToArray();
            const int chunkSize = 8192;
            for (int offset = 0; offset < data.Length; offset += chunkSize)
            {
                int count = Math.Min(chunkSize, data.Length - offset);
                byte[] chunk = new byte[count];
                Array.Copy(data, offset, chunk, 0, count);
                await _hubContext.Clients.Group(roomCode).ReceiveAudio(chunk);
            }
            await _hubContext.Clients.Group(roomCode).AudioStreamCompleted();
        }

        public async Task GenerateAudioToPlayer(string text, string connectionId)
        {
            BinaryData speech = await _audioClient.GenerateSpeechAsync(text, GeneratedSpeechVoice.Alloy);
            byte[] data = speech.ToArray();
            const int chunkSize = 8192;
            for (int offset = 0; offset < data.Length; offset += chunkSize)
            {
                int count = Math.Min(chunkSize, data.Length - offset);
                byte[] chunk = new byte[count];
                Array.Copy(data, offset, chunk, 0, count);
                await _hubContext.Clients.Client(connectionId).ReceiveAudio(chunk);
            }
            await _hubContext.Clients.Client(connectionId).AudioStreamCompleted();
        }

        public void Stop()
        {
            Console.WriteLine("Speech stopped.");
        }
    }
}
