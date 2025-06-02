using DrawPT.Common.Models;

namespace DrawPT.GameEngine.Interfaces;

public interface IThemeService
{
    Task<List<GameTheme>> GetRandomThemesAsync();
} 