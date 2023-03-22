using Game.Common.MassTransit;
using Game.Common.MongoDB;
using Game.Inventory.Service.Clients;
using Game.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddMongo()
                        .AddRepository<InventoryItem>("inventoryitems")
                        .AddRepository<CatalogItem>("catalogitems")
                        .AddMassTransitWithRabbitMQ();

        AddCatalogClient(builder);

        builder.Services.AddControllers();
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
    }

    static void AddCatalogClient(WebApplicationBuilder builder)
    {
        var jitterer = new Random();

        //httpClient dependency registration
        //Or - if failed due to timeout issues then go ahead and try again
        //AddTransientHttpErrorPolicy - WaitAndRetryAsync
        //retry for 5 times with exponential delay time for 2 raised to retry attempt count + random value as fail time cannot be exact 2,4,8 etc.
        //Also it will avoid overwhelming/overloading of dependent services
        //onRetry(optional) parameter to log retry messages. can be removed in prod app as it creates new service provider is created.
        //AddTransientHttpErrorPolicy - AdvancedCircuitBreakerAsync
        //if 3 consecutive errors occur, the circuit is cut for 30 seconds:
        //AddPolicyHandler - 
        //wait for 1 sec for any http response. else return message.
        //Here sequence is important
        builder.Services.AddHttpClient<CatalogClient>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:5001");
        })
        .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
            5,
            retryAttempts => TimeSpan.FromSeconds(Math.Pow(2, retryAttempts)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
            onRetry: (outcome, timeSpan, retryAttempt) =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();
                serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Delay for {timeSpan.TotalSeconds} seconds, then making retry {retryAttempt}.");
            }
        ))
        .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
            3,
            TimeSpan.FromSeconds(30),
            onBreak: (outcome, timeSpan) =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();
                serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Opening the circuit for {timeSpan.TotalSeconds} seconds...");
            },
            onReset: () =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();
                serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Closing the circuit...");
            }
        ))
        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
    }
}