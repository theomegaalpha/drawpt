using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameSession
{
    Task PlayGameAsync(string roomCode);
} 