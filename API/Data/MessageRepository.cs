using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnectionAsync(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupByConnectionIdAsync(string connectionId)
        {
            return await _context.Groups
            .Include(g => g.Connections)
            .Where(g => g.Connections.Any(c => c.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Group> GetMessageGroupAsync(string groupName)
        {
            return await _context.Groups
            .Include(g => g.Connections)
            .FirstOrDefaultAsync(g => g.Name == groupName);
        }

        public async Task<PageList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams)
        {
            var query = _context.Messages
                        .OrderByDescending(m => m.MessageSent)
                        .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                        .AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(m =>
                m.RecipientUsername == messageParams.Username
                && m.RecipientDelete == false
                ),
                "Outbox" => query.Where(m =>
                m.SenderUsername == messageParams.Username
                && m.SenderDelete == false),
                _ => query.Where(m =>
                 m.RecipientUsername == messageParams.Username
                 && m.RecipientDelete == false
                && m.MessageRead == null)
            };
            // var queryDto = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PageList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
            // .Include(m => m.Recipient).ThenInclude(m => m.Photos)
            // .Include(m => m.Sender).ThenInclude(m => m.Photos)
            .Where(m => m.Recipient.UserName == currentUsername
             && m.Sender.UserName == recipientUsername
             && m.RecipientDelete == false
              ||
             m.Sender.UserName == currentUsername
             && m.SenderDelete == false
             && m.Recipient.UserName == recipientUsername
             )
             .OrderBy(m => m.MessageSent)
             .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
             .ToListAsync();
            var uReadMessages = messages.Where(m => m.RecipientUsername == currentUsername &&
            m.MessageRead == null).ToList();
            if (uReadMessages.Any())
            {
                foreach (var message in uReadMessages)
                {
                    message.MessageRead = DateTime.UtcNow;
                }
            }
            // return _mapper.Map<IEnumerable<MessageDto>>(messages);
            return messages;

        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

    }
}