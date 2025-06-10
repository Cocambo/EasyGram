using EasyGram.ViewModels.Exam;

namespace EasyGram.Services
{
    public interface IEmailService
    {
        Task SendCertificateAsync(string emailAddress, byte[] certificatePdf, CertificateViewModel certificateData);
    }
}
