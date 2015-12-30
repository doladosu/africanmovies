using System;
using System.Collections.Generic;
using System.Linq;
using AfricanMovies.Backend.Infrastructure;
using AfricanMovies.Backend.ViewModels;

namespace AfricanMovies.Backend.Queries
{
    public class FlattenedVideosQuery
    {
        public string ChannelName { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public int? PublicationYear { get; set; }
        public int? Page { get; set; }
        public string PropertyName { get; set; }
        public OrderDirectionEnum Direction { get; set; }
    }

    public class FindVideosQuery : IQuery<List<VideoViewModel>>, IPagable, ISortable
    {
        private IEnumerable<string> tags;

        public FindVideosQuery()
        {
            Tags = new List<string>();
            Pagination = new PaginationSettings();

            OrderBy = new OrderSettings()
            {
                PropertyName = "PublicationDate",
                Direction = OrderDirectionEnum.Descending
            };
        }
        public string ChannelName { get; set; }
        public int? PublicationYear { get; set; }

        public IEnumerable<string> Tags {
            get { return tags; }
            set 
            {
                try
                {
                    tags = value
                        .Where(t => !string.IsNullOrEmpty(t))
                        .Select(t => t.ToLower())
                        .Distinct();
                }
                catch (ArgumentNullException)
                {
                    tags = null;
                }
            }
        }

        public PaginationSettings Pagination { get; set; }
        public OrderSettings OrderBy { get; set; }
    }
}