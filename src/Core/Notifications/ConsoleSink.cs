namespace Core.Notifications;

public class ConsoleSink : INotificationSink
{
    public void Notify(string message)
    {
        Console.WriteLine(message);
    }
}