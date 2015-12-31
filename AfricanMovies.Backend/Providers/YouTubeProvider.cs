using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AfricanMovies.Backend.DataSources.DTO;
using AfricanMovies.Backend.ViewModels;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace AfricanMovies.Backend.Providers
{
    public class YouTubeProvider : IVideoProvider
    {
        private readonly YouTubeConfig config;
        private readonly IReadOnlyCollection<string> channels;

        public class YouTubeConfig
        {
            internal string AppKey
            {
                get { return "AIzaSyD8GjuH9jIXYnOVzHPwikhT-_co_ZJkXjU"; }
            }

            internal string ChannelUri(string channelId)
            {
                return string.Format("https://www.youtube.com/channel/{0}", channelId);
            }

            internal string VideoUri(string videoId)
            {
                return string.Format("https://www.youtube.com/watch?v={0}", videoId);
            }
        }

        private readonly YouTubeService service;

        public YouTubeProvider()
            : this(new YouTubeConfig(), new List<string>()
                                        {
                                            "Nollywood5ive",
                                            "TVNolly",
                                            "realnollymovies",
                                            "nollystar1,,NollywoodLove,mynaijalove,Movieandclipstv,Ikorotv,nollywoodbest",
                                            "NollywoodPicturestv",
                                            "NollywoodLove,,Movieandclipstv,Ikorotv,nollywoodbest",
                                            "mynaijalove",
                                            "Movieandclipstv",
                                            "Ikorotv",
                                            "nollywoodbest",
                                            "mynaijalove",
                                            "UCdUth5xByWogqrnjKmTAlIw", //Yoruba360
                                            "UC6soW1N4F9DgdCVhHoXy2pw", //MovieMore
                                            "geledetvhits",
                                            "Yorubahood",
                                            "UC83tx3zGcvQ_rXzsZigTnEA", //ApataTV+
                                            "OlekuTV",
                                            "NewNollywood",
                                            "yorubamagic",
                                            "Yorubac",
                                            "Yoruba5star",
                                            "UCAb8z0bY19q-K_8-kY3S8nw",
                                            "UC_LDb23YlsAbpuUdg7xqjDA",
                                            "UCmLRPOm_1rvS0FC1k0pgWwA",
                                            "UCmLRPOm_1rvS0FC1k0pgWwA",
                                            "UCdUth5xByWogqrnjKmTAlIw",
                                            "UC6soW1N4F9DgdCVhHoXy2pw",
                                            "UC83tx3zGcvQ_rXzsZigTnEA"
                                        })
        {
        }

        public YouTubeProvider(YouTubeConfig config, IReadOnlyCollection<string> channels)
        {
            this.config = config;
            this.channels = channels;

            service = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = config.AppKey
            });
        }


        public async Task<List<ChannelDTO>> ChannelsData()
        {
            var results = new List<ChannelDTO>();

            foreach (var channel in channels)
            {
                var conference = new ChannelDTO();

                var info = await ChannelInfo(channel);

                if (info == null)
                    continue;

                conference.Id = info.Id;
                conference.Name = info.Snippet.Title;
                conference.Description = info.Snippet.Description;
                conference.Link = config.ChannelUri(conference.Id);

                conference.Videos.AddRange(await ChannelVideos(conference.Id));

                results.Add(conference);
            }
            return results;
        }

        private async Task<List<VideoViewModel>>  ChannelVideos(string channelId)
        {
            var results = new List<VideoViewModel>();

            var s = service.Search.List("snippet");

            s.ChannelId = channelId;
            s.Type = "video";
            s.MaxResults = 50;

            string nextPage = null;

            do
            {
                s.PageToken = nextPage;

                var videos = await s.ExecuteAsync();

                foreach (var video in videos.Items)
                {
                    if (video.Id.Kind != "youtube#video")
                        continue;

                    results.Add(new VideoViewModel()
                    {
                        Title = video.Snippet.Title,
                        Url = config.VideoUri(video.Id.VideoId),
                        Id = video.Id.VideoId,
                        PublicationDate = video.Snippet.PublishedAt ?? new DateTime(),
                        Description = video.Snippet.Description
                    });
                }

                nextPage = videos.NextPageToken;
            } while (string.IsNullOrEmpty(nextPage) == false);

            await FillStatistics(results);

            return results;
        }

        private async Task FillStatistics(List<VideoViewModel> results)
        {
            var videosIds = results.Select(c => c.Id).ToArray();

            var request = service.Videos.List("statistics");

            const int perPage = 50;

            request.MaxResults = perPage;

            var skip = 0;
            do
            {
                request.Id = string.Join(",", videosIds.Skip(skip).Take(perPage));

                var response = await request.ExecuteAsync();

                foreach (var statistics in response.Items)
                {
                    var vid = results.First(v => v.Id == statistics.Id);

                    vid.Likes = Convert.ToInt32(statistics.Statistics.LikeCount);
                    vid.Dislikes = Convert.ToInt32(statistics.Statistics.DislikeCount);
                    vid.Views = Convert.ToInt32(statistics.Statistics.ViewCount);
                }

                skip += perPage;

            } while (skip < videosIds.Length);
        }

        private async Task<Google.Apis.YouTube.v3.Data.Channel> ChannelInfo(string channel)
        {
            var request = service.Channels.List("snippet");

            request.ForUsername = channel;

            var response = await request.ExecuteAsync();

            if (response.PageInfo.TotalResults != 0) 
                return response.Items.FirstOrDefault();

            request.ForUsername = null;
            request.Id = channel;

            response = await request.ExecuteAsync();

            return response.Items.FirstOrDefault();
        }
    }
}