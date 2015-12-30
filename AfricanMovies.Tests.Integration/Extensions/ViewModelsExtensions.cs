using System;
using System.Collections.Generic;
using System.Linq;
using AfricanMovies.Backend.DataSources.DTO;
using Should;
using VideoViewModel = AfricanMovies.Backend.ViewModels.VideoViewModel;

namespace AfricanMovies.Tests.Integration.Extensions
{
    public static class ViewModelsExtensions
    {
        public static void ShouldBeFilledCorrectly(this List<ChannelDTO> conferences)
        {
            conferences.ShouldNotBeEmpty();
            conferences.ForEach(ShouldBeFilledCorrectly);
        }

        public static void ShouldBeFilledCorrectly(this ChannelDTO conference)
        {
            conference.Id.ShouldNotBeEmpty();
            conference.Name.ShouldNotBeEmpty();
            conference.Description.ShouldNotBeNull();
            conference.Link.ShouldNotBeEmpty();
            
            // Filter videos
            Func<VideoViewModel, bool> filter = 
                v => v.Dislikes > 0 
                    && v.Likes > 0 
                    && v.Views > 0
                    && string.IsNullOrEmpty(v.Description) == false;

            var videosToCheck = conference.Videos
                .Where(filter)
                .ToList();
            
            videosToCheck.ShouldNotBeEmpty();
            videosToCheck.ForEach(ShouldBeFilledCorrectly);
        }

        public static void ShouldBeFilledCorrectly(this VideoViewModel video)
        {
            video.Id.ShouldNotBeEmpty();
            video.Title.ShouldNotBeEmpty();
            video.Url.ShouldNotBeEmpty();
            video.PublicationDate.ShouldNotEqual(new DateTime());
            video.Description.ShouldNotBeEmpty();
            video.Views.ShouldBeGreaterThan(0);
            video.Likes.ShouldBeGreaterThan(0);
            video.Dislikes.ShouldBeGreaterThan(0);
            video.Tags.ShouldNotBeEmpty();
        }
    }
}