using Login_Register.DTO_s;

namespace Login_Register.Repository.IRepository
{
    public interface IEmailSender
    {
        //Task Execute(string email, string Subject, string message);
        //Task SendEmailAsync(string email, string subject, string Message);
        void SendEmailAsync(Emaildto emaildto);
    }
}
