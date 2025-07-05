using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;

            if (user?.IsInRole("Admin") == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                Console.WriteLine($"Admin connected: {Context.ConnectionId}");
            }
            else if (user?.Identity?.IsAuthenticated == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Users");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.User?.IsInRole("Admin") == true)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
