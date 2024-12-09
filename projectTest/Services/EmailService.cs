using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace projectTest.Services
{
    public class EmailService
    {
        public bool SendEmail(string fromName, string fromEmail, string toName, string toEmail, string subject, string textContent, string htmlContent)
        {
            try
            {
                // 建立郵件
                var message = new MimeMessage();

                // 添加寄件者
                message.From.Add(new MailboxAddress(fromName, fromEmail));

                // 添加收件者
                message.To.Add(new MailboxAddress(toName, toEmail));

                // 設定郵件標題
                message.Subject = subject;

                // 使用 BodyBuilder 建立郵件內容
                var bodyBuilder = new BodyBuilder
                {
                    TextBody = textContent,
                    HtmlBody = htmlContent
                };

                // 設定郵件內容
                message.Body = bodyBuilder.ToMessageBody();

                // 使用 MailKit 發送郵件
                using (var client = new SmtpClient())
                {
                    // 郵件伺服器設定
                    var hostUrl = "lccmail.nuk.edu.tw";
                    var port = 25; 
                    var useSsl = false; // 是否使用 SSL/TLS 加密

                    // 連接 Mail Server
                    client.Connect(hostUrl, port, useSsl);

                    // 提供帳號密碼進行驗證（如果需要）
                    //client.Authenticate("your_account", "your_password");

                    client.Send(message);

                    client.Disconnect(true);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
