using DrawPT.Common.Interfaces.Game;
using DrawPT.GameEngine.LocalCache;

namespace DrawPT.GameEngine.Services;

public class ThemeService : IThemeService
{
    private readonly ThemeCache _referenceCache;

    public ThemeService(ThemeCache referenceCache)
    {
        _referenceCache = referenceCache;
    }

    public List<string> GetRandomThemes(int count = 5)
    {
        List<string> themes = new();
        Random random = new();
        var themeKeys = _referenceCache.Themes.Keys.ToList();

        // If there are fewer themes than requested, adjust the count
        count = Math.Min(count, themeKeys.Count);

        for (int i = 0; i < count; i++)
        {
            int index = random.Next(themeKeys.Count);
            themes.Add(_referenceCache.Themes[themeKeys[index]]);
            themeKeys.RemoveAt(index);
        }

        return themes;
    }
} 