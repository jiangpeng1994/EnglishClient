public sealed class DownloadCounterNode
{
    public int DownloadedLength { get; private set; }

    public float ElapseSeconds { get; private set; }

    public DownloadCounterNode()
    {
        DownloadedLength = 0;
        ElapseSeconds = 0f;
    }

    public static DownloadCounterNode Create(int downloadedLength)
    {
        DownloadCounterNode downloadCounterNode = new DownloadCounterNode();
        downloadCounterNode.DownloadedLength = downloadedLength;
        return downloadCounterNode;
    }

    public void Update(float elapseSeconds, float realElapseSeconds)
    {
        ElapseSeconds += realElapseSeconds;
    }

    public void AddDownloadedLength(int downloadedLength)
    {
        DownloadedLength += downloadedLength;
    }

    public void Clear()
    {
        DownloadedLength = 0;
        ElapseSeconds = 0f;
    }
}