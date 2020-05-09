using System;

public sealed class DownloadEventArgs : EventArgs
{
    public int SerialId { get; private set; }
    public string DownloadUrl { get; private set; }
    public string DownloadPath { get; private set; }
    public int SavedLength { get; private set; }
    public string MD5 { get; private set; }
    public float DownloadProgress { get; private set; }
    public string ErrorMessage { get; private set; }
    public int ErrorCode { get; private set; }
    public object UserData { get; private set; }

    public DownloadEventArgs()
    {
        SerialId = 0;
        DownloadUrl = null;
        DownloadPath = null;
        SavedLength = 0;
        MD5 = null;
        DownloadProgress = 0;
        ErrorCode = 0;
        ErrorMessage = null;
        UserData = null;
    }

    public static DownloadEventArgs CreateStartEventArgs(int serialId, string downloadUrl, string downloadPath, int savedLength, object userData)
    {
        DownloadEventArgs downloadStartEventArgs = new DownloadEventArgs
        {
            SerialId = serialId,
            DownloadUrl = downloadUrl,
            DownloadPath = downloadPath,
            SavedLength = savedLength,
            UserData = userData
        };
        return downloadStartEventArgs;
    }

    public static DownloadEventArgs CreateUpdateEventArgs(int serialId, string downloadUrl, string downloadPath, int savedLength, float downloadProgress, object userData)
    {
        DownloadEventArgs downloadStartEventArgs = new DownloadEventArgs
        {
            SerialId = serialId,
            DownloadUrl = downloadUrl,
            DownloadPath = downloadPath,
            SavedLength = savedLength,
            DownloadProgress = downloadProgress,
            UserData = userData
        };
        return downloadStartEventArgs;
    }

    public static DownloadEventArgs CreateSuccessEventArgs(int serialId, string downloadUrl, string downloadPath, int savedLength, string md5, object userData)
    {
        DownloadEventArgs downloadStartEventArgs = new DownloadEventArgs
        {
            SerialId = serialId,
            DownloadUrl = downloadUrl,
            DownloadPath = downloadPath,
            SavedLength = savedLength,
            MD5 = md5,
            UserData = userData
        };
        return downloadStartEventArgs;
    }

    public static DownloadEventArgs CreateFailureEventArgs(int serialId, string downloadUrl, string downloadPath, int savedLength, int errorCode, string errorMessage, object userData)
    {
        DownloadEventArgs downloadStartEventArgs = new DownloadEventArgs
        {
            SerialId = serialId,
            DownloadUrl = downloadUrl,
            DownloadPath = downloadPath,
            SavedLength = savedLength,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode,
            UserData = userData
        };
        return downloadStartEventArgs;
    }

    public void Clear()
    {
        SerialId = 0;
        DownloadUrl = null;
        DownloadPath = null;
        SavedLength = 0;
        MD5 = null;
        DownloadProgress = 0;
        ErrorCode = 0;
        ErrorMessage = null;
        UserData = null;
    }
}