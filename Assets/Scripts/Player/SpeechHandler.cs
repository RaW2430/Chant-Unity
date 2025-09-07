using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;

public class SpeechHandler : MonoBehaviour
{
    [Header("语音识别状态")]
    public TextMeshProUGUI text1; // 显示 success
    public TextMeshProUGUI text2; // 显示识别文本

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void InitEnglishSpeech();
#endif

    void Start()
    {
        if (text1 == null || text2 == null)
        {
            Debug.LogError("请为 text1 和 text2 分配 TextMeshPro 组件!");
            return;
        }

        text1.text = "Click anywhere to start...";
        text2.text = "";

#if UNITY_WEBGL && !UNITY_EDITOR
        InitEnglishSpeech();
#else
        text1.text = "请在 WebGL 构建中测试";
#endif
    }

    // JS 回调：检测到声音
    public void OnSpeechResultText1(string result)
    {
        text1.text = result;
    }

    // JS 回调：识别文本
    public void OnSpeechResultText2(string result)
    {
        text2.text = result;
    }
}
