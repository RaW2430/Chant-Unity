using System.Collections;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [Header("参数设置")]
    public float speed = 5f;          // 障碍物移动速度
    public float spawnRate = 2f;      // 生成频率（秒）
    public float obstacleGap = 2f;    // 上下间距
    public float xSpawnPos = 10f;     // 生成位置的X坐标
    public float minY = -2f;          // 随机Y范围最小值
    public float maxY = 2f;           // 随机Y范围最大值

    [Header("预制体")]
    public Transform obstacleUp;      // 上方障碍物
    public Transform obstacleDown;    // 下方障碍物

    void Start()
    {
        // 启动协程，循环生成障碍物
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnObstaclePair();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    void SpawnObstaclePair()
    {
        // 随机一个中心Y
        float centerY = Random.Range(minY, maxY);

        // 生成下方障碍物
        Transform down = Instantiate(
            obstacleDown,
            new Vector3(xSpawnPos, centerY - obstacleGap / 2f, 0f),
            Quaternion.identity);
        down.gameObject.AddComponent<ObstacleMover>().speed = speed;

        // 生成上方障碍物
        Transform up = Instantiate(
            obstacleUp,
            new Vector3(xSpawnPos, centerY + obstacleGap / 2f, 0f),
            Quaternion.identity);
        up.gameObject.AddComponent<ObstacleMover>().speed = speed;
    }
}

/// <summary>
/// 控制障碍物向左移动
/// </summary>
public class ObstacleMover : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}
