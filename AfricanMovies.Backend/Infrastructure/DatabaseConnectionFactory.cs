using System.Configuration;
using MongoDB.Driver;

namespace AfricanMovies.Backend.Infrastructure
{
    public static class DatabaseConnectionFactory
    {
        public static MongoDatabase CreateDatabaseConnection(string connectionStringName, string dbName)
        {
            var client = new MongoClient(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString);

            return client.GetServer().GetDatabase(dbName);
        }
    }
}
