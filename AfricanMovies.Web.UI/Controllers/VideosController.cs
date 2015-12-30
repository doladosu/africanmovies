using System.Configuration;
using System.Web.Mvc;
using AfricanMovies.Backend.Infrastructure;
using AfricanMovies.Backend.Providers;
using AfricanMovies.Backend.Queries;
using AfricanMovies.Backend.ViewModels;

namespace AfricanMovies.Web.UI.Controllers
{
    public class VideosController : Controller
    {
        private readonly VideosLibraryQueryHandlers hander;
        private readonly VideosFiltersProvider filtersProvider;

        public VideosController()
        {
            var db = DatabaseConnectionFactory
                .CreateDatabaseConnection(ConfigurationManager.AppSettings["CurrentConnString"], ConfigurationManager.AppSettings["DevStuffDbName"]);

            hander = new VideosLibraryQueryHandlers(db);
            filtersProvider = new VideosFiltersProvider(db);
        }

        public ActionResult Index(FlattenedVideosQuery @params)
        {
            var query = new FindVideosQuery()
            {
                ChannelName = @params.ChannelName,
                PublicationYear = @params.PublicationYear,
                Tags = @params.Tags
            };

            query.Pagination.Page = @params.Page ?? 1;
            query.Pagination.PerPage = 12;

            var filters = filtersProvider.TakeAvailableFilters();
            var videos = hander.Handle(query);

            filters.Current = new VideosFiltersViewModel.SelectedFiltersViewModel
            {
                ChannelName = query.ChannelName,
                PublicationYear = query.PublicationYear,
                Tags = query.Tags
            };

            return View("Index", null, new VideosListViewModel
            {
                Query = query,
                Videos = videos,
                Filters = filters
            });
        }
    }

}