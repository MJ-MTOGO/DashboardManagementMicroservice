using DashboardManagementService.Application.Ports;
using DashboardManagementService.Application.Services;
using DashboardManagementService.Infrastructure.Publishers;
using DashboardManagementService.Infrastructure.Subscribers;
using DashboardManagementService.Infrastructure.WebSocketManagement;

namespace DashboardManagementService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register IMessageBus with GooglePubSubMessageBus
            builder.Services.AddSingleton<IMessageBus, GooglePubSubMessageBus>(sp =>
                new GooglePubSubMessageBus(builder.Configuration["GoogleCloud:ProjectId"]));

            // Register WebSocketManager
            builder.Services.AddSingleton<DashboardWebSocketManager>();

            // Register subscription handlers
            builder.Services.AddScoped<DashboardSubscriptionHandler>();

            // Register the SubscriptionManager
            builder.Services.AddSingleton<SubscriptionManager>();

            // Add services to the container.
            builder.Services.AddControllers();

            // Configure Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register HttpClient and DashboardService
            builder.Services.AddHttpClient<DashboardService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            // Add WebSocket Middleware
            app.UseWebSockets();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var webSocketManager = context.RequestServices.GetRequiredService<DashboardWebSocketManager>();
                        await webSocketManager.HandleWebSocketConnectionAsync(webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400; // Bad Request
                    }
                }
                else
                {
                    await next();
                }
            });
            app.MapControllers();

            // Start the subscriptions
            var subscriptionManager = app.Services.GetRequiredService<SubscriptionManager>();
            await subscriptionManager.StartSubscriptionsAsync();

            // Run the application
            app.Run();
        }
    }
}
