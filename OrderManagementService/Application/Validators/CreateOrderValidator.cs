using OrderManagementService.Application.DTOs;
using System.Text.RegularExpressions;

namespace OrderManagementService.Application.Validators
{
    public static class CreateOrderValidator
    {
        public static void ValidateStreet(string street)
        {
            if (string.IsNullOrWhiteSpace(street))
            {
                throw new ArgumentException("Street is required.");
            }

            if (street.Length > 255)
            {
                throw new ArgumentException("Street must not exceed 255 characters.");
            }
        }

        public static void ValidateCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentException("City is required.");
            }
        }

        public static void ValidatePostalCode(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode))
            {
                throw new ArgumentException("PostalCode is required.");
            }

            if (!Regex.IsMatch(postalCode, @"^\d{4}$"))
            {
                throw new ArgumentException("PostalCode must be exactly 4 digits.");
            }
        }

        public static void ValidateOrderItems(List<OrderItemRequest> orderItems)
        {
            if (orderItems == null || orderItems.Count == 0)
            {
                throw new ArgumentException("At least one order item is required.");
            }

            foreach (var item in orderItems)
            {
                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    throw new ArgumentException("Order item name is required.");
                }

                if (item.Price <= 0)
                {
                    throw new ArgumentException("Order item price must be greater than zero.");
                }
            }
        }

        public static void Validate(CreateOrderRequest request)
        {
            ValidateStreet(request.Street);
            ValidateCity(request.City);
            ValidatePostalCode(request.PostalCode);
            ValidateOrderItems(request.OrderItems);
        }
    }
}
