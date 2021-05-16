using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
         void AddMessage(Message message);
         void DeleteMessage(Message message);
         Task<Message> GetMessageAsync(int id);
         Task<PageList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams);
         Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currentUsername,string recipientUsername);
         Task<bool>SaveAllAsync();
    }
}