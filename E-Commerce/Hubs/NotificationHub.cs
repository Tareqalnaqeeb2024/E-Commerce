using Microsoft.AspNetCore.SignalR;
namespace E_Commerce.Hubs
{
    public class NotificationHub :Hub
    {
        public async Task NotifyAdminNewOrder(int orderId, string customerName)
        {
            await Clients.Group("Admins").SendAsync("ReceiveNewOrder", orderId, customerName);
        }
    }
}
