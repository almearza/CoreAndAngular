using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using API.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using API.Entities;
using System.Linq;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _presenceTracker;

        public MessageHub(IMessageRepository messageRepository,
                          IMapper mapper,
                          IUserRepository userRepository,
                          IHubContext<PresenceHub> presenceHub,
                          PresenceTracker presenceTracker
                          )
        {
            _userRepository = userRepository;
            this._presenceHub = presenceHub;
            this._presenceTracker = presenceTracker;
            this._mapper = mapper;
            this._messageRepository = messageRepository;
        }
        public async override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
            //we need it to update read in sender because we donot want to send the messages agian to sender
            var messages = await _messageRepository.GetMessageThreadAsync(Context.User.GetUsername(), otherUser);
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
            // await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
            Console.WriteLine(messages);
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(CreateMessage createMessage)
        {
            Console.WriteLine(" recipient ----------------------------------------------");
            Console.WriteLine(createMessage.RecipientUsername);
            var username = Context.User.GetUsername();

            var sender = await _userRepository.GetUserByUserNameAsync(username);
            var recipient = await _userRepository.GetUserByUserNameAsync(createMessage.RecipientUsername.ToLower());
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine(recipient.UserName);
            if (recipient == null)
                throw new HubException("recipient user not found !");
            if (recipient.UserName.ToLower() == sender.UserName)
                throw new HubException("sender will not be reipient of message");
            var message = new Message
            {
                Sender = sender,
                SenderUsername = sender.UserName,
                Recipient = recipient,
                RecipientUsername = recipient.UserName,
                Content = createMessage.Content
            };
            _messageRepository.AddMessage(message);


            var groupName = GetGroupName(username, createMessage.RecipientUsername);
            var group = await _messageRepository.GetMessageGroupAsync(groupName);
            if (group != null && group.Connections.Any(c => c.Username == recipient.UserName))
            {
                message.MessageRead = DateTime.UtcNow;
            }
            else
            {
                var recpientConnections = await _presenceTracker.GetConnectionForUser(recipient.UserName);
                if (recpientConnections != null)
                    await _presenceHub.Clients.Clients(recpientConnections)
                    .SendAsync("NewMessageReceived", new
                    {
                        username = sender.UserName,
                        knownUs = sender.KnownUs
                    });
            }
            if (await _messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }
        private string GetGroupName(string caller, string other)
        {
            var strComare = string.Compare(caller, other) < 0;
            return strComare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _messageRepository.GetMessageGroupAsync(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());
            if (group == null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);
            if (await _messageRepository.SaveAllAsync()) return group;
            throw new HubException("unable to add this connection");

        }
        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageRepository.GetGroupByConnectionIdAsync(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            if (await _messageRepository.SaveAllAsync()) return group;
            throw new HubException("unable to remove this connection");
        }
    }
}