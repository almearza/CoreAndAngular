using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessage createMessage)
        {
            if (string.IsNullOrEmpty(createMessage.RecipientUsername))
                return BadRequest("no recipient username found");

            var sender = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            var recipient = await _userRepository.GetUserByUserNameAsync(createMessage.RecipientUsername.ToLower());

            if (recipient == null)
                return BadRequest("recipient user not found !");
            if (recipient.UserName.ToLower() == sender.UserName)
                return BadRequest("sender will not be reipient of message");
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
            if (result)
                return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("something went wrong while sending message");

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await _messageRepository.GetMessagesForUserAsync(messageParams);
            Response.AddPaginationHeader(messages.CurrentPage, messages.TotalPages, messages.TotalCount, messages.PageSize);
            return Ok(messages);
        }
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesThread(string username)
        {
            var currentUsername = User.GetUsername();
            return Ok(await _messageRepository.GetMessageThreadAsync(currentUsername, username));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var message = await _messageRepository.GetMessageAsync(id);
            var currentUsername = User.GetUsername();
            if (message == null ||
            (message.SenderUsername != currentUsername && message.RecipientUsername != currentUsername))
                return Unauthorized("no message or u havenot permission to delete it");

            if (message.SenderUsername == currentUsername) message.SenderDelete = true;
            if (message.RecipientUsername == currentUsername) message.RecipientDelete = true;
            if (message.SenderDelete && message.RecipientDelete)
                _messageRepository.DeleteMessage(message);
                if(await _messageRepository.SaveAllAsync())
                return Ok();
                return BadRequest("some thing went wrong !");
        }
    }
}