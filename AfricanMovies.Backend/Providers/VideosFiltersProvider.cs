using System.Collections.Generic;
using System.Linq;
using AfricanMovies.Backend.ViewModels;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AfricanMovies.Backend.Providers
{
    public class VideosFiltersProvider : IFiltersProvider<VideosFiltersViewModel>
    {
        private readonly MongoDatabase db;

        public VideosFiltersProvider(MongoDatabase db)
        {
            this.db = db;
        }

        public VideosFiltersViewModel TakeAvailableFilters()
        {
            var channels = GetAllChannels();
            var tags = GetAllTags();
            var pubYears = GetPublicationYears();

            return new VideosFiltersViewModel()
            {
                Channels = channels,
                Tags = tags,
                PublicationYears = pubYears
            };
        }

        private List<int> GetPublicationYears()
        {
            return db.GetCollection<Video>("Videos")
                .AsQueryable()
                .Select(v => v.PublicationDate.Year)
                .ToList()
                .Distinct()
                .OrderBy(y => y)
                .ToList();
        }

        private List<string> GetAllTags()
        {
            return db.GetCollection<Video>("Videos")
                .AsQueryable()
                .Select(v => v.Tags)
                .ToList()
                .SelectMany(t => t)
                .Select(t => t.Name)
                .OrderBy(t => t)
                .Distinct(new TagEqualityComparer())
                .ToList();
        }

        private List<string> GetAllChannels()
        {
            return db.GetCollection<Channel>("Channels")
                .AsQueryable()
                .Select(c => c.Name)
                .ToList()
                .OrderBy(c => c)
                .ToList();
        }
    }
}