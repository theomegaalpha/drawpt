using DrawPT.Common.Interfaces;
using DrawPT.Common.Models;
using RabbitMQ.Client;

namespace DrawPT.GameEngine.Managers
{
    /// <summary>
    /// Manages game questions and themes
    /// </summary>
    public class QuestionManager : IQuestionManager
    {
        private readonly ILogger<QuestionManager> _logger;
        private readonly IModel _channel;
        private readonly string _gameId;
        private readonly GameConfiguration _configuration;
        private readonly IAIService _aiService;
        private readonly IThemeRepository _themeRepository;
        private readonly Random _random = new();
        private GameTheme? _currentTheme;

        public QuestionManager(
            ILogger<QuestionManager> logger,
            IConnection rabbitMqConnection,
            string gameId,
            GameConfiguration configuration,
            IAIService aiService,
            IThemeRepository themeRepository)
        {
            _logger = logger;
            _gameId = gameId;
            _configuration = configuration;
            _aiService = aiService;
            _themeRepository = themeRepository;

            // Set up RabbitMQ channel
            _channel = rabbitMqConnection.CreateModel();
            _channel.ExchangeDeclare("game_events", ExchangeType.Topic, true);

            // Declare queue for question events
            _channel.QueueDeclare($"question_events_{gameId}", true, false, false);
            _channel.QueueBind($"question_events_{gameId}", "game_events", $"game.{gameId}.question.*");
        }

        /// <summary>
        /// Gets the current theme
        /// </summary>
        public GameTheme? CurrentTheme => _currentTheme;

        /// <summary>
        /// Gets all available themes
        /// </summary>
        public async Task<IEnumerable<GameTheme>> GetAvailableThemesAsync()
        {
            return await _themeRepository.GetActiveThemesAsync();
        }

        /// <summary>
        /// Selects a theme for the current round
        /// </summary>
        public async Task<GameTheme> SelectThemeAsync(string playerId)
        {
            var themes = await GetAvailableThemesAsync();
            var themeList = themes.ToList();

            if (!themeList.Any())
            {
                throw new InvalidOperationException("No active themes available");
            }

            // Select a random theme
            _currentTheme = themeList[_random.Next(themeList.Count)];

            // Publish theme selected event
            var message = new
            {
                EventType = "ThemeSelected",
                GameId = _gameId,
                PlayerId = playerId,
                Theme = _currentTheme
            };

            var body = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message));
            _channel.BasicPublish(
                exchange: "game_events",
                routingKey: $"game.{_gameId}.question.theme_selected",
                basicProperties: null,
                body: body);

            _logger.LogInformation($"Player {playerId} selected theme {_currentTheme.Name} in game {_gameId}");

            return _currentTheme;
        }

        /// <summary>
        /// Generates a new question based on the current theme
        /// </summary>
        public async Task<GameQuestion> GenerateQuestionAsync()
        {
            if (_currentTheme == null)
            {
                throw new InvalidOperationException("No theme selected");
            }

            try
            {
                var question = await _aiService.GenerateGameQuestionAsync();

                // Publish question generated event
                var message = new
                {
                    EventType = "QuestionGenerated",
                    GameId = _gameId,
                    Theme = _currentTheme,
                    Question = question
                };

                var body = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message));
                _channel.BasicPublish(
                    exchange: "game_events",
                    routingKey: $"game.{_gameId}.question.generated",
                    basicProperties: null,
                    body: body);

                _logger.LogInformation($"Generated question for theme {_currentTheme.Name} in game {_gameId}");

                return question;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to generate question for theme {_currentTheme.Name} in game {_gameId}");
                throw;
            }
        }

        /// <summary>
        /// Gets a random theme from the available themes
        /// </summary>
        public async Task<GameTheme> GetRandomThemeAsync()
        {
            var themes = await GetAvailableThemesAsync();
            var themeList = themes.ToList();

            if (!themeList.Any())
            {
                throw new InvalidOperationException("No active themes available");
            }

            return themeList[_random.Next(themeList.Count)];
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
} 