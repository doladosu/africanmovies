using System.Collections.Generic;
using AfricanMovies.Backend.ViewModels;

namespace AfricanMovies.Backend.DataSources.DTO
{
    public class ChannelDTO
    {
        public ChannelDTO()
        {
            Videos = new List<VideoViewModel>();
        }

        public string Name { get; set; }

        public List<VideoViewModel> Videos { get; private set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Id { get; set; }
    }
}