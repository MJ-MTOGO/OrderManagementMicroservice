using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderManagementService.Domain.Entities
{
    public class Order
    {
        public Guid OrderId { get; private set; }
        public Guid CustomerId { get; private set; }
        public Guid RestaurantId { get; private set; }
        public DateTime OrderedTime { get; private set; }
        public decimal TotalPrice => _orderItems.Sum(item => item.Price);
        public string OrderStatus { get; private set; } // "Pending","ReadyToPickup", "Delivered"

        private readonly List<OrderItem> _orderItems = new List<OrderItem>();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        private Order() { } // For EF Core or serialization

        public Order(Guid customerId, Guid restaurantId, List<OrderItem> orderItems)
        {
            if (orderItems == null || !orderItems.Any())
                throw new ArgumentException("Order must contain at least one order item.");

            OrderId = Guid.NewGuid();
            CustomerId = customerId;
            RestaurantId = restaurantId;
            OrderedTime = DateTime.UtcNow;
            OrderStatus = "Pending";

            _orderItems.AddRange(orderItems);
        }

        public void AddOrderItem(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentException("Order item cannot be null.");

            _orderItems.Add(orderItem);
        }

        public void RemoveOrderItem(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentException("Order item cannot be null.");

            _orderItems.Remove(orderItem);
        }

        public void MarkAsDelivered()
        {
            if (OrderStatus == "Delivered")
                throw new InvalidOperationException("Order is already delivered.");

            OrderStatus = "Delivered";
        }
        public void MarkAsReadyToPickup()
        {   
            if (OrderStatus == "ReadyToPickup")
                throw new InvalidOperationException("Order is already ready to pickup.");

            OrderStatus = "ReadyToPickup";
        }
    }
}
