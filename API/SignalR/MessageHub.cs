using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessageHub(IMessageRepository messageRepository, IMapper mapper)
        {
            this._mapper = mapper;
            this._messageRepository = messageRepository;
        }
        public async override Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            var otherUser = context.Request.Query["user"];
            var groupName = GetGroupName(context.User.GetUsername(),otherUser);
            var messages = await _messageRepository.GetMessageThreadAsync(context.User.GetUsername(),otherUser);
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread",messages);
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
        private string GetGroupName(string caller, string other)
        {
            var strComare = string.Compare(caller, other) < 0;
            return strComare?$"{caller}-{other}":$"{other}-{caller}";
        }
    }
}