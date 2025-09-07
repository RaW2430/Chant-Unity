using System.Collections;
using UnityEngine;

public class SawGenerator : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject sawPrefab;      // 齿轮
    public float spawnInterval = 2f;  // 生成间隔

    [Header("运动参数")]
    public float moveSpeed = 3f;      // 移动速度
    public float rotationSpeed = 180f;// 旋转速度

    [Header("屏幕边界（世界坐标）")]
    public float spawnX = 10f;        // 生成X坐标
    public float spawnY = 5f;         // 上下边界
    public float targetX = -10f;      
    public float targetY = 5f;        // 上下边界

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnSaw();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnSaw()
    {
        bool spawnTop = Random.value > 0.5f;
        Vector3 spawnPos = new Vector3(spawnX, spawnTop ? spawnY : -spawnY, 0);
        Vector3 targetPos = new Vector3(targetX, spawnTop ? -targetY : targetY, 0);
        Vector3 moveDir = (targetPos - spawnPos).normalized;
        GameObject saw = Instantiate(sawPrefab, spawnPos, Quaternion.identity);
        saw.AddComponent<SawBehavior>().Init(moveSpeed, rotationSpeed, moveDir);
    }
}

class SawBehavior : MonoBehaviour
{
    private float moveSpeed;
    private float rotationSpeed;
    private Vector3 moveDirection;

    public void Init(float moveSpeed, float rotationSpeed, Vector3 moveDirection)
    {
        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;
        this.moveDirection = moveDirection;
    }

    void Update()
    {
        // 对角移动
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // 自转
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        // GC
        if (Mathf.Abs(transform.position.x) > 30f || Mathf.Abs(transform.position.y) > 20f)
        {
            Destroy(gameObject);
        }
    }
}
