using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DashboardManagementService.Application.DTOs;

namespace DashboardManagementService.Application.Services
{
    public class DashboardService
    {
        private readonly HttpClient _httpClient;

        public DashboardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            // Fetch data
            var earnings = await FetchEarningsAsync("http://localhost:5194/api/earnings") ?? new List<EarningsDto>();
            var orders = await FetchOrdersAsync("http://localhost:5194/api/orders") ?? new List<OrderDto>();

          
            // Log fetched data
            Console.WriteLine("Fetched Orders: " + JsonSerializer.Serialize(orders));
            Console.WriteLine("Fetched Earnings: " + JsonSerializer.Serialize(earnings));

            // Aggregate data
            return AggregateDashboardData(earnings, orders);
        }

        private async Task<List<EarningsDto>> FetchEarningsAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<EarningsDto>>(content, options);
        }

        private async Task<List<OrderDto>> FetchOrdersAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<OrderDto>>(content, options);
        }

        public async Task<DashboardDto> UpdateAndGetDashboardDataAsyncSubscribe(EarningsDto earningsDto, OrderDto orderDto)
        {
            // Fetch existing data
            var earnings = await FetchEarningsAsync("http://localhost:5194/api/earnings") ?? new List<EarningsDto>();
            var orders = await FetchOrdersAsync("http://localhost:5194/api/orders") ?? new List<OrderDto>();

            // Update earnings list
            if (earningsDto != null)
            {
                if (!earnings.Any(e => e.OrderId == earningsDto.OrderId))
                {
                    earnings.Add(earningsDto);
                }
            }

            // Update orders list
            if (orderDto != null)
            {
                if (!orders.Any(o => o.OrderId == orderDto.OrderId))
                {
                    orders.Add(orderDto);
                }
            }

            // Aggregate updated data
            return AggregateDashboardData(earnings, orders);
        }

        private DashboardDto AggregateDashboardData(List<EarningsDto> earnings, List<OrderDto> orders)
        {
            return new DashboardDto
            {
                TotalMtogoEarning = earnings.Sum(e => e.MtogoEarning),
                TotalRestaurantEarning = earnings.Sum(e => e.RestaurantEarning),
                TotalAgentEarning = earnings.Sum(e => e.AgentEarning),
                TotalPendingOrder = orders.Count(o => string.Equals(o.OrderStatus, "Pending", StringComparison.OrdinalIgnoreCase)),
                TotalReadyToPickupOrder = orders.Count(o => string.Equals(o.OrderStatus, "ReadyToPickup", StringComparison.OrdinalIgnoreCase)),
                TotalDeliveredOrder = orders.Count(o => string.Equals(o.OrderStatus, "Delivered", StringComparison.OrdinalIgnoreCase))
            };
        }
    }
}
