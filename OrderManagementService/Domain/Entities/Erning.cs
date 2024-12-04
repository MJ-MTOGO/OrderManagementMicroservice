namespace OrderManagementService.Domain.Entities
{
    public class Erning
    {
        public Guid ErningId { get; set; }
        public decimal MtogoErning { get; set; }
        public decimal RestaurantErning { get; set; }
        public decimal AgentErning { get; set; }    
    }   
}
