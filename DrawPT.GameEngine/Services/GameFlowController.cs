using DrawPT.GameEngine.Interfaces;
using DrawPT.Common.Models;
using RabbitMQ.Client;

namespace DrawPT.GameEngine.Services;

public class GameFlowController : IGameFlowController
{
    private readonly IRoundOrchestrator _roundOrchestrator;
    private readonly IModel _channel;

    public GameFlowController(
        IConnection rabbitMqConnection,
        IRoundOrchestrator roundOrchestrator)
    {
        _roundOrchestrator = roundOrchestrator;
        _channel = rabbitMqConnection.CreateModel();
    }

    public async Task PlayGameAsync()
    {
        await StartGameAsync();

        for (int i = 0; i < 8; i++)
        {
            var round = await StartNewRoundAsync(i);

            // Theme selection
            var theme = await _roundOrchestrator.SelectThemeAsync("1");

            // Question and answers
            var question = await _roundOrchestrator.GenerateQuestionAsync(theme);
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