using System.Collections.Generic;
using AfricanMovies.Backend;
using AfricanMovies.Tests.Integration.Extensions;
using AfricanMovies.Tests.Integration.Fixtures;
using MongoDB.Driver;
using Ploeh.AutoFixture;
using Xunit;

namespace AfricanMovies.Tests.Integration.AutoData
{
    public abstract class WithPopulatedMongoDb : IUseFixture<MongoDbFixture>
    {
        protected MongoDatabase db;

        protected readonly IEnumerable<Video> videos;
        protected readonly IEnumerable<Channel> channels;

        protected WithPopulatedMongoDb()
        {
            var fixture = new Fixture()
                .Customize(new ModelAutoData.Customization());

            videos = fixture.Resolve<IEnumerable<Video>>();
            channels = fixture.Resolve<IEnumerable<Channel>>();
        }

        public void SetFixture(MongoDbFixture data)
        {
            db = data.Db;
            data.Reset();

            db.GetCollection("Videos").InsertBatch(videos);
            db.GetCollection("Channels").InsertBatch(channels);
        }
    }
}