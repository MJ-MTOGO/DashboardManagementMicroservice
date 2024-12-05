namespace DashboardManagementService.Application.DTOs
{
    public class DashboardDto
    {
        public int TotalPendingOrder { get; set; }
        public int TotalReadyToPickupOrder { get; set; }
        public int TotalDeliveredOrder { get; set; }
        public decimal TotalMtogoEarning { get; set; }
        public decimal TotalRestaurantEarning { get; set; }
        public decimal TotalAgentEarning { get; set; }


    }   
}
