using DrawPT.Common.Models.Game;

namespace DrawPT.GameEngine.Interfaces;

public interface IThemeService
{
    Task<List<GameTheme>> GetRandomThemesAsync();
} 