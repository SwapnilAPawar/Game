using Game.Catalog.Service.Entities;
using Game.Common.MongoDB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Register dependencies
builder.Services.AddMongo().AddRepository<Item>("item");

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
