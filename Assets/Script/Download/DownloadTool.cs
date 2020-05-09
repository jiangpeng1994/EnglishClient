using System.IO;
using UnityEngine;

public class DownloadTool
{
    /// <summary>
    /// 1MB = (1024 * 1024)B
    /// </summary>
    public static readonly float OneMegaBytes = 1048576;
    
    /// <summary>
    /// 1KB = 1024B
    /// </summary>
    public static readonly float OneKiloBytes = 1024;

    /// <summary>
    /// 字节单位转换为千字节/兆字节
    /// </summary>
    /// <param name="dataSize">字节大小</param>
    /// <returns></returns>
    public static string ByteUnitConversion(float dataSize)
    {
        string size = "";
        if (dataSize >= OneMegaBytes)
        {
            size = ((double)dataSize / OneMegaBytes).ToString("0.00") + "MB";
        }
        else
        {
            size = ((double)dataSize / OneKiloBytes).ToString("0.00") + "KB";
        }
        return size;
    }

    /// <summary>
    /// 设备可用存储大小是否足够
    /// </summary>
    /// <param name="needSize">需要的存储大小</param>
    /// <returns>设备可用存储大小是否足够</returns>
    public static bool IsTheAvailableStorageSizeEnough(long needSize)
    {
        return true;
    }

    /// <summary>
    /// 校验文件的MD5
    /// </summary>
    /// <param name="targetFilePath">目标文件路径</param>
    /// <param name="md5">校验的md5</param>
    /// <returns></returns>
    public static bool CheckFileMd5(string targetFilePath, string md5)
    {
        bool ret = false;

        if (File.Exists(targetFilePath))
        {
            string targetFileMd5 = FileUtils.getFileMd5(targetFilePath);
            if (md5.Equals(targetFileMd5))
            {
                ret = true;
            }
        }

        return ret;
    }
}