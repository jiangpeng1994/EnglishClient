public sealed class DownloadTask
{
    private static int s_Serial;
    public int SerialId { get; private set; }
    public DownloadTaskStatus Status { get; set; }
    public string DownloadUrl { get; private set; }
    public string DownloadPath { get; private set; }
    public string MD5 { get; private set; }
    public int FlushSize { get; private set; }
    public float Timeout { get; private set; }
    public object UserData { get; private set; }

    public DownloadTask()
    {
        Status = DownloadTaskStatus.Todo;
        DownloadUrl = null;
        DownloadPath = null;
        MD5 = null;
        FlushSize = 0;
        Timeout = 0f;
        UserData = null;
    }

    public static DownloadTask Create(string downloadUrl, string downloadPath, string md5, int flushSize, float timeout, object userData)
    {
        DownloadTask downloadTask = new DownloadTask
        {
            SerialId = ++s_Serial,
            DownloadUrl = downloadUrl,
            DownloadPath = downloadPath,
            MD5 = md5,
            FlushSize = flushSize,
            Timeout = timeout,
            UserData = userData
        };
        return downloadTask;
    }

    public void Clear()
    {
        Status = DownloadTaskStatus.Todo;
        DownloadUrl = null;
        DownloadPath = null;
        MD5 = null;
        FlushSize = 0;
        Timeout = 0f;
        UserData = null;
    }
}

public enum DownloadTaskStatus
{
    Todo,
    Doing,
    Done,
    Error
}