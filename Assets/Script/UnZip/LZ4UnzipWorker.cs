public class LZ4UnzipWorker 
{
	private string _zipFilePath;
	private string _targetPath;
	private int _type = 0;
	private ZipResult _zipResult;
    
	public LZ4UnzipWorker(string gzipFilePath, string targetPath, int type = 0)
	{
		_zipFilePath = gzipFilePath;
		_targetPath = targetPath;
		_type = type;
		_zipResult = new ZipResult();
	}

	public void StartUnzipByType()
    {
        switch (_type)
        {
            case 0 :
                // 普通文件Zip解压。
                ZipHelper.NormalDecompress(_zipFilePath, _targetPath, ref _zipResult);
                break;
            case 1 :
                // 热更新文件Zip解压。
                ZipHelper.HotUpdateDecompress(_zipFilePath, _targetPath, ref _zipResult);
                break;
            case 2:
                // 普通文件LZ4解压。
                LZ4Helper.HotUpdateDecompress(_zipFilePath, _targetPath, ref _zipResult);
                break;
            case 3:
                // 热更新文件LZ4解压。
                LZ4Helper.HotUpdateDecompress(_zipFilePath, _targetPath, ref _zipResult);
                break;
        }
    }

	public ZipResult GetUnzipResult()
	{
		return _zipResult;
	}
}