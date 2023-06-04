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
        public void SendMail(EmailBody model)
        {
            // Get Host and Port 
            using (SmtpClient client = new SmtpClient(config.GetValue<string>("Mail:Host"), config.GetValue<int>("Mail:Port")))
            {
                // Body Of Mail 
                var Msg = new MailMessage();
                Msg.To.Add(model.Email);                    // Get Mail Of Sender 
				Msg.Subject = model.Subject;
                Msg.Body = model.Body;
                Msg.From = new MailAddress(config.GetValue<string>("Mail:From"), config.GetValue<string>("Mail:Sender"), System.Text.Encoding.UTF8);

                // To Accept Html 
                Msg.IsBodyHtml = true;

                // Get Mail and Password that Send to it [ebrahema89859@gmail.com , Password]
                client.Credentials = new System.Net.NetworkCredential(config.GetValue<string>("Mail:From"), config.GetValue<string>("Mail:PWD"));
                client.Send(Msg);

            }
        }
    }
}
