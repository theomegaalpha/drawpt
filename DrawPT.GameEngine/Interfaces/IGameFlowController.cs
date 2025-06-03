using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IGameFlowController
{
    Task PlayGameAsync(string roomCode);
} 