using LZ4;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Threading;
using UnityEngine;

public class LZ4Helper
{
    public static bool CompressDirectory(string dirPath, FileInfo[] fileInfos, string zipFileName, bool deleteDir)
    {
        var dirInfo = new DirectoryInfo(dirPath);
        dirPath = dirInfo.FullName.Replace("\\", "/");
        ZipInfo zipInfo = ZipInfo.Get();
        if (fileInfos == null)
        {
            GenerateZipInfo(dirPath, dirPath, zipInfo);
        }
        else
        {
            zipInfo = GenerateZipInfo(dirPath, fileInfos);
        }
        if (zipInfo == null)
            return false;
        
        using (var fileStream = new FileStream(zipFileName, FileMode.Create))
        using (var lz4Stream = new LZ4Stream(fileStream, LZ4StreamMode.Compress))
        {
            // 先写入zip文件的信息
            byte[] buff;
            using (var ms = new MemoryStream())
            {
                IFormatter iFormatter = new BinaryFormatter();
                iFormatter.Serialize(ms, zipInfo);
                buff = ms.GetBuffer();
            }

            int zipInfoLength = buff.Length;
            byte[] intBuff = BitConverter.GetBytes(zipInfoLength);     // 将 int 转换成字节数组
            lz4Stream.Write(intBuff, 0, intBuff.Length);
            lz4Stream.Write(buff, 0, buff.Length);

            for (int i = 0; i < zipInfo.allFileNames.Count; ++i)
            {
                var singleFileStream = new FileStream(dirPath + zipInfo.allFileNames[i], FileMode.Open);
                BinaryReader br = new BinaryReader(singleFileStream);
                System.Byte[] byData = br.ReadBytes((int)singleFileStream.Length);
                singleFileStream.Close();
                lz4Stream.Write(byData, 0, byData.Length);
            }
        }
        ZipInfo.Recovery(zipInfo);
        return true;
    }

	public static void HotUpdateDecompress(string zipPath, string targetPath, ref ZipResult zipResult)
    {
        zipResult.Errors = false;

        try
        {
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);//生成解压目录 

            var buffer = new byte[1024 * 1024 * 20]; // 申请一个20M的buff
            //var buffer = new byte[1024];
            var intBuff = new byte[4];
            using (var finput = File.OpenRead(zipPath))
            using (var zinput = new LZ4Stream(finput, LZ4StreamMode.Decompress))
            {
                // 先读取zipInfo
                ZipInfo newZipInfo = new ZipInfo();//多线程的，先直接new吧
                zinput.Read(intBuff, 0, 4);
                int zipInfoLength = bytesToInt(intBuff, 0);
                int leftSize = zipInfoLength;
                MemoryStream ms = new MemoryStream();
                while (true)
                {
                    if (leftSize > buffer.Length)
                    {//buffer一次读不完
                        var size = zinput.Read(buffer, 0, buffer.Length);
                        ms.Write(buffer, 0, size);
                        leftSize = leftSize - size;
                    }
                    else
                    {
                        var size = zinput.Read(buffer, 0, leftSize);
                        ms.Write(buffer, 0, size);
                        leftSize = leftSize - size;
                        if (leftSize != 0)
                            Debug.LogError("wtf??");
                        break;
                    }
                }
                ms.Position = 0;

                IFormatter iFormatter = new BinaryFormatter();
                newZipInfo = (ZipInfo)iFormatter.Deserialize(ms);
                //zipInfo读取完成

                ms.Dispose();

                for (int i = 0; i < newZipInfo.allFileSizes.Count; ++i)
                {
                    //目录
                    string dir = (targetPath + newZipInfo.allFileNames[i]).Replace("\\", "/");
                    string[] temp = dir.Split('/');
                    dir = dir.Replace(temp[temp.Length - 1], "");
                    DirectoryInfo di = new DirectoryInfo(dir);
                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    if (newZipInfo.allFileNames[i] != string.Empty)
                    {
                        //多级目录的，先把目录创建好
                        if (newZipInfo.allFileNames[i].Contains("//"))
                        {
                            string parentDirPath = newZipInfo.allFileNames[i].Remove(newZipInfo.allFileNames[i].LastIndexOf("//") + 1);
                            if (!Directory.Exists(parentDirPath))
                            {
                                Directory.CreateDirectory(targetPath + parentDirPath);
                            }
                        }

                        string tmpPath = targetPath + newZipInfo.allFileNames[i];
                        //检测文件是否存在。若存在，检测大小是否一致。若大小不一致，检测是否热更版本。
                        bool needOverWrite = !File.Exists(tmpPath);
                        if (!needOverWrite)
                        {//文件存在.检测大小
                            var fileInfo = new FileInfo(tmpPath);
                            if (fileInfo.Length == newZipInfo.allFileSizes[i])
                            {
                                needOverWrite = false;
                            }
                            else
                            {//文件大小不一致。检测是否热更版本。
                             //非热更文件，需要重写
                                //needOverWrite = !AssetUpdateRecord.Instance.CheckFile(newZipInfo.allFileNames[i].Remove(0, 1));
                            }
                        }

                        if (needOverWrite)
                        {
                            for (int j = 0; j < 10; j++)
                            {
                                var allSize = 0; // 用来回退异常时的游标
                                try
                                {
                                    using (var fileStream = new FileStream(targetPath + newZipInfo.allFileNames[i], FileMode.Create))
                                    {
                                        leftSize = newZipInfo.allFileSizes[i];
                                        while (true)
                                        {
                                            if (leftSize > buffer.Length)
                                            {//buffer一次读不完
                                                int size = zinput.Read(buffer, 0, buffer.Length);
                                                allSize += size;
                                                fileStream.Write(buffer, 0, size);
                                                leftSize = leftSize - size;
                                            }
                                            else
                                            {
                                                int size = zinput.Read(buffer, 0, leftSize);
                                                allSize += size;
                                                fileStream.Write(buffer, 0, size);
                                                leftSize = leftSize - size;
                                                if (leftSize != 0)
                                                    Debug.LogError("wtf??");
                                                break;
                                            }
                                        }

                                        fileStream.Close();
                                        //正常情况跳出 for 循环
                                        break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Thread.Sleep(500);

                                    //write 出现异常，重试时把 zinput 的游标恢复到读取之前的位置 
                                    if (allSize > 0)
                                    {
                                        zinput.Seek(-allSize, SeekOrigin.Current);
                                    }

                                    if (j == 9)
                                    {
                                        throw;
                                    }
                                }
                            }

                            if (newZipInfo.allFileNames[i].EndsWith(".dll"))
                            {
                                //UnityEngine.Debug.Log("Has dll Content");
                                zipResult.hasDllFile = true;
                            }
                        }
                        else
                        {
                            //不覆盖的时候会跳过读取，此时移动一下游标
                            leftSize = newZipInfo.allFileSizes[i];
                            while (true)
                            {
                                if (leftSize > buffer.Length)
                                {//buffer一次读不完
                                    int size = zinput.Read(buffer, 0, buffer.Length);
                                    leftSize = leftSize - size;
                                }
                                else
                                {
                                    int size = zinput.Read(buffer, 0, leftSize);
                                    leftSize = leftSize - size;
                                    if (leftSize != 0)
                                        Debug.LogError("wtf??");
                                    break;
                                }
                            }

                            if (newZipInfo.allFileSizes.Count != 0)
                            {
                                zipResult.UnZipPercent = 1f * (i+1) / newZipInfo.allFileSizes.Count;
                            }

                        }
                    }
                }

                zipResult.UnZipPercent = 1;
            }
        }
        catch (Exception ex)
        {
            zipResult.Errors = true;
        }
    }
	
