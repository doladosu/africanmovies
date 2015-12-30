using System;
using System.Collections.Generic;
using System.Linq;
using AfricanMovies.Backend;
using AfricanMovies.Backend.Queries;
using AfricanMovies.Backend.ViewModels;
using AfricanMovies.Tests.Integration.AutoData;
using AfricanMovies.Tests.Integration.Extensions;
using MongoDB.Driver.Builders;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Should;
using Xunit;
using Xunit.Extensions;

namespace AfricanMovies.Tests.Integration
{
    public class VideosQueryHandlerTests : WithPopulatedMongoDb
    {
        public VideosLibraryQueryHandlers create_handler()
        {
            return new VideosLibraryQueryHandlers(db);
        }

        private static int DefaultNumberOfRecordsPerPage 
        {
            get { return new PaginationSettings().PerPage; }
        }

        [Fact]
        public void Query__no_filters_given__returns_corresponding_data()
        {
            var actualResult = create_handler().Handle(new FindVideosQuery());

            actualResult.ShouldEqual(videos.TransformToExpectedViewModel(channels, DefaultNumberOfRecordsPerPage));
        }

        [Fact]
        public void Query__query_object_not_given__uses_default_settings()
        {
            var actualResult = create_handler().Handle(null);

            actualResult.ShouldEqual(videos.TransformToExpectedViewModel(channels, DefaultNumberOfRecordsPerPage));
        }

        [Fact]
        public void Query__name_filter_given__returns_corresponding_data()
        {
            var channelNameToQuery = channels.Last().Name;

            var actualResult = create_handler().Handle(new FindVideosQuery()
            {
                ChannelName = channelNameToQuery
            });

            Func<VideoViewModel, bool> matchesExpectedFilter
                = video => video.ChannelInfo.Name == channelNameToQuery;

            actualResult
                .All(matchesExpectedFilter)
                .ShouldBeTrue();
        }

        [Fact]
        public void Query__publication_year_filter_given__returns_corresponding_data()
        {
            var publicationYearToQuery = videos.First().PublicationDate.Year;

            var actualResult = create_handler().Handle(new FindVideosQuery()
            {
                PublicationYear = publicationYearToQuery
            });

            Func<VideoViewModel, bool> matchesExpectedFilter 
                = video => video.PublicationDate.Year == publicationYearToQuery;

            actualResult
                .All(matchesExpectedFilter)
                .ShouldBeTrue();
        }

        [Fact]
        public void Query__publication_year_and_name_filters_given__returns_corresponding_data()
        {
            var randomVideo = videos.First();
            var relatedChannel = channels.First(c => c.Id == randomVideo.ChannelId);

            var publicationYearFilterValue = randomVideo.PublicationDate.Year;
            var channelNameFilterValue = relatedChannel.Name;

            var actualResult = create_handler().Handle(new FindVideosQuery()
            {
                PublicationYear = publicationYearFilterValue,
                ChannelName = channelNameFilterValue
            });

            Func<VideoViewModel, bool> matchesExpectedFilters = video =>
            {
                var yearMatches = video.PublicationDate.Year == publicationYearFilterValue;
                var nameMatches = video.ChannelInfo.Name == channelNameFilterValue;

                return yearMatches && nameMatches;
            };

            actualResult
                .All(matchesExpectedFilters)
                .ShouldBeTrue();
        }

        [Fact]
        public void Query__tags_filter_given__returns_matching_results()
        {
            var tags = videos
                .First()
                .Tags
                .ToList();

            tags.AddRange(videos.Last().Tags);

            var expectedTags = tags.Distinct().Select(t => t.Name);

            var actualResult = create_handler().Handle(new FindVideosQuery()
            {
                Tags = expectedTags
            });

            actualResult.ForEach(v =>
            {
                v.Tags
                    .Any(t => expectedTags.Contains(t))
                    .ShouldBeTrue("Video does not contain any of expected tags.");
            });
        }

