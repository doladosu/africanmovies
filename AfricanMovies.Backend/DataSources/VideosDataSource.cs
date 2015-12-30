using System.Collections.Generic;
using System.Threading.Tasks;
using AfricanMovies.Backend.DataSources.DTO;
using AfricanMovies.Backend.Providers;

namespace AfricanMovies.Backend.DataSources
{
    public class VideosDataSource : IVideosDataSource
    {
        private readonly IReadOnlyList<IVideoProvider> providers;

        public VideosDataSource()
            : this(new List<IVideoProvider>()
            {
                new VimeoProvider(),
                new YouTubeProvider()
            })
        {
            
        }

        public VideosDataSource(IReadOnlyList<IVideoProvider> videoProviders)
        {
            providers = videoProviders;
        }

        public async Task<List<ChannelDTO>> FindAll()
        {
            var results = new List<ChannelDTO>();

            foreach (var provider in providers)
            {
                results.AddRange(await provider.ChannelsData());
            }

            return results;
        }
    }
}