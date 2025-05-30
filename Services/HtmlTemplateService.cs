
    public class HtmlTemplateService
    {

    //Template for resetting password
    public string PasswordResetTemplate(string otp, string name, string appName)
    {
        return $@"<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Password Reset Request</title>
   {HtmlTemplateHelper.GenerateCSSCommonStyles()}
  </head>
  <body>
    <div class=""container"">
      <div class=""bg-white p-4 rounded shadow-sm"">
        <!-- Logo section -->
        <div class=""text-start mb-2"">
          <img
            src=""https://images.unsplash.com/vector-1739891945136-9959e9a14e57?q=80&w=1480&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D""
            alt=""Company Logo""
            class=""img-fluid""
            style=""max-width: 3rem""
          />
        </div>

        <!-- Title section -->
        <div class=""mb-4"">
          <h1 class=""text-dark"">Password reset request</h1>
        </div>

        <!-- Message section -->
        <div>
          <p>Hi {name},</p>
          <p>
           We received a request to reset your password. Please use the one-time password (OTP) below to continue:
          </p>
           <p>Your reset code: {otp}</p>
          <p>Thank you,</p>
          <p>{appName}</p>
        </div>

        <!-- Footer -->
        <div>
          <p class=""text-muted small"">
            This code will expire in 10 minutes. If you didn’t request a password reset, you can safely ignore this message.
          </p>
        </div>
      </div>
    </div>
  </body>
</html>

";
    }

    //Template for confirming email
    public string EmailConfirmationTemplate(string url, string name, string appName)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Email Confirmation</title>
     {HtmlTemplateHelper.GenerateCSSCommonStyles()}
  </head>
  <body class=""bg-light"">
    <div class=""container my-5"">
      <div class=""bg-white p-4 rounded shadow-sm"">
        <!--Logo section-->
        <div class=""text-start mb-2"">
          <img
            src=""https://images.unsplash.com/vector-1739891945136-9959e9a14e57?q=80&w=1480&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D""
            alt=""Company Logo""
            class=""img-fluid""
            style=""max-width: 3rem""
          />
        </div>
        <!--Title section-->
        <div class=""mb-4"">
          <h1 class=""h4 text-dark"">
            Confirm Your Email
          </h1>
        </div>
        <!--Message section-->
        <div>
          <p>Hi {name},</p>
          <p>
          To ensure continued access to your car wash account, please verify your email address using the button below.
          </p>
          <p>Best regards,</p>
          <p>{appName}</p>
        </div>
        <div>
          <a href=""{url}"" class=""btn btn-dark""
            >Confirm</a
          >
          <p class=""text-muted small"">
            If this wasn’t you, please disregard this message.
          </p>
        </div>
      </div>
    </div>
  </body>
</html>
";
    }
    //Template for contact us
    public string ContactUsTemplate(string email, string name, string message)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Contact Us Message</title>
    {HtmlTemplateHelper.GenerateCSSCommonStyles()}
  </head>
  <body class=""bg-light"">
    <div class=""container my-5"">
      <div class=""bg-white p-4 rounded shadow-sm"">
        <!--Logo section-->
        <div class=""text-start mb-2"">
          <img
            src=""https://images.unsplash.com/vector-1739891945136-9959e9a14e57?q=80&w=1480&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D""
            alt=""Company Logo""
            class=""img-fluid""
            style=""max-width: 3rem""
          />
        </div>
        <!--Title section-->
        <div class=""mb-4"">
          <h1 class=""h4 text-dark"">New Contact Us Message</h1>
        </div>
        <!--Message section-->
        <div>
          <p class=""text-secondary""><strong>Name:</strong> {name}</p>
          <p class=""text-secondary""><strong>Email:</strong> {email}</p>
          <p class=""text-secondary""><strong>Message:</strong></p>
          <p class=""text-secondary"">
            {message}
          </p>
        </div>
        <!-- Footer -->
        <div>
          <p class=""text-muted small"">
            This message was sent via the contact form on your website.
          </p>
        </div>
      </div>
    </div>
  </body>
</html>

";
    }

}

