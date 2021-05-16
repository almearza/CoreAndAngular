using System;

namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public AppUser Sender { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public AppUser Recipient { get; set; }
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string Content { get; set; }
        public DateTime MessageSent { get; set; }=DateTime.Now;
        public DateTime? MessageRead { get; set; }
        public bool SenderDelete { get; set; }
        public bool RecipientDelete { get; set; }
    }
}