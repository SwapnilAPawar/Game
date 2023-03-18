using Game.Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Game.Common.MongoDB
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            //to store Guid as string in mongo db.
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            //to store DatetimeOffset as string in mongo db.
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            //to get service settings from app settings

            //construct mongodb client
            //singleton to have single instance across entire microservice
            //this be used where ever we have injected IMongoDatabase in constructor
            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var serviceSettings = configuration!.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var mongoDbSettings = configuration!.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings!.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings!.ServiceName);
            });
            return services;
        }

        public static IServiceCollection AddRepository<T>(this IServiceCollection services, string collectionName)
        where T : IEntity
        {
            services.AddSingleton<IRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<IMongoDatabase>();
                if (database != null)
                {
                    return new MongoRepository<T>(database, collectionName);
                }
                else
                {
                    throw new Exception("Could not create database");
                }
            });
            return services;
        }
    }
}