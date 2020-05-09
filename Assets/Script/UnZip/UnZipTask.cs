public class UnZipTask
{
    private static int s_Serial;
    public int SerialId { get; private set; }
    public UnZipTaskStatus Status { get; set; }
    public string SourceFilePath { get; private set; }
    public string UnZipPath { get; private set; }
    public int UnZipType { get; private set; }
    public bool IsDeleteSourceFileAfterUnZip { get; private set; }
    public object UserData { get; private set; }

    public UnZipTask()
    {
        Status = UnZipTaskStatus.Todo;
        SourceFilePath = null;
        UnZipPath = null;
        UnZipType = 0;
        IsDeleteSourceFileAfterUnZip = false;
        UserData = null;
    }

    public static UnZipTask Create(string sourceFilePath, string unZipPath, int unZipType, bool isDeleteSourceFileAfterUnZip, object userData)
    {
        UnZipTask unZipTask = new UnZipTask
        {
            SerialId = ++s_Serial,
            SourceFilePath = sourceFilePath,
            UnZipPath = unZipPath,
            UnZipType = unZipType,
            IsDeleteSourceFileAfterUnZip = isDeleteSourceFileAfterUnZip,
            UserData = userData
        };
        return unZipTask;
    }

    public void Clear()
    {
        Status = UnZipTaskStatus.Todo;
        SourceFilePath = null;
        UnZipPath = null;
        UnZipType = 0;
        IsDeleteSourceFileAfterUnZip = false;
        UserData = null;
    }
}

public enum UnZipTaskStatus
{
    Todo,
    Doing,
    Done,
    Error
}