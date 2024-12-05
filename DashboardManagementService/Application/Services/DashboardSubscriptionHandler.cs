using DashboardManagementService.Application.DTOs;
using DashboardManagementService.Infrastructure.WebSocketManagement;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DashboardManagementService.Application.Services
{
    public class DashboardSubscriptionHandler
    {
        private readonly DashboardService _dashboardService;
        private readonly DashboardWebSocketManager _webSocketManager;

        public DashboardSubscriptionHandler(DashboardService dashboardService, DashboardWebSocketManager dashboardWebSocketManager)
        {
            _dashboardService = dashboardService;
            _webSocketManager = dashboardWebSocketManager;
        }

        public async Task EarningHandleMessageAsync(string message) 
        {
            var earningsDto = JsonConvert.DeserializeObject<EarningsDto>(message);
            var dashboardDto = await _dashboardService.UpdateAndGetDashboardDataAsyncSubscribe(earningsDto, null);


            var dashboardDtoJson = JsonConvert.SerializeObject(dashboardDto);
            await _webSocketManager.BroadcastMessageAsync(dashboardDtoJson);
        }
        public async Task OrderHandleMessageAsync(string message)   
        {
            var orderDto = JsonConvert.DeserializeObject<OrderDto>(message);
            var dashboardDto =  await _dashboardService.UpdateAndGetDashboardDataAsyncSubscribe(null, orderDto);

            var dashboardDtoJson = JsonConvert.SerializeObject(dashboardDto);
            await _webSocketManager.BroadcastMessageAsync(dashboardDtoJson);
        }
    }
}
