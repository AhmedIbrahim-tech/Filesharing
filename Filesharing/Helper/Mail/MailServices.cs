using System.Net.Mail;

namespace Filesharing.Helper.Mail
{
    public class MailServices : IMailServices
    {
        private readonly IConfiguration config;

        public MailServices(IConfiguration config)
        {
            this.config = config;
        }
        public void SendMail(InputEmailMessage model)
        {
            using (SmtpClient client = new SmtpClient(config.GetValue<string>("Mail:Host"), config.GetValue<int>("Mail:Port")))
            {

                var Msg = new MailMessage();
                Msg.To.Add(model.Email);
                Msg.Subject = model.Subject;
                Msg.Body = model.Body;
                Msg.From = new MailAddress(config.GetValue<string>("Mail:Form"), config.GetValue<string>("Mail:Sender"), System.Text.Encoding.UTF8);
                Msg.IsBodyHtml = true;

                client.Credentials = new System.Net.NetworkCredential(config.GetValue<string>("Mail:Form"), config.GetValue<string>("Mail:PWD"));
                client.Send(Msg);

            }
        }
    }
}
