using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine;

public class GameOrchestrator
{
    private readonly IGameEngine _gameEngine;
    private readonly IGameStateManager _stateManager;
    private readonly ILogger<GameOrchestrator> _logger;
    private readonly GameConfiguration _configuration;

    public GameOrchestrator(
        IGameEngine gameEngine,
        IGameStateManager stateManager,
        ILogger<GameOrchestrator> logger,
        GameConfiguration configuration)
    {
        _gameEngine = gameEngine;
        _stateManager = stateManager;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task PlayGameAsync(string roomCode, CancellationToken cancellationToken)
    {
        try
        {
            // Initialize game state
            await _gameEngine.InitializeGameAsync(roomCode, _configuration);
            await _gameEngine.StartGameAsync(roomCode);

            // Main game loop
            for (var roundNumber = 0; roundNumber < _configuration.NumberOfQuestions; roundNumber++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var state = await _gameEngine.GetGameStateAsync(roomCode);

                // Empty game check
                if (state.Players.Count == 0)
                    break;

                // Theme selection
                var themePlayer = state.Players.Values.ElementAt(roundNumber % state.Players.Count);
                var theme = await SelectThemeAsync(roomCode, themePlayer.ConnectionId, cancellationToken);

                // Generate and ask question
                var question = await _gameEngine.GenerateQuestionAsync(roomCode, theme);
                var round = new GameRound
                {
                    RoundNumber = roundNumber,
                    Question = question
                };
                await _stateManager.AddGameRoundAsync(roomCode, round);

                // Collect answers
                var answerTasks = new List<Task<GameAnswer>>();
                foreach (var player in state.Players.Values)
                {
                    answerTasks.Add(CollectAnswerAsync(roomCode, player.ConnectionId, question, cancellationToken));
                }

                // Wait for all answers or timeout
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_configuration.QuestionTimeout));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

                try
                {
                    await Task.WhenAll(answerTasks);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Round {RoundNumber} timed out", roundNumber);
                }

                // Assess answers
                var answers = answerTasks
                    .Where(t => t.IsCompletedSuccessfully)
                    .Select(t => t.Result)
                    .ToList();

                var assessedAnswers = await _gameEngine.AssessAnswersAsync(roomCode, roundNumber);

                // Update scores
                foreach (var answer in assessedAnswers)
                {
                    if (state.Players.TryGetValue(answer.PlayerConnectionId, out var playerState))
                    {
                        playerState.Score += answer.Score + answer.BonusPoints;
                        await _stateManager.UpdatePlayerStateAsync(roomCode, answer.PlayerConnectionId, playerState);
                    }
                }

                // Transition delay between rounds
                if (roundNumber < _configuration.NumberOfQuestions - 1)
                {
                    await Task.Delay(_configuration.TransitionDelay * 1000, cancellationToken);
                }
            }

            // End game and calculate final results
            await _gameEngine.EndGameAsync(roomCode);
            var results = await _gameEngine.GetGameResultsAsync(roomCode);
            await _stateManager.PublishGameEventAsync(roomCode, "GameCompleted", results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during game play for room {RoomCode}", roomCode);
            await _stateManager.UpdateGameStatusAsync(roomCode, GameStatus.Error);
            throw;
        }
    }

    private async Task<string> SelectThemeAsync(string roomCode, string playerConnectionId, CancellationToken cancellationToken)
    {
        // Wait for theme selection from the designated player
        var themeSelectionTimeout = TimeSpan.FromSeconds(_configuration.ThemeTimeout);
        using var cts = new CancellationTokenSource(themeSelectionTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        try
        {
            // This would be implemented by the game hub to handle the actual theme selection
            // For now, we'll use a placeholder theme
            return "Default Theme";
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Theme selection timed out for player {PlayerId}", playerConnectionId);
            return "Default Theme";
        }
    }

    private async Task<GameAnswer> CollectAnswerAsync(
        string roomCode,
        string playerConnectionId,
        GameQuestion question,
        CancellationToken cancellationToken)
    {
        try
        {
            // This would be implemented by the game hub to handle the actual answer collection
            // For now, we'll use a placeholder answer
            return await _gameEngine.SubmitAnswerAsync(roomCode, playerConnectionId, "Default Answer", false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting answer from player {PlayerId}", playerConnectionId);
            throw;
        }
    }
}