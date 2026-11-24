namespace Core.Notifications;

public class FileSink : INotificationSink
{
    private readonly string _path;

    public FileSink(string path)
    {
        _path = path;

        Directory.CreateDirectory(
            Path.GetDirectoryName(_path)!);
    }

    public void Notify(string message)
    {
        File.AppendAllText(
            _path,
            message + Environment.NewLine);
    }
}