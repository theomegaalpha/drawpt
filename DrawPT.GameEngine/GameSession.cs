using DrawPT.Common.Configuration;
using DrawPT.Common.Interfaces;
using DrawPT.Common.Models;
using DrawPT.Common.Models.Game;
using DrawPT.GameEngine.Interfaces;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Text.Json;

namespace DrawPT.GameEngine;

public class GameSession : IGameSession
{
    private readonly IGameCommunicationService _gameCommunicationService;
    private readonly IAssessmentService _assessmentService;
    private readonly IGameStateService _gameStateService;
    private readonly IQuestionService _questionService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GameSession> _logger;
    private readonly IModel _channel;
    private readonly EventingBasicConsumer _consumer;

    public GameSession(
        IConnection rabbitMqConnection,
        ICacheService cacheService,
        IQuestionService questionService,
        IGameStateService gameStateService,
        IAssessmentService assessmentService,
        IGameCommunicationService gameCommunicationService,

        ILogger<GameSession> logger)
    {
        _channel = rabbitMqConnection.CreateModel();
        _cacheService = cacheService;
        _gameStateService = gameStateService;
        _gameCommunicationService = gameCommunicationService;
        _questionService = questionService;
        _assessmentService = assessmentService;
        _logger = logger;


        _channel.ExchangeDeclare(ApiPlayerMQ.ExchangeName, ExchangeType.Topic);
        _consumer = new EventingBasicConsumer(_channel);
        _consumer.Received += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;

            _logger.LogInformation($"Received message with routing key: {routingKey}");

            try
            {
                // Extract room code from routing key (format: client_broadcast.{roomCode}.{action})
                var parts = routingKey.Split('.');
                var roomCode = parts[1];
                var action = parts[2];
                await HandleApiBroadcast(roomCode, action, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling api broadcast: {message}");
            }
        };
    }

    public async Task PlayGameAsync(string roomCode)
    {
        _channel.QueueDeclare(
            queue: ApiPlayerMQ.QueueName(roomCode),
            durable: false,
            exclusive: false,
            autoDelete: true
        );
        _channel.QueueBind(ApiPlayerMQ.QueueName(roomCode),
            ApiPlayerMQ.ExchangeName, ApiPlayerMQ.RoutingKey(roomCode));
        _channel.BasicConsume(queue: ApiPlayerMQ.QueueName(roomCode), autoAck: true, consumer: _consumer);

        // Broadcast start game message
        var gameState = await _gameStateService.StartGameAsync(roomCode);
        _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineBroadcastMQ.GameStartedAction, gameState);
        await Task.Delay(100);

        List<RoundResults> allRoundResults = new();
        List<Player> originalPlayers = await _cacheService.GetRoomPlayersAsync(roomCode);

        for (int i = 0; i < gameState.TotalRounds; i++)
        {
            gameState = await _gameStateService.StartRoundAsync(roomCode, i + 1);
            _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineBroadcastMQ.RoundStartedAction, i + 1);
            await Task.Delay(100);

            // ask player for theme
            var players = await _cacheService.GetRoomPlayersAsync(roomCode);
            // add players that are missing from original list into originalPlayers
            foreach (var player in players.Where(p => !originalPlayers.Any(op => op.Id == p.Id)))
                originalPlayers.Add(player);
            var selectedTheme = await _gameCommunicationService.AskPlayerThemeAsync(players.ElementAt(i % players.Count), 30);
            _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineBroadcastMQ.PlayerThemeSelectedAction, selectedTheme);

            // ask all players for their answers
            var question = await _questionService.GenerateQuestionAsync(selectedTheme);
            question.RoundNumber = i + 1;
            List<Task<PlayerAnswer>> playerAnswers = new(players.Count);
            foreach (var player in players)
            {
                playerAnswers.Add(_gameCommunicationService.AskPlayerQuestionAsync(player, question, 30));
            }

            // empty game check
            if (playerAnswers.Count == 0)
                break;

            await Task.WhenAll(playerAnswers);

            var answers = new List<PlayerAnswer>();
            foreach (var answer in playerAnswers.Select(t => t.Result))
                answers.Add(answer);


            _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineBroadcastMQ.AssessingAnswersAction);

            var assessedAnswers = await _assessmentService.AssessAnswersAsync(question.OriginalPrompt, answers);
            var roundResults = new RoundResults
            {
                RoundNumber = i + 1,
                Theme = selectedTheme,
                Question = question,
                Answers = assessedAnswers
            };
            allRoundResults.Add(roundResults);
            _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineBroadcastMQ.RoundResultsAction, roundResults);
            await Task.Delay(gameState.GameConfiguration.TransitionDelay * 1000);
        }

        var finalGameState = await _gameStateService.EndGameAsync(roomCode);

        var allAnswers = allRoundResults.SelectMany(r => r.Answers);
        var playerScores = allAnswers
            .GroupBy(a => a.PlayerId)
            .ToDictionary(
                g => g.Key,
                g => new PlayerResults
                {
                    PlayerId = g.Key,
                    Score = g.Sum(a => a.Score + a.BonusPoints),
                    Username = originalPlayers.FirstOrDefault(p => p.Id == g.Key)?.Username ?? "Unknown",
                }
            );
        var finalScores = new GameResults
        {
            PlayerResults = playerScores.Values.ToList(),
            WasCompleted = true,
            TotalRounds = gameState.TotalRounds
        };
        _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineBroadcastMQ.GameResultsAction, finalScores);
    }


    private async Task HandleApiBroadcast(string roomCode, string action, string message)
    {
        switch (action)
        {
            case ApiPlayerMQ.PlayerLeftAction:
                var player = JsonSerializer.Deserialize<Player>(message);
                await _cacheService.RemovePlayerFromRoom(roomCode, player);
                _gameCommunicationService.BroadcastGameEvent(roomCode, GameEngineBroadcastMQ.PlayerLeftAction, player);
                break;
        }
    }
}