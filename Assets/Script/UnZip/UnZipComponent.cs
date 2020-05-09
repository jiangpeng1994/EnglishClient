using UnityEngine;

/// <summary>
/// 解压组件。
/// </summary>
public class UnZipComponent : MonoBehaviour
{
    /// <summary>
    /// 解压管理器。
    /// </summary>
    private UnZipManager m_UnZipManager = null;

    /// <summary>
    /// 获取解压是否被暂停。
    /// </summary>
    public bool Paused
    {
        get
        {
            return m_UnZipManager.Paused;
        }
    }

    /// <summary>
    /// 解压组件初始化。
    /// </summary>
    private void Awake()
    {
        m_UnZipManager = new UnZipManager();
        m_UnZipManager.UnZipStart += OnUnZipStart;
        m_UnZipManager.UnZipUpdate += OnUnZipUpdate;
        m_UnZipManager.UnZipSuccess += OnUnZipSuccess;
        m_UnZipManager.UnZipFailure += OnUnZipFailure;
    }

    /// <summary>
    /// 解压组件循环。
    /// </summary>
    private void Update()
    {
        if (m_UnZipManager == null)
        {
            return;
        }

        m_UnZipManager.Update(Time.deltaTime, Time.unscaledDeltaTime);
    }

    /// <summary>
    /// 增加解压代理。
    /// </summary>
    public void CreateUnZipAgent(int tpye)
    {
        if (tpye == 1)
        {
            // 创建队列解压型的代理(多个任务依次解压，解压完成一个后再开始下一个)
            m_UnZipManager.CreateQueueUnZipAgent();
        } else
        {
            // 创建并行解压型的代理(多个任务并行解压)
        }
    }

    /// <summary>
    /// 增加解压任务。
    /// </summary>
    /// <param name="sourceFilePath">解压源文件。</param>
    /// <param name="unZipPath">解压后存放路径。</param>
    /// <returns>新增解压任务的序列编号。</returns>
    public int AddUnZipTask(string sourceFilePath, string unZipPath)
    {
        return m_UnZipManager.AddUnZipTask(sourceFilePath, unZipPath);
    }

    /// <summary>
    /// 增加解压任务。
    /// </summary>
    /// <param name="sourceFilePath">解压源文件。</param>
    /// <param name="unZipPath">解压后存放路径。</param>
    /// <param name="unZipType">解压类型（默认值为0）。1：热更新解压 0：普通解压 </param>
    /// <param name="isDeleteSourceFileAfterUnZip">解压后是否删除源文件（默认值为false）。</param>
    /// <returns>新增解压任务的序列编号。</returns>
    public int AddUnZipTask(string sourceFilePath, string unZipPath, int unZipType, bool isDeleteSourceFileAfterUnZip)
    {
        return m_UnZipManager.AddUnZipTask(sourceFilePath, unZipPath, unZipType, isDeleteSourceFileAfterUnZip);
    }

    /// <summary>
    /// 移除解压任务。
    /// </summary>
    /// <param name="serialId">要移除解压任务的序列编号。</param>
    public void RemoveUnZipTask(int serialId)
    {
        m_UnZipManager.RemoveUnZipTask(serialId);
    }

    /// <summary>
    /// 移除所有解压任务。
    /// </summary>
    public void RemoveAllUnZipTasks()
    {
        m_UnZipManager.RemoveAllUnZipTasks();
    }

    /// <summary>
    /// 开始解压
    /// </summary>
    public void StartUnZip()
    {
        m_UnZipManager.StartUnZip();
    }

    /// <summary>
    /// 暂停解压
    /// </summary>
    public void PauseUnZip()
    {
        m_UnZipManager.PauseUnZip();
    }

    /// <summary>
    /// 应用层的回调：任务开始解压。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void OnUnZipStart(object sender, UnZipEventArgs e)
    {
        GlobalEvent.DispatchEvent("MSG_ON_UNZIP_START", e);
    }

    /// <summary>
    ///  应用层的回调：任务解压的数据更新。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void OnUnZipUpdate(object sender, UnZipEventArgs e)
    {
        GlobalEvent.DispatchEvent("MSG_ON_UNZIP_UPDATE", e);
    }

    /// <summary>
    ///  应用层的回调：任务解压完成。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void OnUnZipSuccess(object sender, UnZipEventArgs e)
    {
        GlobalEvent.DispatchEvent("MSG_ON_UNZIP_SUCCESS", e);
    }

    /// <summary>
    /// 应用层的回调：任务解压失败。
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">事件参数</param>
    private void OnUnZipFailure(object sender, UnZipEventArgs e)
    {
        GlobalEvent.DispatchEvent("MSG_ON_UNZIP_FAILURE", e);
    }
}