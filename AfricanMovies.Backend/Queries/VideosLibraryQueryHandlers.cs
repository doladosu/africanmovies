using System;
using System.Collections.Generic;
using System.Linq;
using AfricanMovies.Backend.Extensions;
using AfricanMovies.Backend.Infrastructure;
using AfricanMovies.Backend.ViewModels;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AfricanMovies.Backend.Queries
{
    public class VideosLibraryQueryHandlers : IQueryHandler<List<VideoViewModel>, FindVideosQuery>
    {
        private readonly MongoDatabase db;

        public VideosLibraryQueryHandlers(MongoDatabase db)
        {
            this.db = db;
        }

        public List<VideoViewModel> Handle(FindVideosQuery query)
        {
            if(query == null)
                query = new FindVideosQuery();

            Func<Channel, bool> withName = vid =>
            {
                if (string.IsNullOrEmpty(query.ChannelName) == false)
                    return vid.Name == query.ChannelName;

                return true;
            };

            Func<Video, bool> withPublicationYear = vid =>
            {

                if (query.PublicationYear.HasValue)
                    return vid.PublicationDate.Year == query.PublicationYear;

                return true;
            };

            Func<Video, bool> withTags = vid =>
            {
                if (query.Tags != null && query.Tags.Any())
                    return vid
                        .Tags
                        .Select(t => t.Name.ToLower())
                        .ContainsAny(query.Tags);

                return true;
            };

            Func<IEnumerable<Channel>, Func<Video, bool>> withChannels = matchingChannels =>
            {
                var ids = matchingChannels.Select(c => c.Id);

                Func<Video, bool> withChannel = vid => ids.Contains(vid.ChannelId);

                return withChannel;
            };

            var channels = db.GetCollection<Channel>("Channels")
                .FindAll()
                .Where(withName)
                .ToList();

            var videos = db.GetCollection<Video>("Videos")
                .FindAll()
                .Where(withPublicationYear)
                .Where(withTags)
                .Where(withChannels(channels));

            query.Pagination.NumberOfRecords = videos.Count();

            return videos
                .Skip(query.Pagination.NumberOfRecordsToSkip)
                .Take(query.Pagination.PerPage)
                .ToViewModel(channels)
                .AsQueryable()
                .OrderBy(query.OrderBy.PropertyName, query.OrderBy.Direction)
                .ToList();
        }
    }
}