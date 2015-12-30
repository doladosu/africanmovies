using System;
using System.Linq;
using AfricanMovies.Backend;
using AfricanMovies.Backend.Providers;
using AfricanMovies.Backend.ViewModels;
using AfricanMovies.Tests.Integration.AutoData;
using Ploeh.AutoFixture;
using Should;
using Xunit;

namespace AfricanMovies.Tests.Integration
{
    public class VideosFiltersTests : WithPopulatedMongoDb
    {

        private VideosFiltersViewModel execute()
        {
            return new VideosFiltersProvider(db)
                .TakeAvailableFilters();
        }

        [Fact]
        public void Channels__get_all__returns_list_of_available_channels()
        {
            var actualChannels = execute().Channels;

            var expectedChannels = channels
                .Select(c => c.Name)
                .OrderBy(c => c);

            actualChannels
                .SequenceEqual(expectedChannels)
                .ShouldBeTrue();
        }

        [Fact]
        public void Tags__get_all__returns_list_of_available_tags()
        {
            var actualTags = execute().Tags;

            var expectedTags = videos
                .SelectMany(v => v.Tags)
                .Select(t => t.Name)
                .OrderBy(t => t)
                .Distinct();

            actualTags
                .SequenceEqual(expectedTags)
                .ShouldBeTrue();
        }

        [Fact]
        public void Tags__is_not_case_sensitive__returns_distinct_result()
        {
            var fixture = new Fixture();
            var tag = fixture.Create<string>();
            var randomVideo = new Video()
            {
                Id = fixture.Create<string>()
            };

            randomVideo.Tags = new[]
            {
                new Tag()
                {
                    Name = tag.ToLower()
                },
                new Tag()
                {
                    Name = tag.ToUpper()
                },
            };

            db.GetCollection<Video>("Videos").Insert(randomVideo);

            var actualTags = execute().Tags;

            const int expectedNumberOfTags = 1;

            actualTags
                .Count(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase))
                .ShouldEqual(expectedNumberOfTags);
        }

        [Fact]
        public void PublicationYears__get_all__returns_list_of_available_publication_years()
        {
            var actualPublicationYears = execute().PublicationYears;

            var expectedPublicationYears = videos
                .Select(v => v.PublicationDate.Year)
                .Distinct()
                .OrderBy(y => y);

            actualPublicationYears
                .SequenceEqual(expectedPublicationYears)
                .ShouldBeTrue();
        }
    }
}