    static ZipInfo GenerateZipInfo(string dirPath, FileInfo[] fileInfos)
    {
        if (fileInfos == null)
            return null;
        var zipInfo = ZipInfo.Get();
        foreach (FileInfo item in fileInfos)
        {
            zipInfo.allFileNames.Add(item.FullName.Replace("\\", "/").Substring(dirPath.Length));
            zipInfo.allFileSizes.Add((int)item.Length);
        }

        return zipInfo;
    }

    static void GenerateZipInfo(string rootPath, string dirPath, ZipInfo zipInfo)
    {
        DirectoryInfo fileDire = new DirectoryInfo(dirPath);
        if (!fileDire.Exists)
        {
            throw new System.IO.FileNotFoundException("目录:" + fileDire.FullName + "没有找到!");
        }
        
        foreach (FileInfo file in fileDire.GetFiles("*.*"))
        {
            zipInfo.allFileNames.Add(file.FullName.Replace("\\", "/").Substring(rootPath.Length));
            zipInfo.allFileSizes.Add((int)file.Length);
        }

        foreach (DirectoryInfo dir in fileDire.GetDirectories())
        {
            GenerateZipInfo(rootPath, dir.FullName, zipInfo);
        }
    }

    public static bool GenerateRecordFile(string dirPath, FileInfo[] fileInfos)
    {
        ZipInfo zipInfo = ZipInfo.Get();
        if (fileInfos == null)
        {
            GenerateZipInfo(dirPath, dirPath, zipInfo);
        }
        else
        {
            zipInfo = GenerateZipInfo(dirPath, fileInfos);
        }
        if (zipInfo == null)
            return false;

        //AssetUpdateRecord.Instance.StartWriteRecord();
        for (int i = 0; i < zipInfo.allFileSizes.Count; ++i)
        {
            //AssetUpdateRecord.Instance.WriteRecord(zipInfo.allFileNames[i]);
        }
        //AssetUpdateRecord.Instance.EndWriteRecord();
        return true;
    }

    public static int bytesToInt(byte[] src, int offset)
    {
        int value;
        value = (int)((src[offset] & 0xFF)
                | ((src[offset + 1] & 0xFF) << 8)
                | ((src[offset + 2] & 0xFF) << 16)
                | ((src[offset + 3] & 0xFF) << 24));
        return value;
    }
}
