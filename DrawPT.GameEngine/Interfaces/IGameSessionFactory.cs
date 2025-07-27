namespace DrawPT.GameEngine.Interfaces;

public interface IGameSessionFactory
{
    IGameSession Create(bool playerPromptMode);
}
