﻿using Microsoft.AspNetCore.Mvc;
using OrderManagementService.Application.DTOs;
using OrderManagementService.Application.Ports;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.ValueObjects;

namespace OrderManagementService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessagePublisher _messagePublisher;

        public OrdersController(IOrderRepository orderRepository, IMessagePublisher messagePublisher)
        {
            _orderRepository = orderRepository;
            _messagePublisher = messagePublisher;
        }
        //api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            // Validate request
            if (request == null || request.OrderItems == null || !request.OrderItems.Any())
                return BadRequest("Invalid order request. Must contain order items.");

            // Map OrderItems from DTO to Domain objects
            var orderItems = request.OrderItems
                .Select(item => new OrderItem(item.Name, item.Price))
                .ToList();

            // Create the Order entity
            var order = new Order(request.CustomerId, request.RestaurantId, orderItems);

            // Save to repository
            await _orderRepository.AddOrderAsync(order);

            // Create DeliveryAddress
            var deliverAddress = new DeliveryAddress(request.Street, request.City, request.PostalCode);

            // Publish event
            await _messagePublisher.PublishOrderCreatedAsync(order.OrderId, deliverAddress);
            
            // Return the created order
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        [HttpPost("publish")]
        public async Task<IActionResult> PublishMessage([FromBody] DeliveryAddress request)
        {
            var deliveryAddress = new DeliveryAddress(request.Street, request.City, request.PostalCode);
            await _messagePublisher.PublishOrderCreatedAsync(Guid.NewGuid(), deliveryAddress);

            return Ok("Message published to Google Cloud Pub/Sub.");
        }
        //api/order/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            return order == null ? NotFound() : Ok(order);
        }
        //api/order/id
        [HttpGet("pending/{id}")]
        public async Task<IActionResult> GetPendingOrdersById(Guid id)
        {
            var pendingOrders = await _orderRepository.GetOrdersByRestaurantAndStatusAsync(id, "Pending");

            if (pendingOrders == null || !pendingOrders.Any())
                return NotFound("No pending orders found for the specified restaurant.");

            return Ok(pendingOrders);
        }
    }
}