using DrawPT.Api.Models;
using DrawPT.Data.Repositories;
using System.Collections.Concurrent;

namespace DrawPT.Api.Cache
{
    public class ReferenceCache
    {
        private readonly ReferenceRepository _repository;
        public List<string> Adjectives { get; set; }
        public List<string> Nouns { get; set; }
        public List<ItemType> ItemTypes { get; set; }
        public ConcurrentDictionary<Guid, string> Themes { get; set; } = new();

        public ReferenceCache(ReferenceRepository referenceRepository)
        {
            _repository = referenceRepository;
        }

        public void BuildCache()
        {
            Adjectives = _repository.GetAllAdjectives();
            Nouns = _repository.GetAllNouns();
            ItemTypes = _repository.GetAllItemTypes();
            var themes = _repository.GetAllThemes();
            foreach (var theme in themes)
            {
                Themes[theme.Id] = theme.Name;
            }
        }
    }
}
