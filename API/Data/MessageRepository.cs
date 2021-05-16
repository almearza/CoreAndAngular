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
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            return await _context.Messages
            .Include(m=>m.Sender)
            .Include(m=>m.Recipient)
            .SingleOrDefaultAsync(m=>m.Id==id);
        }

        public async Task<PageList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams)
        {
            var query = _context.Messages
                        .OrderByDescending(m => m.MessageSent)
                        .AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(m => 
                m.Recipient.UserName == messageParams.Username
                &&m.RecipientDelete==false
                ),
                "Outbox" => query.Where(m => 
                m.Sender.UserName == messageParams.Username
                &&m.SenderDelete==false),
                _ => query.Where(m =>
                 m.Recipient.UserName == messageParams.Username
                 &&m.RecipientDelete==false
                && m.MessageRead == null )
            };
            var queryDto = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PageList<MessageDto>.CreateAsync(queryDto, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
            .Include(m => m.Recipient).ThenInclude(m => m.Photos)
            .Include(m => m.Sender).ThenInclude(m => m.Photos)
            .Where(m => m.Recipient.UserName == currentUsername
             && m.Sender.UserName == recipientUsername
             &&m.RecipientDelete==false
              ||
             m.Sender.UserName == currentUsername
             &&m.SenderDelete==false
             && m.Recipient.UserName == recipientUsername
             )
             .OrderBy(m=>m.MessageSent)
             .ToListAsync();
            var uReadMessages = messages.Where(m => m.Recipient.UserName == currentUsername &&
            m.MessageRead == null).ToList();
            if(uReadMessages.Any()){
                foreach (var message in uReadMessages)
                {
                    message.MessageRead=DateTime.Now;
                }
                await SaveAllAsync();
            }
            return _mapper.Map<IEnumerable<MessageDto>>(messages);

        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}