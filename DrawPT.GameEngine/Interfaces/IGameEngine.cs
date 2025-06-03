using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameEngine
{
    Task PlayGameAsync(string roomCode);
} 