using DashboardManagementService.Application.Ports;
using DashboardManagementService.Application.Services;

namespace DashboardManagementService.Infrastructure.Subscribers
{
    public class SubscriptionManager
    {
        private readonly IMessageBus _messageBus;
        private readonly IServiceProvider _serviceProvider;

        public SubscriptionManager(IMessageBus messageBus, IServiceProvider serviceProvider )
        {
            _messageBus = messageBus;
            _serviceProvider = serviceProvider;
        }

        public async Task StartSubscriptionsAsync()
        {
            _ = Task.Run(async () =>
            {
                var orderCreatedHandler = _serviceProvider.GetRequiredService<DashboardSubscriptionHandler>();
                Console.WriteLine("***************************** orderCreatedHandler");
                await _messageBus.SubscribeAsync("OrderCreated-sub3", orderCreatedHandler.OrderHandleMessageAsync);
            });

            _ = Task.Run(async () =>
            {
                var readyToPickupHandler = _serviceProvider.GetRequiredService<DashboardSubscriptionHandler>();
                await _messageBus.SubscribeAsync("ready-to-pickup-sub2", readyToPickupHandler.OrderHandleMessageAsync);
            });

            _ = Task.Run(async () =>
            {
                var calculatedEarningsHandler = _serviceProvider.GetRequiredService<DashboardSubscriptionHandler>();
                await _messageBus.SubscribeAsync("calculated-earnings-sub1", calculatedEarningsHandler.EarningHandleMessageAsync);
            });
        }
    }
}
