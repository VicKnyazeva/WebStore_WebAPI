using System.Collections.Generic;
using System.Threading.Tasks;

using WebStore.Domain.Entities.Orders;
using WebStore.ViewModels;

namespace WebStore.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetUserOrders(string User);
        Task<Order> GetOrderById(int Id);
        Task<Order> CreateOrder(string UserName, CartViewModel Cart, OrderViewModel OrderModel);
    }
}
