using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AfricanMovies.Backend.DataSources.DTO;
using Newtonsoft.Json;
using VideoViewModel = AfricanMovies.Backend.ViewModels.VideoViewModel;

namespace AfricanMovies.Backend.Providers
{
    public class VimeoProvider : IVideoProvider
    {
        public class VimeoConfig
        {
            internal Uri ApiUri
            {
                get { return new Uri("https://api.vimeo.com"); }
            }

            internal string Version
            {
                get { return "3.2"; }
            }

            internal string AcceptHeader
            {
                get { return string.Format("application/vnd.vimeo.*+json;version={0}", Version); }
            }

            internal AuthenticationHeaderValue AuthorizationHeader
            {
                get
                {
                    return new AuthenticationHeaderValue("bearer", "75352f15477eb0a2b53d4dbef76a4da6");
                }
            }

            internal Uri ChannelsUri(string account)
            {
                return new Uri(ApiUri, string.Format("users/{0}/channels", account));
            }

            internal Uri ChannelInfoUri(string account)
            {
                return new Uri(ApiUri, string.Format("users/{0}", account));
            }

            internal Uri VideosUri(string account, int page = 1)
            {
                return new Uri(ApiUri, string.Format("users/{0}/videos?page={1}&per_page=50", account, page));
            }
        }

        private readonly VimeoConfig config;

        readonly IReadOnlyCollection<string> accounts;

        private readonly Func<VimeoConfig, HttpClient> webClientFactory = cfg =>
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = cfg.AuthorizationHeader;

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", cfg.AcceptHeader);

            return client;
        };

        public VimeoProvider()
            : this(new VimeoConfig(), new ReadOnlyCollection<string>(new List<string>(1)
            {
                "user14410096", //tretton37
                "ndcoslo"
            }))
        {
            
        }

        public VimeoProvider(VimeoConfig config, IReadOnlyCollection<string> accounts)
        {
            this.config = config;
            this.accounts = accounts;
        }

        public async Task<List<ChannelDTO>> ChannelsData()
        {
            using (var client = webClientFactory(config))
            {
                var conferences = new List<ChannelDTO>();

                foreach (var account in accounts)
                {
                    var conference = new ChannelDTO();

                    var info = await GetChannelInfo(client, account);

                    conference.Id = info.Id;
                    conference.Name = info.Name;
                    conference.Description = info.Desc;
                    conference.Link = info.Link;

                    conference.Videos.AddRange(await GetAllVideos(client, account));

                    conferences.Add(conference);
                }

                return conferences;
            }
        }

        private async Task<dynamic> GetChannelInfo(HttpClient client, string account)
        {
            try
            {
                var response = await client.GetAsync(config.ChannelInfoUri(account));

                dynamic info = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

                string uri = info.uri;
                return new
                {
                    Id = uri.Split('/').Last(),
                    Name = info.name,
                    Link = info.link,
                    Desc = info.bio,
                };
            }
            catch (Exception e)
            {
                // TODO: Add logger here
                throw;
            }
        }

        private async Task<List<VideoViewModel>> GetAllVideos(HttpClient client, string account)
        {
            try
            {
                var results = new List<VideoViewModel>(500);

                var nextPage = string.Empty;
                do
                {
                    var uri = string.IsNullOrEmpty(nextPage)
                        ? config.VideosUri(account)
                        : new Uri(config.ApiUri, nextPage);

                    var response = await client.GetAsync(uri);

                    nextPage = await ProcessResult(response.Content, results);

                } while (nextPage != null);

                return results;
            }
            catch (Exception e)
            {
                //TODO: Add logger here
                throw;
            }
        } 

        private static async Task<string> ProcessResult(HttpContent content, ICollection<VideoViewModel> videos)
        {
            try
            {
                if (videos == null) throw new ArgumentNullException("videos");

                dynamic result = JsonConvert.DeserializeObject(await content.ReadAsStringAsync());

                //TODO: Add logger here
                if (string.IsNullOrEmpty(result.error) == false)
                    return null;

                foreach (var video in result.data)
                {
                    string name = video.name;
                    string uri = video.uri;
                    string pubDate = video.created_time;

                    var vm = new VideoViewModel
                    {
                        Url = video.link,
                        Title = name,
                        Description = video.description,
                        PublicationDate = Convert.ToDateTime(pubDate),
                        Likes = video.metadata.connections.likes.total,
                        Views = video.stats.plays,
                        Id = uri.Split('/').Last(),
                    };

                    foreach (var tag in video.tags)
                    {
                        string value = tag.tag;
                        vm.Tags.Add(value);
                    }

                    videos.Add(vm);
                }

                return result.paging.next;
            }
            catch (Exception e)
            {
                //TODO: Add logger here
                throw;
            }
        }

    }
}