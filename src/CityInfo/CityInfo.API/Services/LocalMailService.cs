namespace CityInfo.API.Services;

public class LocalMailService
{
    private const string MailTo = "admin@mycompany.com";
    private const string MailFrom = "noreply@mycompany.com";

    public void Send(string subject, string message)
    {
        // send mail - output to console window only
        Console.WriteLine($"Mail from {MailFrom} to {MailTo} with {nameof(LocalMailService)}.");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
    }
}
