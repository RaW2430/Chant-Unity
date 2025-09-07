using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpForce = 5.0f;
    public float volumeThreshold = 0.12f; // 音量阈值
    public float attackVolumeTreshold = 0.22f;   

    private Rigidbody2D rb;

    private bool canJump = true;    
    private float jumpCooldown = 0.1f; // 跳跃冷却时间

    public bool isGM = false;

    // Editor变量
#if UNITY_EDITOR
    private AudioClip microphoneClip;
    private string microphoneName;
    private const int sampleWindow = 128;
#endif

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.name = "Player"; 

        // 平台初始化
#if UNITY_EDITOR
        InitEditorMicrophone();
#elif UNITY_WEBGL
        Debug.Log("WebGL模式：开始初始化音量检测");
        InitVolumeDetection();
        SetVolumeThreshold(volumeThreshold);
        SetAttackVolumeThreshold(attackVolumeTreshold);
        Debug.Log($"WebGL模式：jump 已设置阈值为 {volumeThreshold}");
        Debug.Log($"WebGL模式：attack 已设置阈值为 {attackVolumeTreshold}");
#endif
    }

    void Update()
    {
#if UNITY_EDITOR
        CheckEditorVolume();
#endif

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
            Debug.Log("Jump by Space");
        }
        if (Input.GetMouseButtonDown(0))
        {
            Jump();
            Debug.Log("Jump by Click");
        }
    }

    void Jump()
    {
        if (!canJump)
        {
            return;
        }
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        StartCoroutine(JumpCooldownCoroutine());
    }

    // 消除陷阱
    void Attack()
    {
        GameObject[] saws = GameObject.FindGameObjectsWithTag("Saw");
        foreach (var saw in saws) Destroy(saw);
    }

    IEnumerator JumpCooldownCoroutine()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    // 声音触发跳跃接口（JS调用）
    public void TriggerJumpBySound()
    {
        Jump();
        Debug.Log("Jump by Sound"); 
    }
    public void TriggerAttackBySound()
    {
        Attack();
        Debug.Log("Attack by Sound"); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Saw"))
        {
            if (isGM) 
            {
                return;
            }

            Debug.Log("Game Over!");
            //Time.timeScale = 0f;
            GameEvents.OnGameOver?.Invoke();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (isGM)
        {
            return;
        }

        if (other.CompareTag("ScoreTrigger"))
        {
            Debug.Log("Get One!");
            GameEvents.OnScoreChanged?.Invoke(1);
        }
    }

    #region 编辑器模式麦克风处理（保持不变）
#if UNITY_EDITOR
    private void InitEditorMicrophone()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("未检测到麦克风");
            return;
        }
        microphoneName = Microphone.devices[0];
        microphoneClip = Microphone.Start(microphoneName, true, 1, 44100);
        Debug.Log("编辑器麦克风启动");
    }

    private void CheckEditorVolume()
    {
        if (string.IsNullOrEmpty(microphoneName) || !Microphone.IsRecording(microphoneName))
            return;

        float[] samples = new float[sampleWindow];
        int micPosition = Microphone.GetPosition(microphoneName) - (sampleWindow + 1);
        if (micPosition < 0) return;

        microphoneClip.GetData(samples, micPosition);
        float sum = 0;
        foreach (float sample in samples) sum += sample * sample;
        float volume = Mathf.Sqrt(sum / sampleWindow);

        if(volume >= volumeThreshold)
        {
            Debug.Log($"player volume: {volume}");
        }
        

        if (volume > volumeThreshold)
            TriggerJumpBySound();
        if (volume > attackVolumeTreshold)
            TriggerAttackBySound();
    }
#endif
    #endregion

    #region WebGL与JS交互
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void InitVolumeDetection();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetVolumeThreshold(float threshold);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetAttackVolumeThreshold(float threshold);
    #endregion

    void OnDestroy()
    {
#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(microphoneName) && Microphone.IsRecording(microphoneName))
            Microphone.End(microphoneName);
#endif
    }
}
