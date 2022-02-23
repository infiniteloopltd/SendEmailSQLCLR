using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Microsoft.SqlServer.Server;

public partial class SendEmailCLR
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static Int32 SendEmail(string smtpServer, string smtpUsername, string smtpPassword, string from, string to, string subject, string body)
    {
        SqlPipe sqlP = SqlContext.Pipe;
        try
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 |
                                                   SecurityProtocolType.Tls |
                                                   (SecurityProtocolType)768 |
                                                   (SecurityProtocolType)3072;
            var mail = new MailMessage(from, to);
            var client = new SmtpClient
            {
                Port = 25,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = smtpServer,
                Credentials = new NetworkCredential(
                    smtpUsername,
                    smtpPassword)

            };
            mail.IsBodyHtml = true;
            mail.Subject = subject;
            mail.Body = body;
            mail.BodyEncoding = Encoding.UTF8;
            Retry((i) =>
            {
                client.Send(mail);
            }, 5);
            sqlP.Send("Sent html email to " + to);
            return 1;
        }
        catch (Exception ex)
        {
        
            sqlP.Send(ex.ToString());
            return -1;
        }
   
    }

    private static void Retry(Action<int> action, int times)
    {
        var lastException = new Exception();
        for (var i = 0; i < times; i++)
        {
            try
            {
                action(i);
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
        throw lastException;
    }
}
