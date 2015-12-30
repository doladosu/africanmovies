using System.Collections.Generic;
using System.Linq;
using AfricanMovies.Backend.DataSources;
using AfricanMovies.Backend.DataSources.DTO;
using AfricanMovies.Tests.Integration.Fixtures;
using Should;
using Xunit;
using Xunit.Extensions;

namespace AfricanMovies.Tests.Integration
{
    public class VideosDataSourceTests : IUseFixture<VideoProvidersFixture>
    {
        private IVideosDataSource dataSource;
        private List<ChannelDTO> conferences;

        [Theory]
        [InlineData("Jake Fried")] // vimeo source
        [InlineData("Scott Hanselman")] // youtube source
        public void Query__conference_name_filter_applied__returns_videos_only_from_expected_conf(string conferenceName)
        {
            var matchingConferences = conferences
                .Where(r => r.Name == conferenceName)
                .ToList();

            matchingConferences.ShouldNotBeEmpty();
        }

        public void SetFixture(VideoProvidersFixture data)
        {
            dataSource = data.DataSource;

            // get all results once to minimize network requests
            conferences = dataSource.FindAll()
                .GetAwaiter()
                .GetResult();
        }
    }
}