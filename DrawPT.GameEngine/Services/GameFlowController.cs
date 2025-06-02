using DrawPT.Common.Interfaces;
using DrawPT.Common.Models;
using DrawPT.Common.Services;
using DrawPT.GameEngine.Interfaces;
using RabbitMQ.Client;

namespace DrawPT.GameEngine.Services;

public class GameFlowController : IGameFlowController
{
    private readonly IRoundOrchestrator _roundOrchestrator;
    private readonly IModel _channel;
    private readonly ICacheService _cacheService;

    public GameFlowController(
        ICacheService cacheService,
        IConnection rabbitMqConnection,
        IRoundOrchestrator roundOrchestrator)
    {
        _cacheService = cacheService;
        _roundOrchestrator = roundOrchestrator;
        _channel = rabbitMqConnection.CreateModel();
    }

    public async Task PlayGameAsync(string roomCode)
    {
        await StartGameAsync();

        for (int i = 0; i < 8; i++)
        {
            var round = await StartNewRoundAsync(i);

            // Theme selection
            var theme = await _roundOrchestrator.SelectThemeAsync("1");

            // Question and answers
            var question = await _roundOrchestrator.GenerateQuestionAsync(theme);

            var players = await _cacheService.GetRoomPlayersAsync(roomCode);

            await Task.Delay(500);

            var answer = await _roundOrchestrator.RequestUserInputAsync("hi", players.First().ConnectionId, 60000);
            var answers = await _roundOrchestrator.CollectAnswersAsync(question);
            var assessedAnswers = await _roundOrchestrator.AssessAnswersAsync(question, answers);

            await EndRoundAsync(round);
        }

        await EndGameAsync();
    }

    public async Task StartGameAsync()
    {
        // Broadcast
    }

    public async Task<GameRound> StartNewRoundAsync(int roundNumber)
    {
        return new GameRound();
    }

    public async Task EndRoundAsync(GameRound round)
    {

    }

    public async Task EndGameAsync()
    {
    }

    public Task<bool> IsGameCompleteAsync()
    {
        return Task.FromResult(false);
    }
} 