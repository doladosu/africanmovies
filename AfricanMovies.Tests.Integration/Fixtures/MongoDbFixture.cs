using System;
using MongoDB.Driver;

namespace AfricanMovies.Tests.Integration.Fixtures
{
    public class MongoDbFixture : IDisposable
    {
        private readonly MongoServer server;

        private const string dbName = "developersstuff_tests";

        public MongoDbFixture()
        {
            var client = new MongoClient("mongodb://127.0.0.1:27017/");

            server = client.GetServer();

            Reset();
        }

        public MongoDatabase Db { get; private set; }

        public void Reset()
        {
            server.DropDatabase(dbName);

            Db = server.GetDatabase(dbName);
        }

        public void Dispose()
        {
            server.DropDatabase(dbName);
        }
    }
}