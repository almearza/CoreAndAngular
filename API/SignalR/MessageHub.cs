using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using API.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using API.Entities;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public MessageHub(IMessageRepository messageRepository,
                          IMapper mapper,
                          IUserRepository userRepository)
        {
            _userRepository = userRepository;
            this._mapper = mapper;
            this._messageRepository = messageRepository;
        }
        public async override Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            var otherUser = context.Request.Query["user"];
            var groupName = GetGroupName(context.User.GetUsername(), otherUser);
            var messages = await _messageRepository.GetMessageThreadAsync(context.User.GetUsername(), otherUser);
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(CreateMessage createMessage)
        {
            var username = Context.User.GetUsername();

            var sender = await _userRepository.GetUserByUserNameAsync(username);
            var recipient = await _userRepository.GetUserByUserNameAsync(createMessage.RecipientUsername.ToLower());

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
            var result = await _messageRepository.SaveAllAsync();
            if (result){
                var groupName = GetGroupName(username,createMessage.RecipientUsername);
                await Clients.Group(groupName).SendAsync("NewMessage",_mapper.Map<MessageDto>(message));
            }
        }
        private string GetGroupName(string caller, string other)
        {
            var strComare = string.Compare(caller, other) < 0;
            return strComare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}