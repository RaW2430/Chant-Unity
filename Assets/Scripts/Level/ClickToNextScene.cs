using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class ClickToNextScene : MonoBehaviour
{
    public string nextSceneName;

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void RequestMicrophoneAccess();
#endif

    void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        // 游戏一开始就请求麦克风权限
        RequestMicrophoneAccess();
#endif
    }

    void Update()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("请在 Inspector 设置 nextSceneName！");
        }
    }
}
