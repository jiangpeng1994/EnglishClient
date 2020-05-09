using System;

public class DownloadHelperEventArgs : EventArgs
{
    private byte[] m_Bytes;

    public int Offset
    {
        get;
        private set;
    }

    public int Length
    {
        get;
        private set;
    }

    public float DownloadProgress
    {
        get;
        private set;
    }

    public string ErrorMessage
    {
        get;
        private set;
    }

    public int ErrorCode
    {
        get;
        private set;
    }

    public DownloadHelperEventArgs()
    {
        m_Bytes = null;
        Offset = 0;
        Length = 0;
        DownloadProgress = 0;
        ErrorMessage = null;
        ErrorCode = 0;
    }

    public static DownloadHelperEventArgs Create(byte[] bytes, int offset, int length, float downloadProgress)
    {
        if (bytes == null)
        {
            throw new Exception("Bytes is invalid.");
        }
        if (offset < 0 || offset >= bytes.Length)
        {
            throw new Exception("Offset is invalid.");
        }
        if (length <= 0 || offset + length > bytes.Length)
        {
            throw new Exception("Length is invalid.");
        }

        DownloadHelperEventArgs downloadAgentHelperUpdateBytesEventArgs = new DownloadHelperEventArgs
        {
            m_Bytes = bytes,
            Offset = offset,
            Length = length,
            DownloadProgress = downloadProgress
        };
        return downloadAgentHelperUpdateBytesEventArgs;
    }

    public static DownloadHelperEventArgs Create(int length)
    {
        if (length <= 0)
        {
            throw new Exception("Length is invalid.");
        }

        DownloadHelperEventArgs downloadAgentHelperUpdateBytesEventArgs = new DownloadHelperEventArgs
        {
            Length = length
        };
        return downloadAgentHelperUpdateBytesEventArgs;
    }

    public static DownloadHelperEventArgs Create(int errorCode, string errorMessage)
    {
        DownloadHelperEventArgs downloadAgentHelperUpdateBytesEventArgs = new DownloadHelperEventArgs
        {
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        };
        return downloadAgentHelperUpdateBytesEventArgs;
    }

    public void Clear()
    {
        m_Bytes = null;
        Offset = 0;
        Length = 0;
        DownloadProgress = 0;
        ErrorMessage = null;
        ErrorCode = 0;
    }

    public byte[] GetBytes()
    {
        return m_Bytes;
    }
}