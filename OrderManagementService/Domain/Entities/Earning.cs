namespace OrderManagementService.Domain.Entities
{
    public class Earning
    {
        public Guid EarningId { get; set; }
        public Guid OrderId { get; set; }
        public decimal MtogoEarning { get; set; }
        public decimal RestaurantEarning { get; set; }   
        public decimal AgentEarning { get; set; }    
    }   
}
