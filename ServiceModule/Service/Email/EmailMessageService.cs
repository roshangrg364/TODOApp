using DomainModule.Dto;
using DomainModule.Entity;
using DomainModule.Exceptions;
using DomainModule.RepositoryInterface;
using DomainModule.ServiceInterface;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceModule.Service
{
    public class EmailMessageService : EmailMessageServiceInterface
    {
        private readonly EmailMessageRepositoryInterface _emailMessageRepo;
        private readonly IUnitOfWork _unitOfWork;
        public EmailMessageService(EmailMessageRepositoryInterface emailMessageRepo,
            IUnitOfWork unitOfWork)
        {
            _emailMessageRepo = emailMessageRepo;
            _unitOfWork = unitOfWork;

        }
        public async Task CreateEmailMessage(EmailMessageDto dto)
        {
            try
            {
                ValidateEmailMessage(dto);
                EmailMessage emailMessage = AddEmailMessage(dto);
                await _emailMessageRepo.InsertAsync(emailMessage).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task CreateEmailMessageAndSendEmail(EmailMessageDto dto)
        {

            try
            {
                ValidateEmailMessage(dto);
                EmailMessage emailMessage = AddEmailMessage(dto);
                emailMessage.MarkAsDelivered();
                await _emailMessageRepo.InsertAsync(emailMessage).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                await SendEmails(new List<EmailMessage> { emailMessage });
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        public async Task SendEmails(List<EmailMessage> emailMessages)
        {
            try
            {
                using var smtp = new SmtpClient();
                if (!smtp.IsConnected)
                {
                    await smtp.ConnectAsync("smtp.ethereal.email", 587, SecureSocketOptions.StartTls).ConfigureAwait(false);
                    await smtp.AuthenticateAsync("freeman18@ethereal.email", "rDyA84NevH2B9S6ZrM").ConfigureAwait(false);
                }
                foreach (var emailMessage in emailMessages)
                {
                    await SendEmail(smtp, emailMessage).ConfigureAwait(false);

                }
                await smtp.DisconnectAsync(true).ConfigureAwait(false);
            }
            catch (Exception)
            {

                throw;
            }

        }

        private static async Task SendEmail(SmtpClient smtp, EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.Sender = MailboxAddress.Parse("freeman18@ethereal.email");
            var recipients = emailMessage.EmailRecipients.Select(a => a.RecipientEmail).ToList();
            foreach (var recipient in recipients)
            {
                message.To.Add(MailboxAddress.Parse(recipient));
            }

            message.Subject = emailMessage.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = emailMessage.Content;
            message.Body = builder.ToMessageBody();
            await smtp.SendAsync(message).ConfigureAwait(false);
        }

        private static EmailMessage AddEmailMessage(EmailMessageDto dto)
        {
            var emailMessage = new EmailMessage(dto.Subject, dto.Content, "testEmail.com", EmailMessage.HighPriority);
            emailMessage.AddEmailRecipients(dto.EmailRecipients);
            return emailMessage;
        }

        private static void ValidateEmailMessage(EmailMessageDto dto)
        {
            if (string.IsNullOrEmpty(dto.Content)) throw new CustomException("No Email Content Found");
            if (!dto.EmailRecipients.Any()) throw new CustomException("No Recipients found");
        }
    }
}
