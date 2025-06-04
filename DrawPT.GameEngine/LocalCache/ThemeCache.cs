using DrawPT.Data.Repositories;
using System.Collections.Concurrent;

namespace DrawPT.GameEngine.LocalCache
{
    public class ThemeCache
    {
        public ConcurrentDictionary<Guid, string> Themes { get; set; } = new();

        public void BuildCache(ReferenceRepository _repository)
        {
            var themes = _repository.GetAllThemes();
            foreach (var theme in themes)
            {
                Themes[theme.Id] = theme.Name;
            }
        }
    }
}
