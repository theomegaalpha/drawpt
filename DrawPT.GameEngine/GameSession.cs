using DrawPT.Common.Interfaces;
using DrawPT.Common.Models.Game;
using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine;

public class GameSession : IGameSession
{
    private readonly IGameCommunicationService _gameCommunicationService;
    private readonly ICacheService _cacheService;
    private readonly IQuestionService _questionService;
    private readonly ILogger<GameSession> _logger;

    public GameSession(
        ICacheService cacheService,
        IQuestionService questionService,
        IGameCommunicationService gameCommunicationService,
        ILogger<GameSession> logger)
    {
        _cacheService = cacheService;
        _gameCommunicationService = gameCommunicationService;
        _questionService = questionService;
        _logger = logger;
    }

    public async Task PlayGameAsync(string roomCode)
    {
        // Broadcast start game message
        // gameStateService.StartGame(roomCode);
        await Task.Delay(10000);

        for (int i = 0; i < 8; i++)
        {
            // gameStateService.StartRound(roomCode, i);
            await Task.Delay(10000);

            // ask player for theme
            var players = await _cacheService.GetRoomPlayersAsync(roomCode);
            var selectedTheme = await _gameCommunicationService.AskPlayerTheme(players.ElementAt(i%players.Count), 30);
            
            // ask all players for their answers
            var question = await _questionService.GenerateQuestionAsync(selectedTheme);
            question.RoundNumber = i + 1;
            List<Task<PlayerAnswer>> playerAnswers = new(players.Count);
            foreach (var player in players)
            {
                playerAnswers.Add(_gameCommunicationService.AskPlayerQuestion(player, question, 30));
            }

            // empty game check
            if (playerAnswers.Count == 0)
                break;

            await Task.WhenAll(playerAnswers);

            var answers = new List<PlayerAnswer>();
            foreach (var answer in playerAnswers.Select(t => t.Result))
                answers.Add(answer);

            // broadcast assessing answers message to players

            _logger.LogDebug($"[{roomCode}] Round {i + 1} answers collected: {answers.Count}");
            // scoringService
            // broadcast round scores to players
            // await _gameCommunicationService
        }

        // gameStateService.EndGame(roomCode);
        // broadcast end game scores to players
    }
} 