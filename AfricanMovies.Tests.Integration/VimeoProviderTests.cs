using System.Threading.Tasks;
using AfricanMovies.Backend.Providers;
using AfricanMovies.Tests.Integration.Extensions;
using AfricanMovies.Tests.Integration.Fixtures;
using Ploeh.AutoFixture;
using Xunit;

namespace AfricanMovies.Tests.Integration
{
    public class VimeoProviderTests : IUseFixture<VideoProvidersFixture>
    {
        IVideoProvider provider;

        [Fact]
        public async Task ChannelsData__explore_channel__returns_complete_info()
        {
            var conferences = await provider.ChannelsData();

            // Vimeo doesn't provide dislikes
            conferences.ForEach(c => c.Videos.ForEach(v => v.Dislikes = new Fixture().Create<int>()));

            conferences.ShouldBeFilledCorrectly();
        }

        public void SetFixture(VideoProvidersFixture data)
        {
            provider = data.VimeoProvider;
        }
    }
}
