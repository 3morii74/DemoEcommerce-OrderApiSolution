using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversations;
using OrderApi.Application.Interfaces;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface, HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        public async Task<ProductDTO> GetProduct(int productId)
        {
            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
            {
                // Log the error or handle it appropriately
                return null!;
            }
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }

        public async Task<AppUserDTO> GetUser(int userId)
        {
            var getUser = await httpClient.GetAsync($"/api/users/{userId}");
            if (!getUser.IsSuccessStatusCode)
            {
                // Handle the case where the user is not found
                return null!;
            }
            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user!;
        }

        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            // Prepare Order
            var order = await orderInterface.FindByIdAsync(orderId);
            if (order is null || order.Id <= 0)
                return null!;

            // Get Retry pipeline
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            // Prepare Product
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));
            if (productDTO is null)
                return null!;

            // Prepare Client (allowed to be null)
            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            // Populate order Details (handle null appUserDTO)
            return new OrderDetailsDTO(
                 order.Id,
                 productDTO.Id,
                 appUserDTO?.Id,          // Null-safe access
                 appUserDTO?.Email,
                 appUserDTO?.Address,
                 appUserDTO?.TelephoneNumber,
                 productDTO.Name,
                 order.PurchaseQuantity,
                 productDTO.Price,
                 productDTO.Quantity * order.PurchaseQuantity,
                 order.OrderedDate // Fix typo: "UnderedDate" → "OrderedDate"
                );
        }

        // GET ORDERS BY CLIENT ID
        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            // Get all Client's orders
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
            if (!orders.Any()) return null!;

            // Convert from entity to DTO
            var (_, _orders) = OrderConversation.FromEntity(null, orders);
            return _orders!;
        }
    }
}