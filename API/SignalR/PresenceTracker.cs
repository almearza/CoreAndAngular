using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        public static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();
        public Task<bool> UserConnected(string username, string connectionId)
        {
            var isOnline=false;
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    isOnline=true;
                    OnlineUsers.Add(username, new List<string> { connectionId });
                }
            }
            return Task.FromResult(isOnline);
        }
        public Task<bool> UserDisConnected(string username, string connectionId)
        {
            var isOffline=false;
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username))
                    return Task.FromResult(isOffline);

                OnlineUsers[username].Remove(connectionId);

                if (OnlineUsers[username].Count == 0)
                {
                    isOffline=true;
                    OnlineUsers.Remove(username);
                }

            }
            return Task.FromResult(isOffline);
        }
        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock(OnlineUsers){
                onlineUsers = OnlineUsers.OrderBy(k=>k.Key).Select(k=>k.Key).ToArray();
            }
            return Task.FromResult(onlineUsers);
        }
        public Task<List<string>> GetConnectionForUser(string username){
            var connections = new List<string>();
            lock(OnlineUsers){
                connections = OnlineUsers.GetValueOrDefault(username);
            }
            return Task.FromResult(connections);
        }
    }
}