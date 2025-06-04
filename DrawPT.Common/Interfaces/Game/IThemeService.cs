namespace DrawPT.Common.Interfaces.Game;

public interface IThemeService
{
    /// <summary>
    /// Get 5 random themes from the reference cache.
    /// </summary>
    /// <param name="count">5 by default</param>
    /// <returns></returns>
    List<string> GetRandomThemes(int count = 5);
}