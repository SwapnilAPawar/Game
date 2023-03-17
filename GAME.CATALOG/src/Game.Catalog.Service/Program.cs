using Game.Catalog.Service.Repositories;
using Game.Catalog.Service.Settings;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//to store Guid as string in mongo db.
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
//to store DatetimeOffset as string in mongo db.
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

//to get service settings from app settings
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

//construct mongodb client
//singleton to have single instance across entire microservice
//this be used where ever we have injected IMongoDatabase in constructor
builder.Services.AddSingleton(serviceProvider =>
{
    var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
    var mongoClient = new MongoClient(mongoDbSettings!.ConnectionString);
    return mongoClient.GetDatabase(serviceSettings!.ServiceName);
});

//Register dependencies
builder.Services.AddSingleton<IItemRepository, ItemRepository>();

builder.Services.AddControllers(options =>
{
    //to stop .net core from removing async suffix at runtime.
    options.SuppressAsyncSuffixInActionNames = false;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
