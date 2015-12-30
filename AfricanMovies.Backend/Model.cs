using System;
using System.Collections.Generic;
using System.Linq;
using AfricanMovies.Backend.ViewModels;

namespace AfricanMovies.Backend
{
    public static class ModelTranformationsExtensions
    {
        public static IEnumerable<VideoViewModel> ToViewModel(this IEnumerable<Video> videos,
            IEnumerable<Channel> channels)
        {
            return videos.Select(v =>
            {
                var channel = channels.FirstOrDefault(c => c.Id == v.ChannelId) ?? new Channel();

                return new VideoViewModel
                {
                    Id = v.Id,
                    Title = v.Title,
                    Url = v.Link,
                    Description = v.Description,
                    Views = v.Stats.Views,
                    Dislikes = v.Stats.Dislikes,
                    Likes = v.Stats.Likes,
                    PublicationDate = v.PublicationDate,
                    ChannelInfo = new ChannelInfoViewModel()
                    {
                        Description = channel.Description,
                        Id = channel.Id,
                        Link = channel.Link,
                        Name = channel.Name
                    },
                    Tags = v.Tags.Select(t => t.Name).ToList()
                };
            });
        }
    }

    public class TagEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return string.IsNullOrWhiteSpace(x) == false
                   && string.IsNullOrWhiteSpace(y) == false
                   && x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            return obj
                .ToLower()
                .GetHashCode();
        }
    }

    public class Channel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }
    }

    public class Video
    {
        public string Id { get; set; }

        public string ChannelId { get; set; }

        public string Description { get; set; }

        public Statistics Stats { get; set; }

        public string Title { get; set; }

        public DateTime PublicationDate { get; set; }

        public Tag[] Tags { get; set; }

        public string Link { get; set; }
    }

    public class Tag
    {
        public string Name { get; set; }

        public string Slug { get; set; }
    }

    public class Statistics
    {
        public int Views { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }
    }
}
