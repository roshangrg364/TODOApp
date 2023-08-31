using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Entity
{
    public class EmailRecipient
    {
        protected EmailRecipient()
        {
            
        }
        public EmailRecipient(EmailMessage emailMessage, string recipientEmail)
        {
            RecipientEmail = recipientEmail;
            EmailMessage = emailMessage;
            CreatedOn = DateTime.Now;
        }

        public  int Id { get; protected set; }
        public  string RecipientEmail { get; protected set; }
        public  DateTime CreatedOn { get; protected set; }
        public int EmailMessageId { get; protected set; }
        public virtual EmailMessage EmailMessage { get; protected set; }
    }
}
