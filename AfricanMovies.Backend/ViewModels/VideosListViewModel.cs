using System.Collections.Generic;
using AfricanMovies.Backend.Queries;

namespace AfricanMovies.Backend.ViewModels
{
    public class VideosListViewModel
    {
        public List<VideoViewModel> Videos { get; set; }
        public VideosFiltersViewModel Filters { get; set; }
        public FindVideosQuery Query { get; set; }
    }
}