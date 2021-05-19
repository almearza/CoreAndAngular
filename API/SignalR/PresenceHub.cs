using System;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;
        public PresenceHub(PresenceTracker presenceTracker)
        {
            this._presenceTracker = presenceTracker;

        }
        public async override Task OnConnectedAsync()
        {
            await _presenceTracker.UserConnected(Context.User.GetUsername(),Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
            //set online users:
            var onlineUsers =await _presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("OnlineUsers",onlineUsers);
        }
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await _presenceTracker.UserDisConnected(Context.User.GetUsername(),Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
            //set online users:
            var onlineUsers =await _presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("OnlineUsers",onlineUsers);
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}