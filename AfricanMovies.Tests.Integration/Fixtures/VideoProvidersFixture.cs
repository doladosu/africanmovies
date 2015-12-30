using System.Collections.Generic;
using AfricanMovies.Backend.DataSources;
using AfricanMovies.Backend.Providers;

namespace AfricanMovies.Tests.Integration.Fixtures
{
    public class VideoProvidersFixture
    {
        internal IVideosDataSource DataSource
        {
            get
            {
                return new VideosDataSource(new List<IVideoProvider>()
                {
                    VimeoProvider,
                    YouTubeProvider
                });
            }
        }

        internal IVideoProvider VimeoProvider
        {
            get
            {
                return new VimeoProvider(new VimeoProvider.VimeoConfig(), new List<string>()
                {
                    "jakefried", // random account from vimeo which has small number of videos and update profile
                });
            }
        }

        internal IVideoProvider YouTubeProvider
        {
            get
            {
                return new YouTubeProvider(new YouTubeProvider.YouTubeConfig(), new List<string>()
                {
                    "shanselman" // this channel provides all needed information and has not many vids
                });
            }
        }
    }
}