using System;

namespace FinancialChatBot
{
    public class Message
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public string UserName { get; set; }
    }
}
