//using DrawPT.GameEngine.Interfaces;
//using DrawPT.GameEngine.Models;
//using Microsoft.AspNetCore.SignalR;

//namespace DrawPT.GameEngine.Services;

//public class GameCommunicationService : IGameCommunicationService
//{
//    private readonly IHubContext<GameHub, IGamePlayer> _hubContext;
//    private readonly GameState _gameState;

//    public GameCommunicationService(
//        IHubContext<GameHub, IGamePlayer> hubContext,
//        GameState gameState)
//    {
//        _hubContext = hubContext;
//        _gameState = gameState;
//    }

//    public async Task BroadcastGameEventAsync(GameEvent gameEvent)
//    {
//        switch (gameEvent)
//        {
//            case GameStartedEvent started:
//                await _hubContext.Clients.Group(_gameState.RoomCode).GameStarted(started.Configuration);
//                break;
//            case GameCompletedEvent completed:
//                await _hubContext.Clients.Group(_gameState.RoomCode).GameCompleted(completed.Results);
//                break;
//            case ThemeSelectionStartedEvent themeStarted:
//                await _hubContext.Clients.GroupExcept(_gameState.RoomCode, themeStarted.SelectorId)
//                    .ThemeSelectionStarted();
//                break;
//            case ThemeSelectedEvent themeSelected:
//                await _hubContext.Clients.Group(_gameState.RoomCode).ThemeSelected(themeSelected.ThemeName);
//                break;
//            default:
//                throw new ArgumentException($"Unknown game event type: {gameEvent.GetType().Name}");
//        }
//    }

//    public async Task SendToPlayerAsync(string connectionId, PlayerEvent playerEvent)
//    {
//        switch (playerEvent)
//        {
//            case PlayerJoinedEvent joined:
//                await _hubContext.Clients.Client(connectionId).PlayerJoined(joined.Player);
//                break;
//            case PlayerLeftEvent left:
//                await _hubContext.Clients.Client(connectionId).PlayerLeft(left.Player);
//                break;
//            case ScoreUpdatedEvent scoreUpdated:
//                await _hubContext.Clients.Client(connectionId).ScoreUpdated(scoreUpdated.NewScore, scoreUpdated.BonusPoints);
//                break;
//            default:
//                throw new ArgumentException($"Unknown player event type: {playerEvent.GetType().Name}");
//        }
//    }

//    public async Task BroadcastRoundResultsAsync(GameRound round)
//    {
//        await _hubContext.Clients.Group(_gameState.RoomCode).BroadcastRoundResults(round);
//    }

//    public async Task BroadcastFinalResultsAsync(GameResults results)
//    {
//        await _hubContext.Clients.Group(_gameState.RoomCode).BroadcastFinalResults(results);
//    }

//    public async Task<string> AskPlayerForThemeAsync(string connectionId, List<string> themes, TimeSpan timeout)
//    {
//        using var cts = new CancellationTokenSource(timeout);
//        return await _hubContext.Clients.Client(connectionId).AskTheme(themes, cts.Token);
//    }

//    public async Task<PlayerAnswer> AskPlayerForAnswerAsync(string connectionId, string imageUrl, TimeSpan timeout)
//    {
//        using var cts = new CancellationTokenSource(timeout);
//        var answer = await _hubContext.Clients.Client(connectionId).AskQuestion(imageUrl, cts.Token);
        
//        return new PlayerAnswer
//        {
//            PlayerConnectionId = connectionId,
//            Guess = answer.Guess,
//            IsGambling = answer.IsGambling
//        };
//    }
//} 