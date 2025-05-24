using DrawPT.Data.Models;
using DrawPT.Data.Repositories;
using System.Collections.Concurrent;

namespace DrawPT.Api.Cache
{
    public class ReferenceCache
    {
        public List<string> Adjectives { get; set; }
        public List<string> Nouns { get; set; }
        public List<ItemType> ItemTypes { get; set; }
        public ConcurrentDictionary<Guid, string> Themes { get; set; } = new();

        public void BuildCache(ReferenceRepository _repository)
        {
            Adjectives = _repository.GetAllAdjectives();
            Nouns = _repository.GetAllNouns();
            var themes = _repository.GetAllThemes();
            foreach (var theme in themes)
            {
                Themes[theme.Id] = theme.Name;
            }
        }
    }
}
