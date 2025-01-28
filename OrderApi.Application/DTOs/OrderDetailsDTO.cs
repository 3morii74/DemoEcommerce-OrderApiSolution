using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.DTOs
{
    public class OrderDetailsDTO
    {
        public int OrderId { get; }
        public int ProductId { get; }
        public int? Client { get; }       // Nullable
        public string? Email { get; }     // Nullable
        public string? Address { get; }   // Nullable
        public string? TelephoneNumber { get; } // Nullable
        public string ProductName { get; }
        public int PurchaseQuantity { get; }
        public decimal UnitPrice { get; }
        public decimal TotalPrice { get; }
        public DateTime OrderedDate { get; }

        public OrderDetailsDTO(
            int orderId,
            int productId,
            int? client,
            string? email,
            string? address,
            string? telephoneNumber,
            string productName,
            int purchaseQuantity,
            decimal unitPrice,
            decimal totalPrice,
            DateTime orderedDate
        )
        {
            OrderId = orderId;
            ProductId = productId;
            Client = client;
            Email = email;
            Address = address;
            TelephoneNumber = telephoneNumber;
            ProductName = productName;
            PurchaseQuantity = purchaseQuantity;
            UnitPrice = unitPrice;
            TotalPrice = totalPrice;
            OrderedDate = orderedDate;
        }
    }
}