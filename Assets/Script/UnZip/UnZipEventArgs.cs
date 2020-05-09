using System;

public sealed class UnZipEventArgs : EventArgs
{
    public int SerialId { get; private set; }
    public string SourceFilePath { get; private set; }
    public string UnZipPath { get; private set; }
    public int UnZipType { get; private set; }
    public float UnZipProgress { get; private set; }
    public string ErrorMessage { get; private set; }
    public int ErrorCode { get; private set; }
    public object UserData { get; private set; }

    public UnZipEventArgs()
    {
        SerialId = 0;
        SourceFilePath = null;
        UnZipPath = null;
        UnZipType = 0;
        UnZipProgress = 0;
        ErrorCode = 0;
        ErrorMessage = null;
        UserData = null;
    }

    public static UnZipEventArgs CreateStartEventArgs(int serialId, string sourceFilePath, string unZipPath, int unZipType, object userData)
    {
        UnZipEventArgs unZipStartEventArgs = new UnZipEventArgs
        {
            SerialId = serialId,
            SourceFilePath = sourceFilePath,
            UnZipPath = unZipPath,
            UnZipType = unZipType,
            UserData = userData
        };
        return unZipStartEventArgs;
    }

    public static UnZipEventArgs CreateUpdateEventArgs(int serialId, string sourceFilePath, string unZipPath, int unZipType, float unZipProgress, object userData)
    {
        UnZipEventArgs unZipStartEventArgs = new UnZipEventArgs
        {
            SerialId = serialId,
            SourceFilePath = sourceFilePath,
            UnZipPath = unZipPath,
            UnZipType = unZipType,
            UnZipProgress = unZipProgress,
            UserData = userData
        };
        return unZipStartEventArgs;
    }

    public static UnZipEventArgs CreateSuccessEventArgs(int serialId, string sourceFilePath, string unZipPath, int unZipType, object userData)
    {
        UnZipEventArgs unZipStartEventArgs = new UnZipEventArgs
        {
            SerialId = serialId,
            SourceFilePath = sourceFilePath,
            UnZipPath = unZipPath,
            UnZipType = unZipType,
            UserData = userData
        };
        return unZipStartEventArgs;
    }

    public static UnZipEventArgs CreateFailureEventArgs(int serialId, string sourceFilePath, string unZipPath, int unZipType, int errorCode, string errorMessage, object userData)
    {
        UnZipEventArgs unZipStartEventArgs = new UnZipEventArgs
        {
            SerialId = serialId,
            SourceFilePath = sourceFilePath,
            UnZipPath = unZipPath,
            UnZipType = unZipType,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode,
            UserData = userData
        };
        return unZipStartEventArgs;
    }

    public void Clear()
    {
        SerialId = 0;
        SourceFilePath = null;
        UnZipPath = null;
        UnZipType = 0;
        UnZipProgress = 0;
        ErrorCode = 0;
        ErrorMessage = null;
        UserData = null;
    }
}