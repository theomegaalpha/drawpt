using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine.Services;

public class GameSessionFactory : IGameSessionFactory
{
    private readonly IServiceProvider _serviceProvider;

    public GameSessionFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IGameSession Create(bool playerPromptMode)
    {
        return playerPromptMode
            ? _serviceProvider.GetRequiredService<DuelGameSession>()
            : _serviceProvider.GetRequiredService<DefaultGameSession>();
    }
}
