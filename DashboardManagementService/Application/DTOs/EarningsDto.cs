namespace DashboardManagementService.Application.DTOs
{
    public class EarningsDto
    {
        public Guid OrderId { get; set; }   
        public decimal MtogoEarning { get; set; }
        public decimal RestaurantEarning { get; set; }  
        public decimal AgentEarning { get; set; }
    }
}
