using DrawPT.Common.Models.Game;

namespace DrawPT.GameEngine.Interfaces;

public interface IThemeService
{
    Task<List<string>> GetRandomThemesAsync();
} 