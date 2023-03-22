using Game.Catalog.Service.Entities;
using Game.Common.MassTransit;
using Game.Common.MongoDB;
using Game.Common.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
//Register dependencies
builder.Services.AddMongo()
                .AddRepository<Item>("item")
                .AddMassTransitWithRabbitMQ();


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
