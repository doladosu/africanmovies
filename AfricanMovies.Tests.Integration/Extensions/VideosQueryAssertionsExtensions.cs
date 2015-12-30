using System.Collections.Generic;
using System.Linq;
using AfricanMovies.Backend;
using AfricanMovies.Backend.ViewModels;
using Should;

namespace AfricanMovies.Tests.Integration.Extensions
{
   public static class VideosQueryAssertionsExtensions
    {
        public static void ShouldEqual(this IEnumerable<VideoViewModel> actual,
            IEnumerable<VideoViewModel> expected)
        {
            actual.Count().ShouldEqual(expected.Count());

            for (var i = 0; i < expected.Count(); i++)
            {
                var a = actual.ElementAt(i);
                var e = actual.ElementAt(i);

                a.Id.ShouldEqual(e.Id);
                a.Likes.ShouldEqual(e.Likes);
                a.Dislikes.ShouldEqual(e.Dislikes);
                a.Views.ShouldEqual(e.Views);
                a.Title.ShouldEqual(e.Title);
                a.Description.ShouldEqual(e.Description);
                a.PublicationDate.ShouldEqual(e.PublicationDate);
                a.Url.ShouldEqual(e.Url);
                a.ChannelInfo.Description.ShouldEqual(e.ChannelInfo.Description);
                a.ChannelInfo.Name.ShouldEqual(e.ChannelInfo.Name);
                a.ChannelInfo.Id.ShouldEqual(e.ChannelInfo.Id);
                a.ChannelInfo.Link.ShouldEqual(e.ChannelInfo.Link);
                a.Tags.SequenceEqual(e.Tags).ShouldBeTrue();
            }
        }

        public static IEnumerable<VideoViewModel> TransformToExpectedViewModel(this IEnumerable<Video> videos,
             IEnumerable<Channel> channels, int numberOfElementsPerPage)
        {
            return videos
                .Take(numberOfElementsPerPage)
                .Select(v =>
            {
                var channel = channels.FirstOrDefault(c => c.Id == v.ChannelId) ?? new Channel();

                return new VideoViewModel
                {
                    Id = v.Id,
                    Title = v.Title,
                    Url = v.Link,
                    Description = v.Description,
                    Dislikes = v.Stats.Dislikes,
                    Views = v.Stats.Views,
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
}