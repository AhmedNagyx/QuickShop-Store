using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;

namespace QuickShop.Services
{
    public class EmailSender
    {
        public static void SendEmail(string senderName,string senderEmail,string toName, string toEmail,
            string textContent,string subject)
        {
            var apiInstance = new TransactionalEmailsApi();
            SendSmtpEmailSender sender = new SendSmtpEmailSender(senderName, senderEmail);
            SendSmtpEmailTo reciever = new SendSmtpEmailTo(toName, toEmail);
            List<SendSmtpEmailTo> recieverList = new List<SendSmtpEmailTo>();
            recieverList.Add(reciever);

            try
            {
                var sendSmtpEmail = new SendSmtpEmail(sender, recieverList, null, null, null, textContent, subject);
                CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
                Console.WriteLine("Email sender ok: "+ result.ToJson());
            }
            catch (Exception e)
            {
                Console.WriteLine("Email sender fail: "+ e.Message);
            }
        }
    }
}
