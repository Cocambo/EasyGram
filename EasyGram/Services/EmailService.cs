using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Net.Mime;
using EasyGram.ViewModels.Exam;

namespace EasyGram.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendCertificateAsync(string emailAddress, byte[] certificatePdf, CertificateViewModel certificateData)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Система обучения EasyGram", _configuration["Email:FromAddress"])); // адрес отправителя
            message.To.Add(new MailboxAddress("", emailAddress)); // адрес получателя
            message.Subject = "Ваш сертификат об успешном прохождении экзамена"; // тема 

            var builder = new BodyBuilder();
            builder.TextBody = $@"
Поздравляем с успешным прохождением экзамена!

Ваши результаты:
- Оценка: {certificateData.Grade}
- Процент выполнения: {certificateData.Percentage}%
- Правильных ответов: {certificateData.CorrectAnswers} из {certificateData.TotalQuestions}
- Дата прохождения: {certificateData.CompletedDate:dd.MM.yyyy}

В прикреплении к письму находится ваш сертификат.

С уважением,
Команда системы обучения EasyGram";

            
            builder.Attachments.Add($"Certificate_{DateTime.Now:yyyyMMdd}.pdf", // Добавление PDF-вложения
                                   certificatePdf,
                                   new MimeKit.ContentType("application", "pdf"));

            message.Body = builder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient(); // отправка письма через SMPT

            await client.ConnectAsync(
                                    _configuration["Email:SmtpHost"],
                                    _configuration.GetValue<int>("Email:SmtpPort"),
                                    SecureSocketOptions.SslOnConnect);

            await client.AuthenticateAsync(
                                    _configuration["Email:Username"],
                                    _configuration["Email:Password"]);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
