namespace DrawPT.Common.ServiceBus
{
    public static class GameEngineQueue
    {
        public const string Name = "gameEngine";

        public const string PlayerJoinedAction = "player_joined";
        public const string PlayerLeftAction = "player_left";
        public const string PlayerAnsweredAction = "player_answered";
        public const string PlayerGambledAction = "player_gambled";
        public const string PlayerScoreUpdateAction = "player_score_update";
        public const string PlayerThemeSelectedAction = "player_theme_selected";
        public const string PlayerImagePromptSelectedAction = "player_image_prompt_selected";
        public const string PlayerGamblingAction = "player_gambling";

        public const string GameStartedAction = "game_started";
        public const string RoundStartedAction = "round_started";
        public const string AssessingAnswersAction = "assessing_answers";
        public const string RoundResultsAction = "round_results";
        public const string GambleResultsAction = "gamble_results";
        public const string GameResultsAction = "game_ended";

        public const string AnnouncerAction = "announcer";

        public const string WriteMessageAction = "write_message";
    }
}
