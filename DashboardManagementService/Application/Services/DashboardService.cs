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
            // Fetch Earnings
            var earnings = await FetchEarningsAsync("http://localhost:5194/api/earnings");

            // Fetch Orders
            var orders = await FetchOrdersAsync("http://localhost:5194/api/orders");

            // Aggregate data for DashboardDto
            var dashboardDto = new DashboardDto
            {
                TotalMtogoEarning = earnings.Sum(e => e.MtogoEarning),
                TotalRestaurantEarning = earnings.Sum(e => e.RestaurantEarning),
                TotalAgentEarning = earnings.Sum(e => e.AgentEarning),
                TotalPendingOrder = orders.Count(o => o.OrderStatus == "Pending"),
                TotalReadyToPickupOrder = orders.Count(o => o.OrderStatus == "ReadyToPickup"),
                TotalDeliveredOrder = orders.Count(o => o.OrderStatus == "Delivered")
            };

            return dashboardDto;
        }


        private async Task<List<EarningsDto>> FetchEarningsAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<EarningsDto>>(content);
        }

        private async Task<List<OrderDto>> FetchOrdersAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<OrderDto>>(content);
        }


        public async Task<DashboardDto> UpdateAndGetDashboardDataAsyncSubscribe(EarningsDto earningsDto, OrderDto orderDto)
        {
            // Fetch existing data from the APIs
            var earnings = await FetchEarningsAsync("http://localhost:5194/api/earnings");
            var orders = await FetchOrdersAsync("http://localhost:5194/api/orders");

            // Update earnings list
            if (earningsDto != null)
            {
                var existingEarnings = earnings.FirstOrDefault(e => e.OrderId == earningsDto.OrderId);
                if (existingEarnings == null)
                {
                    // If it doesn't exist, add it
                    earnings.Add(earningsDto);                  
                }
             
            }

            // Update orders list
            if (orderDto != null)
            {
                var existingOrder = orders.FirstOrDefault(o => o.OrderId == orderDto.OrderId);
                if (existingOrder == null)
                {
                    // If it doesn't exist, add it
                    orders.Add(orderDto);
                }
               
            }

            // Aggregate data for DashboardDto
            var dashboardDto = new DashboardDto
            {
                TotalMtogoEarning = earnings.Sum(e => e.MtogoEarning),
                TotalRestaurantEarning = earnings.Sum(e => e.RestaurantEarning),
                TotalAgentEarning = earnings.Sum(e => e.AgentEarning),
                TotalPendingOrder = orders.Count(o => o.OrderStatus == "Pending"),
                TotalReadyToPickupOrder = orders.Count(o => o.OrderStatus == "ReadyToPickup"),
                TotalDeliveredOrder = orders.Count(o => o.OrderStatus == "Delivered")
            };

          
            return dashboardDto;
        }


    }
}
