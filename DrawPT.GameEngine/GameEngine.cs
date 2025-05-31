using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine;

public class GameEngine : IGameEngine
{
    private readonly IGameStateManager _stateManager;
    private readonly IAIClient _aiClient;
    private readonly ILogger<GameEngine> _logger;

    public GameEngine(
        IGameStateManager stateManager,
        IAIClient aiClient,
        ILogger<GameEngine> logger)
    {
        _stateManager = stateManager;
        _aiClient = aiClient;
        _logger = logger;
    }

    public async Task<bool> InitializeGameAsync(string roomCode, GameConfiguration configuration)
    {
        var state = await _stateManager.GetGameStateAsync(roomCode);
        state.Status = GameStatus.WaitingForPlayers;
        await _stateManager.SaveGameStateAsync(roomCode, state);
        return true;
    }

    public async Task<bool> AddPlayerAsync(string roomCode, Player player)
    {
        return await _stateManager.AddPlayerAsync(roomCode, player);
    }

    public async Task<bool> RemovePlayerAsync(string roomCode, string connectionId)
    {
        return await _stateManager.RemovePlayerAsync(roomCode, connectionId);
    }

    public async Task<GameState> GetGameStateAsync(string roomCode)
    {
        return await _stateManager.GetGameStateAsync(roomCode);
    }

    public async Task<GameQuestion> GenerateQuestionAsync(string roomCode, string theme)
    {
        try
        {
            var question = await _aiClient.GenerateGameQuestionAsync(theme);
            return new GameQuestion
            {
                Id = question.Id,
                ImageUrl = question.ImageUrl,
                OriginalPrompt = question.OriginalPrompt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating question for theme {Theme}", theme);
            throw;
        }
    }

    public async Task<GameAnswer> SubmitAnswerAsync(string roomCode, string connectionId, string guess, bool isGambling)
    {
        var state = await _stateManager.GetGameStateAsync(roomCode);
        var currentRound = state.GameRounds.LastOrDefault();
        
        if (currentRound == null)
        {
            throw new InvalidOperationException("No active round found");
        }

        var answer = new GameAnswer
        {
            PlayerConnectionId = connectionId,
            Guess = guess,
            IsGambling = isGambling,
            Score = 0,
            BonusPoints = 0,
            Reason = "Pending assessment"
        };

        currentRound.Answers.Add(answer);
        await _stateManager.SaveGameStateAsync(roomCode, state);
        
        return answer;
    }

    public async Task<List<GameAnswer>> AssessAnswersAsync(string roomCode, int roundNumber)
    {
        var state = await _stateManager.GetGameStateAsync(roomCode);
        var round = state.GameRounds.FirstOrDefault(r => r.RoundNumber == roundNumber);
        
        if (round == null)
        {
            throw new InvalidOperationException($"Round {roundNumber} not found");
        }

        try
        {
            var assessmentJson = await _aiClient.GenerateAssessmentAsync(
                round.Question.OriginalPrompt,
                round.Answers);

            var assessedAnswers = System.Text.Json.JsonSerializer.Deserialize<List<GameAnswer>>(assessmentJson) ?? [];
            round.Answers = assessedAnswers;
            await _stateManager.SaveGameStateAsync(roomCode, state);
            
            return assessedAnswers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assessing answers for round {RoundNumber}", roundNumber);
            throw;
        }
    }

    public async Task<GameResults> GetGameResultsAsync(string roomCode)
    {
        var state = await _stateManager.GetGameStateAsync(roomCode);
        var results = new GameResults();
        var playerResults = new Dictionary<string, PlayerResult>();

        foreach (var player in state.Players.Values)
        {
            playerResults[player.ConnectionId] = new PlayerResult
            {
                Id = player.Player.Id,
                ConnectionId = player.ConnectionId,
                Username = player.Player.Username,
                FinalScore = 0
            };
        }

        foreach (var round in state.GameRounds)
        {
            foreach (var answer in round.Answers)
            {
                if (playerResults.TryGetValue(answer.PlayerConnectionId, out var result))
                {
                    result.FinalScore += answer.Score + answer.BonusPoints;
                }
            }
        }

        results.PlayerResults = playerResults.Values.ToList();
        return results;
    }

    public async Task<bool> StartGameAsync(string roomCode)
    {
        return await _stateManager.UpdateGameStatusAsync(roomCode, GameStatus.InProgress);
    }

    public async Task<bool> EndGameAsync(string roomCode)
    {
        return await _stateManager.UpdateGameStatusAsync(roomCode, GameStatus.Completed);
    }

    public async Task<bool> IsGameCompleteAsync(string roomCode)
    {
        var state = await _stateManager.GetGameStateAsync(roomCode);
        return state.Status == GameStatus.Completed;
    }

    public async Task<bool> IsPlayerTurnAsync(string roomCode, string connectionId)
    {
        var state = await _stateManager.GetGameStateAsync(roomCode);
        var currentRound = state.GameRounds.LastOrDefault();
        
        if (currentRound == null)
            return false;

        // Check if it's this player's turn to select a theme
        var playerIndex = state.Players.Keys.ToList().IndexOf(connectionId);
        return playerIndex == currentRound.RoundNumber % state.Players.Count;
    }

    public async Task<string> GetCurrentThemeAsync(string roomCode)
    {
        var state = await _stateManager.GetGameStateAsync(roomCode);
        var currentRound = state.GameRounds.LastOrDefault();
        return currentRound?.Question.OriginalPrompt ?? string.Empty;
    }

    public async Task<bool> SetThemeAsync(string roomCode, string theme)
    {
        var state = await _stateManager.GetGameStateAsync(roomCode);
        var currentRound = state.GameRounds.LastOrDefault();
        
        if (currentRound != null)
        {
            currentRound.Question.OriginalPrompt = theme;
            await _stateManager.SaveGameStateAsync(roomCode, state);
            return true;
        }

        return false;
    }
} 