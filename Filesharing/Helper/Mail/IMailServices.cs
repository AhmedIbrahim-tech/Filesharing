namespace Filesharing.Helper.Mail
{
    public interface IMailServices
    {
        void SendMail(InputEmailMessage model);
    }
}
