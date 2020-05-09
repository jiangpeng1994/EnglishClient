using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>  
/// 文件(夹)压缩、解压缩  
/// </summary>  
public class ZipHelper
{
    /// <summary>   
    /// 压缩文件   
    /// </summary>   
    /// <param name="fileToZip">要压缩的文件全名</param>   
    /// <param name="zipedFile">压缩后的文件名</param>   
    /// <param name="password">密码</param>   
    /// <returns>压缩结果</returns>   
    public static bool ZipFile(string fileToZip, string zipedFile, string password)
    {
        bool result = true;
        ZipOutputStream zipStream = null;
        FileStream fs = null;
        ZipEntry ent = null;

        if (!File.Exists(fileToZip))
            return false;

        try
        {
            fs = File.Open(fileToZip,FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            fs = File.Create(zipedFile);
            zipStream = new ZipOutputStream(fs);
            if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
            ent = new ZipEntry(Path.GetFileName(fileToZip));
            zipStream.PutNextEntry(ent);
            zipStream.SetLevel(6);

            zipStream.Write(buffer, 0, buffer.Length);

        }
        catch(Exception e)
        {
            result = false;
            Debug.Log("压缩异常：" + e.ToString());
        }
        finally
        {
            if (zipStream != null)
            {
                zipStream.Finish();
                zipStream.Close();
            }
            if (ent != null)
            {
                ent = null;
            }
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }
        }
        GC.Collect();
        GC.Collect(1);

        return result;
    }

