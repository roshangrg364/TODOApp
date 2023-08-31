using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Dto
{
    public class EmailMessageDto
    {
        public EmailMessageDto(string subject,string content, string from, string priority)
        {
            Subject = subject;
            Content = content;
            From = from;
            Priority = priority;
        }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string From { get; set; }
        public string Priority { get; protected set; }
        public IList<EmailRecipientDto> EmailRecipients { get; set; } = new List<EmailRecipientDto>();
    }

    public class EmailRecipientDto
    {
        public EmailRecipientDto(string recipientEmailAddress)
        {
            RecipientEmailAddress = recipientEmailAddress;

        }
        public string RecipientEmailAddress { get; set; }
    }
}
