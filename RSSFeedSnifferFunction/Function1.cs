using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net;
using System.Net.Mail;
using System.ServiceModel.Syndication;
using System.Xml;

namespace RSSFeedSnifferFunction
{
    public static class Function1
    {
        /// <summary>
        /// Email send parameters
        /// </summary>
        static string smtpAddress = "smtp.gmail.com";
        static int portNumber = 587;
        static bool enableSSL = true;
        static string emailFromAddress = "techegghead@gmail.com";
        static string password = "W#ikfield2758123@";
        static string emailToAddress = "varun.maggo@gmail.com";
        static string subject = "RSS FEED SNIFF DATA";
        static string body = "";
        static string url = "http://feeds.bbci.co.uk/news/world/asia/rss.xml";

        /// <summary>
        /// Batch job to be run every 6 hours
        /// </summary>
        /// <param name="myTimer"></param>
        /// <param name="log"></param>
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("00:06:00")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();
            foreach (SyndicationItem item in feed.Items)
            {
                String subject = item.Title.Text;
                //String summary = item.Summary.Text;
                body = body + "\n" + subject;
            }

            SendEmail();
            body = "";
        }

        /// <summary>
        /// Send email method
        /// </summary>
        public static void SendEmail()
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFromAddress);
                mail.To.Add(emailToAddress);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }
    }
}
