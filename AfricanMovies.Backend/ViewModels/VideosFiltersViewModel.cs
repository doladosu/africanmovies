using System.Collections.Generic;

namespace AfricanMovies.Backend.ViewModels
{
    public class VideosFiltersViewModel
    {
        public class SelectedFiltersViewModel
        {
            public string ChannelName { get; set; }
            public int? PublicationYear { get; set; }
            public IEnumerable<string> Tags { get; set; }
        }

        public VideosFiltersViewModel()
        {
            Channels = new List<string>();
            Tags = new List<string>();
            PublicationYears = new List<int>();
        }
        public List<string> Channels { get; set; }
        public List<string> Tags { get; set; }
        public List<int> PublicationYears { get; set; }
        public SelectedFiltersViewModel Current { get; set; }
    }
}