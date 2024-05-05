using Infrastructure.Mail;

namespace Domain.IService
{
    public interface ISendMailService
    {
        Task SendEmailAsync(MailRequestWithAttachments mailRequest);
        Task SendEmailWithoutAttachmentAsync(MailRequest mailRequest);
    }
}
