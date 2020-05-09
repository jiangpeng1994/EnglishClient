using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ResourceLoader : MonoBehaviour
{
    public static ResourceLoader Instance;
    private string _TextureRootURL;

    public void Awake()
    {
        Instance = this;
        _TextureRootURL = Application.persistentDataPath + "/Resources/";
    }

    public void GetTextureResources(UITexture uITexture, string path)
    {
        uITexture.mainTexture = null;
        //Debug.Log("图片路径：" + _TextureRootURL);
        Texture tex = Resources.Load<Texture>(path);
        if (tex == null)
        {
            StartCoroutine(LoadTexture(path, uITexture));
        }
        else
        {
            uITexture.mainTexture = tex;
        }
    }

    private IEnumerator LoadTexture(string path, UITexture uITexture)
    {
        string url = "file://" + _TextureRootURL + path;
        Debug.LogWarning("图片地址："+url);
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();
            try
            {
                uITexture.mainTexture = DownloadHandlerTexture.GetContent(uwr);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}