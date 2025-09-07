using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("生成参数")]
    public float speed = 5f;          // 移动速度
    public float spawnRate = 2f;      // 生成频率（秒）
    public float xSpawnPos = 10f;     // X坐标
    public float minY = -2f;          // Y坐标
    public float maxY = 2f;           

    [Header("预制体")]
    public GameObject obstaclePrefab; 

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnObstacle();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    void SpawnObstacle()
    {
        float centerY = Random.Range(minY, maxY);

        // 生成障碍物
        GameObject obstacle = Instantiate(
            obstaclePrefab,
            new Vector3(xSpawnPos, centerY, 0f),
            Quaternion.identity);

        // 设置移动速度
        ObstacleMoverV2 mover = obstacle.GetComponent<ObstacleMoverV2>();
        if (mover == null)
        {
            mover = obstacle.AddComponent<ObstacleMoverV2>();
        }
        mover.speed = speed;
    }
}

public class ObstacleMoverV2 : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // 超出左边界销毁
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}