        [Fact]
        public void Query__tags_filter_is_not_case_sensitive__returns_matching_results()
        {
            var someVideo = videos.First();
            var tagForTests = new Fixture()
                .Create<string>()
                .ToLower();

            var newTags = new []
            {
                new Tag()
                {
                    Name = tagForTests.ToUpper()
                }
            };

            var query = Query<Video>.EQ(v => v.Id, someVideo.Id);
            var update = Update<Video>.Set(v => v.Tags, newTags);

            db.GetCollection<Video>("Videos").Update(query, update);

            var actualResult = create_handler().Handle(new FindVideosQuery()
            {
                Tags = new []{ tagForTests }
            });

            actualResult
                .Any(v => v.Id == someVideo.Id)
                .ShouldBeTrue();
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineAutoData(2, 2)]
        [InlineAutoData(3, 1)]
        public void Query__page_and_per_page_filters_set__returns_appropriate_number_of_records(int page, int perPage)
        {
            var query = new FindVideosQuery()
            {
                Pagination = new PaginationSettings()
                {
                    Page = page,
                    PerPage = perPage
                }
            };

            var actualResult = create_handler().Handle(query);

            var expectedPages = (int)Math.Ceiling((decimal)videos.Count() / perPage);

            var expectedVideos = videos
                .Skip((page - 1)*perPage)
                .Take(perPage);

            actualResult.ShouldEqual(expectedVideos.TransformToExpectedViewModel(channels, DefaultNumberOfRecordsPerPage));
            query.Pagination.NumberOfPages.ShouldEqual(expectedPages);
        }

        [Fact]
        public void Query__page_and_per_page_not_given__uses_default_settings()
        {
            const int defaultPage = 1;
            
            var query = new FindVideosQuery();

            var actualResult = create_handler().Handle(query);

            var expectedPages = (int)Math.Ceiling((decimal)videos.Count()/DefaultNumberOfRecordsPerPage);

            var expectedVideos = videos
                .Skip((defaultPage - 1) * DefaultNumberOfRecordsPerPage)
                .Take(DefaultNumberOfRecordsPerPage);

            actualResult.ShouldEqual(expectedVideos.TransformToExpectedViewModel(channels, DefaultNumberOfRecordsPerPage));
            query.Pagination.NumberOfPages.ShouldEqual(expectedPages);
        }

        [Fact]
        public void Query__order_not_set__uses_default_order()
        {
            const int pageSize = 4;

            var query = new FindVideosQuery();

            query.Pagination.PerPage = pageSize;

            var actualResult = create_handler().Handle(query);

            var expectedOrder = videos
                .Take(pageSize)
                .TransformToExpectedViewModel(channels, videos.Count())
                .OrderByDescending(v => v.PublicationDate)
                .Select(v => v.Id)
                .ToArray();

            actualResult
                .Select(v => v.Id)
                .ToArray()
                .ShouldEqual(expectedOrder);
        }

        [Fact]
        public void Query__order_ascending_by_name__result_is_sorted()
        {
            var orderBy = new OrderSettings()
            {
                PropertyName = "Title",
                Direction = OrderDirectionEnum.Ascending
            };
            
            var expectedOrder = videos
                .OrderBy(v => v.Title)
                .TransformToExpectedViewModel(channels, DefaultNumberOfRecordsPerPage);

            execute_sort(orderBy, expectedOrder);
        }

        [Fact]
        public void Query__order_ascending_by_likes__result_is_sorted()
        {
            var orderBy = new OrderSettings()
                {
                    PropertyName = "Likes",
                    Direction = OrderDirectionEnum.Ascending
                };

            var expectedOrder = videos
                .TransformToExpectedViewModel(channels, DefaultNumberOfRecordsPerPage)
                .OrderBy(v => v.Likes);

            execute_sort(orderBy, expectedOrder);
        }

        [Fact]
        public void Query__order_ascending_by_channel_name__result_is_sorted()
        {
            var orderBy = new OrderSettings()
            {
                PropertyName = "ChannelInfo.Name",
                Direction = OrderDirectionEnum.Ascending
            };

            var expectedOrder = videos
                .TransformToExpectedViewModel(channels, DefaultNumberOfRecordsPerPage)
                .OrderBy(v => v.ChannelInfo.Name);

            execute_sort(orderBy, expectedOrder);
        }

        private void execute_sort(OrderSettings orderBy, IEnumerable<VideoViewModel> expectedOrder)
        {
            var query = new FindVideosQuery()
            {
                OrderBy = orderBy
            };

            var actualResult = create_handler().Handle(query);

            var expected = expectedOrder
                .Select(v => v.Id)
                .ToArray();
            
            actualResult
                .Select(v => v.Id)
                .ToArray()
                .ShouldEqual(expected);
        }
    }

    
}
