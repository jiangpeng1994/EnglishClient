
using System.Collections.Generic;
using System;

[Serializable]
public class ZipInfo 
{
    public List<string> allFileNames = new List<string>();
    public List<int> allFileSizes = new List<int>();

    public void Clear()
    {
        allFileSizes.Clear();
        allFileNames.Clear();
    }

    #region pool  --对象池
    private static SelfObjectPool<ZipInfo> mZipInfoPool = new SelfObjectPool<ZipInfo>(5);
    public static ZipInfo Get()
    {
        var ret = mZipInfoPool.CreateObject<ZipInfo>();
        ret.Clear();
        return ret;
    }

    public static void Recovery(ZipInfo zipInfo)
    {
        mZipInfoPool.RecoverObject(zipInfo);
    }
    #endregion
}