    #region 压缩文件  
    /// <summary>    
    /// 压缩文件    
    /// </summary>    
    /// <param name="fileNames">要打包的文件列表</param>    
    /// <param name="GzipFileName">目标文件名</param>    
    /// <param name="CompressionLevel">压缩品质级别（0~9）</param>    
    /// <param name="deleteFile">是否删除原文件</param>  
    public static void CompressFile(List<FileInfo> fileNames, string GzipFileName, int CompressionLevel, bool deleteFile)
    {
        ZipOutputStream s = new ZipOutputStream(File.Create(GzipFileName));
        try
        {
            s.SetLevel(CompressionLevel);   //0 - store only to 9 - means best compression    
            foreach (FileInfo file in fileNames)
            {
                FileStream fs = null;
                try
                {
                    fs = file.Open(FileMode.Open, FileAccess.ReadWrite);
                }
                catch
                { continue; }
                //  方法二，将文件分批读入缓冲区    
                byte[] data = new byte[2048];
                int size = 2048;
                ZipEntry entry = new ZipEntry(Path.GetFileName(file.Name));
                entry.DateTime = (file.CreationTime > file.LastWriteTime ? file.LastWriteTime : file.CreationTime);
                s.PutNextEntry(entry);
                while (true)
                {
                    size = fs.Read(data, 0, size);
                    if (size <= 0) break;
                    s.Write(data, 0, size);
                }
                fs.Close();
                if (deleteFile)
                {
                    file.Delete();
                }
            }
        }
        finally
        {
            s.Finish();
            s.Close();
        }
    }
    /// <summary>    
    /// 压缩文件夹    
    /// </summary>    
    /// <param name="dirPath">要打包的文件夹</param>  
    /// <param name="fileInfos">要压缩的文件</param>   
    /// <param name="GzipFileName">目标文件名</param>    
    /// <param name="CompressionLevel">压缩品质级别（0~9）</param>    
    /// <param name="deleteDir">是否删除原文件夹</param>  
    public static void CompressDirectory(string dirPath, FileInfo[] fileInfos, string GzipFileName, int CompressionLevel, bool deleteDir)
    {
        dirPath = Directory.GetParent(dirPath).FullName.Replace("\\", "/");

        using (ZipOutputStream zipoutputstream = new ZipOutputStream(File.Create(GzipFileName)))
        {
            zipoutputstream.SetLevel(CompressionLevel);
            Crc32 crc = new Crc32();
            Dictionary<string, DateTime> fileList = null;
            if (fileInfos == null)
            {
                fileList = GetAllFies(dirPath);
            }
            else
            {
                fileList = new Dictionary<string, DateTime>();
                foreach (FileInfo item in fileInfos)
                {
                    fileList.Add(item.FullName.Replace("\\", "/"), item.LastWriteTime);
                }
            }

            long index = 0;
            foreach (KeyValuePair<string, DateTime> item in fileList)
            {
                FileStream fs = File.OpenRead(item.Key.ToString());
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);

                ZipEntry entry = new ZipEntry(item.Key.Substring(dirPath.Length));
                entry.DateTime = item.Value;
                entry.Size = fs.Length;
                entry.ZipFileIndex = fileList.Count - index++;
                fs.Close();
                crc.Reset();
                crc.Update(buffer);
                entry.Crc = crc.Value;
                zipoutputstream.PutNextEntry(entry);
                zipoutputstream.Write(buffer, 0, buffer.Length);
            }
        }
        if (deleteDir)
        {
            Directory.Delete(dirPath, true);
        }
    }
    /// <summary>    
    /// 获取所有文件    
    /// </summary>    
    /// <returns></returns>    
    private static Dictionary<string, DateTime> GetAllFies(string dir)
    {
        Dictionary<string, DateTime> FilesList = new Dictionary<string, DateTime>();
        DirectoryInfo fileDire = new DirectoryInfo(dir);
        if (!fileDire.Exists)
        {
            throw new System.IO.FileNotFoundException("目录:" + fileDire.FullName + "没有找到!");
        }
        GetAllDirFiles(fileDire, FilesList);
        GetAllDirsFiles(fileDire.GetDirectories(), FilesList);
        return FilesList;
    }
    /// <summary>    
    /// 获取一个文件夹下的所有文件夹里的文件    
    /// </summary>    
    /// <param name="dirs"></param>    
    /// <param name="filesList"></param>    
    private static void GetAllDirsFiles(DirectoryInfo[] dirs, Dictionary<string, DateTime> filesList)
    {
        foreach (DirectoryInfo dir in dirs)
        {
            foreach (FileInfo file in dir.GetFiles("*.*"))
            {
                filesList.Add(file.FullName.Replace("\\", "/"), file.LastWriteTime);
            }
            GetAllDirsFiles(dir.GetDirectories(), filesList);
        }
    }
    /// <summary>    
    /// 获取一个文件夹下的文件    
    /// </summary>    
    /// <param name="dir">目录名称</param>    
    /// <param name="filesList">文件列表HastTable</param>    
    private static void GetAllDirFiles(DirectoryInfo dir, Dictionary<string, DateTime> filesList)
    {
        foreach (FileInfo file in dir.GetFiles("*.*"))
        {
            filesList.Add(file.FullName.Replace("\\", "/"), file.LastWriteTime);
        }
    }
    #endregion

    /// <summary>    
    /// 解压缩文件    
    /// </summary>    
    /// <param name="sourceFilePath">压缩包文件名</param>    
    /// <param name="unZipPath">解压缩目标路径</param>           
    public static void Decompress(string sourceFilePath, string unZipPath, ref ZipResult zipResult)
    {
        zipResult.Errors = false;
        try
        {
            if (!unZipPath.EndsWith("/"))
            {
                unZipPath = unZipPath + "/";
            }

            if (!Directory.Exists(unZipPath))
            {
                Directory.CreateDirectory(unZipPath);
            }

            byte[] data = new byte[2048];
            int size = 2048;

            ZipEntry zipEntry = null;
            using (ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(sourceFilePath)))
            {
                while ((zipEntry = zipInputStream.GetNextEntry()) != null)
                {
                    string dir = (unZipPath + zipEntry.Name).Replace("\\", "/");
                    string[] temp = dir.Split('/');
                    dir = dir.Replace(temp[temp.Length - 1], "");
                    DirectoryInfo di = new DirectoryInfo(dir);
                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    if (zipEntry.Name != String.Empty)
                    {
                        //  检查多级目录是否存在  
                        if (zipEntry.Name.Contains("//"))
                        {
                            string parentDirPath = zipEntry.Name.Remove(zipEntry.Name.LastIndexOf("//") + 1);
                            if (!Directory.Exists(parentDirPath))
                            {
                                Directory.CreateDirectory(unZipPath + parentDirPath);
                            }
                        }

                        //解压文件到指定的目录    
                        using (FileStream streamWriter = File.Create(unZipPath + zipEntry.Name))
                        {
                            while (true)
                            {
                                size = zipInputStream.Read(data, 0, data.Length);
                                if (size <= 0) break;
                                streamWriter.Write(data, 0, size);
                            }
                        }

						if (zipEntry.Name.EndsWith( ".dll"))
						{
                            Debug.Log("Has dll Content");
							zipResult.hasDllFile = true;
						}
                    }
                }
            }
        }
        catch(Exception e)
        {
            zipResult.Errors = true;
            //Debug.LogError("解压失败：" + e.ToString());
        }
    }

    /// <summary>
    /// 热更新文件解压。
    /// </summary>
    /// <param name="sourceFilePath">源文件。</param>
    /// <param name="unZipPath">解压路径。</param>
    /// <param name="zipResult">解压结果。</param>
    public static void HotUpdateDecompress(string sourceFilePath, string unZipPath, ref ZipResult zipResult)
    {
        zipResult.Errors = false;
        try
        {
            //UnityEngine.Debug.Log("开始解压");
            string directoryName = unZipPath;
            if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);//生成解压目录    
            string CurrentDirectory = directoryName;
            int buffsize = 1024 * 1024;
            byte[] data = new byte[buffsize];
            int size = buffsize;
            ZipEntry theEntry = null;

//            long zipFileIndex = 0;
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(sourceFilePath)))
            {
                StreamWriter sw;
                FileStream recordFs;
                /*if (File.Exists(AssetUpdateRecord.Instance.GetPath()))
                {
                    recordFs = File.Open(AssetUpdateRecord.Instance.GetPath(), FileMode.Append);
                }
                else
                {
                    recordFs = File.Open(AssetUpdateRecord.Instance.GetPath(), FileMode.Create);
                }

                sw = new StreamWriter(recordFs);
                */
                

                while ((theEntry = s.GetNextEntry()) != null)
                {
                    ////if (zipResult.FileCount < theEntry.ZipFileIndex)
                    ////{
                    ////    zipResult.FileCount = theEntry.ZipFileIndex;
                    ////}
                    string dir = (CurrentDirectory + theEntry.Name).Replace("\\", "/");
                    string[] temp = dir.Split('/');
                    dir = dir.Replace(temp[temp.Length - 1], "");
                    DirectoryInfo di = new DirectoryInfo(dir);
                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    if (theEntry.Name != String.Empty)
                    {
                        //  检查多级目录是否存在  
                        if (theEntry.Name.Contains("//"))
                        {
                            string parentDirPath = theEntry.Name.Remove(theEntry.Name.LastIndexOf("//") + 1);
                            if (!Directory.Exists(parentDirPath))
                            {
                                Directory.CreateDirectory(CurrentDirectory + parentDirPath);
                            }
                        }

                        //解压文件到指定的目录    
                        using (FileStream streamWriter = new FileStream(CurrentDirectory + theEntry.Name, FileMode.Create))
                        {
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size <= 0) break;
                                streamWriter.Write(data, 0, size);
                            }
                            streamWriter.Close();
                            //streamWriter.WriteLine(theEntry.Name);
                        }
                    }
                }
                //s.Close();
                s.Close();


                s.Close();
                zipResult.UnZipPercent = 1;

                //UnityEngine.Debug.Log("解压成功");
            }
        }
        catch (Exception ex)
        {
            zipResult.Errors = true;
        }

        try
        {
            //AssetUpdateRecord.Instance.GenData();
        }
        catch (Exception ex)
        {
            //PlayingDownloadLog.Instance.HotUnZipLog("AssetUpdateRecord.GenData() Error.\n\t" + ex);
        }
    }

    /// <summary>
    /// 普通文件解压。
    /// </summary>
    /// <param name="sourceFilePath">源文件路径。</param>
    /// <param name="unZipPath">解压路径。</param>
    /// <param name="zipResult">解压结果。</param>
    public static void NormalDecompress(string sourceFilePath, string unZipPath, ref ZipResult zipResult)
    {
        zipResult.UnZipPercent = 0;
        zipResult.Errors = false;
        try
        {
            int bufferLength = 2048;
            byte[] bufferData = new byte[bufferLength];
            
            // 读取源文件。
            using (FileStream sourceFileStream = File.OpenRead(sourceFilePath))
            {
                long totalLength = sourceFileStream.Length;
                long unZipLength = 0;
                ZipEntry zipEntry = null;
                // 文件流转换为压缩包输入流。
                using (ZipInputStream zipInputStream = new ZipInputStream(sourceFileStream))
                {
                    // 读取每一个需要被解压的文件。
                    while ((zipEntry = zipInputStream.GetNextEntry()) != null)
                    {
                        if (zipEntry.Name == String.Empty)
                        {
                            continue;
                        }

                        string targetPath = Path.Combine(unZipPath, zipEntry.Name);
                        // 如果是文件目录，且没有这个目录，则创建文件目录。
                        if (zipEntry.IsDirectory)
                        {
                            if (!Directory.Exists(targetPath))
                            {
                                Directory.CreateDirectory(targetPath);
                            }
                            continue;
                        }

                        // 如果是文件，则检测当前文件是否需要被解压。
                        bool isNeedUnZip = true;
                        if (File.Exists(targetPath))
                        {
                            // todo:是否需要判断存在的文件就是需要解压的问题，通过比较文件大小，如果是就可以不用解压，不是再判断是否热更的。
                            // 如果本地存在当前文件，则判断存在的文件是不是热更新的。
                            if (false)
                            {
                                // 如果存在的文件是热更新的，则不需要解压。(热更新的文件是最新的，不能将其覆盖了)
                                isNeedUnZip = false;
                            }
                            else
                            {
                                // 如果存在的文件不是热更新的，则需要解压，进行覆盖。
                                isNeedUnZip = true;
                            }
                        }
                        else
                        {
                            // 如果本地不存在当前文件，则需要解压。
                            isNeedUnZip = true;
                        }

                        // 如果当前文件需要被解压。
                        if (isNeedUnZip)
                        {
                            // 解压文件到指定的目录。    
                            using (FileStream streamWriter = File.Create(targetPath))
                            {
                                while (true)
                                {
                                    bufferLength = zipInputStream.Read(bufferData, 0, bufferData.Length);
                                    if (bufferLength <= 0)
                                        break;
                                    streamWriter.Write(bufferData, 0, bufferLength);
                                }
                            }
                        }
                        else
                        {
                            // 解压流跳过当前文件。
                        }

                        unZipLength = unZipLength + zipEntry.CompressedSize;
                        zipResult.UnZipPercent = (float)unZipLength / totalLength;
                    }
                }
            }
            zipResult.UnZipPercent = 1;
        }
        catch (Exception e)
        {
            zipResult.Errors = true;
            Debug.LogError("解压失败：" + e.ToString());
        }
    }
}

public class ZipResult
{
    /// <summary>
    /// 要压缩/解压的文件数
    /// </summary>
    public long FileCount = 0;

    /// <summary>
    /// 压缩百分比
    /// </summary>
    public int CompressionPercent = 0;
   
    /// <summary>
    /// 解压百分比
    /// </summary>
    public float UnZipPercent = 0;

    public bool Errors = false;

	/// <summary>
	/// Has dll content or not
	/// </summary>
	public bool hasDllFile = false;
}