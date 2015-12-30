var videosController = (function() {

    var url,
        query,
        reload = function(queryParams) {
            location.href = url + '?' + $.param(queryParams, true);
        },
        executeAction = function(event, data) {
            event.preventDefault();

            reload($.extend(query, data));
        },
        init = function (settings) {

            url = settings.url;

            query = {
                Direction: settings.lastQuery.OrderBy.Direction,
                PropertyName: settings.lastQuery.OrderBy.PropertyName,
                Page: settings.lastQuery.Pagination.Page,
                ChannelName: settings.lastQuery.ChannelName,
                PublicationYear: settings.lastQuery.PublicationYear,
                Tags: settings.lastQuery.Tags,
            };

            $(settings.selectors.filters)
                .on("filter", executeAction)
                .on("clear", executeAction);
            $(settings.selectors.orderBy).on("changeOrder", executeAction);
            $(settings.selectors.pagination).on("changePage", executeAction);

    };

    return {
        init: init
    };

})();