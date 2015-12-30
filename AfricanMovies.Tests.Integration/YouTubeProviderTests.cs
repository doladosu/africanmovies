using System.Linq;
using System.Threading.Tasks;
using AfricanMovies.Backend.Providers;
using AfricanMovies.Tests.Integration.Extensions;
using AfricanMovies.Tests.Integration.Fixtures;
using Ploeh.AutoFixture;
using Xunit;

namespace AfricanMovies.Tests.Integration
{
    public class YouTubeProviderTests : IUseFixture<VideoProvidersFixture>
    {
        private IVideoProvider provider;

        [Fact]
        public async Task ChannelsData__explore_channel__returns_complete_info()
        {
            var conferences = await provider.ChannelsData();

            // YouTube doesn't provide tags
            var dummyTags = new Fixture().CreateMany<string>().ToList();

            conferences.ForEach(c => c.Videos.ForEach(v => v.Tags = dummyTags));

            conferences.ShouldBeFilledCorrectly();
        }

        public void SetFixture(VideoProvidersFixture data)
        {
            provider = data.YouTubeProvider;
        }
    }
}