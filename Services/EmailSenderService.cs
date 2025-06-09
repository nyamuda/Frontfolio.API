using MimeKit;
using MailKit.Net.Smtp;
public class EmailSenderService
{

    private readonly string _appEmailPassword = string.Empty;
    private readonly string _appEmail = "cratecrarity@gmail.com";
    private readonly string _appName = "Frontfolio";


    public EmailSenderService(IConfiguration config)
    {
        _appEmailPassword = config.GetValue<string>("Authentication:Gmail:Password")
            ?? throw new KeyNotFoundException("SMTP password not found in config.");

        _appName = config.GetValue<string>("AppSettings:AppName")
            ?? throw new KeyNotFoundException("App name not found in config.");

    }


    public async Task SendEmail(string name, string email, string subject, string message)
    {

        var messageToSend = new MimeMessage();
        messageToSend.From.Add(new MailboxAddress(_appName, _appEmail));
        messageToSend.To.Add(new MailboxAddress(name, email));
        messageToSend.Subject = subject;

        //send email body as HTML
        messageToSend.Body = new TextPart("html")
        {
            Text = message
        };

        using (var client = new SmtpClient())
        {
            client.Connect("smtp.gmail.com", 587, false);

            client.Authenticate(_appEmail, _appEmailPassword);

            await client.SendAsync(messageToSend);

            client.Disconnect(true);
        }

    }
}

