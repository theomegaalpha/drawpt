using DrawPT.GameEngine.Events;
using DrawPT.GameEngine.Interfaces;

namespace DrawPT.GameEngine.Helpers
{
    public static class RabbitMQHelpers
    {
        public static string GetRoutingKey(IGameEvent gameEvent)
        {
            return gameEvent switch
            {
                GameStartedEvent e => GameEventRouting.CreateGameRoutingKey(e.GameId, gameEvent.EventType),
                GameEndedEvent e => GameEventRouting.CreateGameRoutingKey(e.GameId, gameEvent.EventType),
                PlayerJoinedEvent e => GameEventRouting.CreatePlayerRoutingKey(e.GameId, e.PlayerId, gameEvent.EventType),
                PlayerLeftEvent e => GameEventRouting.CreatePlayerRoutingKey(e.GameId, e.PlayerId, gameEvent.EventType),
                PlayerScoreUpdatedEvent e => GameEventRouting.CreatePlayerRoutingKey(e.GameId, e.PlayerId, gameEvent.EventType),
                RoundStartedEvent e => GameEventRouting.CreateRoundRoutingKey(e.GameId, e.RoundNumber, gameEvent.EventType),
                RoundEndedEvent e => GameEventRouting.CreateRoundRoutingKey(e.GameId, e.RoundNumber, gameEvent.EventType),
                AnswerSubmittedEvent e => GameEventRouting.CreateRoundRoutingKey(e.GameId, e.RoundNumber, gameEvent.EventType),
                ThemeSelectedEvent e => GameEventRouting.CreateQuestionRoutingKey(e.GameId, e.QuestionId, gameEvent.EventType),
                QuestionGeneratedEvent e => GameEventRouting.CreateQuestionRoutingKey(e.GameId, e.QuestionId, gameEvent.EventType),
                _ => throw new ArgumentException($"Unknown event type: {gameEvent.EventType}")
            };
        }
    }
}
