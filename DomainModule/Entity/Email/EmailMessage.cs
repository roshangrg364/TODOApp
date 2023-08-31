using DomainModule.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Entity
{
    public class EmailMessage
    {
        public const string HighPriority = "High";
        public const string NormalPriority = "Normal";
        public const string LowPriority = "Low";

        public const string StatusPending = "Pending";
        public const string StatusDelivered = "Delivered";
        public const string StatusFailed = "Failed";
        protected EmailMessage() { }
        public EmailMessage(string subject, string content, string from, string priority)
        {
            Subject = subject;
            Content = content;
            From = from;
            CreatedOn = DateTime.Now;
            Priority = priority;
            MarkAsPending();
        }
        public int Id { get; protected set; }
        public string Subject { get; protected set; }
        public string Content { get; protected set; }
        public string From { get; protected set; }
        public DateTime CreatedOn { get; protected set; }
        public string Priority { get; protected set; }
        public DateTime? DeliveredOn { get; protected set; }
        public string Status { get; protected set; }
        public virtual void MarkAsPending()
        {
            Status = StatusPending;
        }
        public virtual void MarkAsDelivered()
        {
            Status = StatusDelivered;
            DeliveredOn = DateTime.Now;
        }
        public virtual void MarkAsFailed()
        {
            Status = StatusFailed;
        }
        public virtual IList<EmailRecipient> EmailRecipients { get; protected set; } = new List<EmailRecipient>();

        public virtual void AddEmailRecipients(IList<EmailRecipientDto> emailReciepientDtos)
        {
            foreach (var dto in emailReciepientDtos)
            {
                EmailRecipient emailRecipient = new(this, dto.RecipientEmailAddress);
                EmailRecipients.Add(emailRecipient);
            }
        }

    }
}
