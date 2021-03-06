using System;
using System.Text.Json.Serialization;

namespace API.Dtos
{
    public class MessageDto
    {

        public int Id { get; set; }
        public string SenderPhotoUrl { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string RecipientPhotoUrl { get; set; }
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string Content { get; set; }
        public DateTime MessageSent { get; set; }
        public DateTime? MessageRead { get; set; }

        [JsonIgnore]
        public bool SenderDelete { get; set; }
        [JsonIgnore]
        public bool RecipientDelete { get; set; }
    }
}