using MimeKit;
    public class EmailSenderService
    {

    private readonly string _emailClientPassword = string.Empty;


    public EmailSenderService(IConfiguration config)
    {
        _emailClientPassword = config.GetValue<string>("Authentication:Gmail:Password") 
            ?? throw new KeyNotFoundException("Email client password could not be found");
    }
    }

