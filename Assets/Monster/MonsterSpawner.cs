using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // 스폰할 몬스터 프리팹
    public float spawnInterval = 2f; // 스폰 간격 (초)
    public float spawnRangeX = 10f; // X축 스폰 범위
    public float spawnRangeY = 10f; // Y축 스폰 범위
    public int maxMonsters = 20; // 최대 몬스터 수

    private float spawnTimer;
    private int currentMonsterCount;

    void Start()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f && currentMonsterCount < maxMonsters)
        {
            SpawnMonster();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnMonster()
    {
        if (monsterPrefab == null) return;

        // 랜덤 위치 생성
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX),
            Random.Range(-spawnRangeY, spawnRangeY),
            0f
        );

        // 몬스터 생성
        GameObject monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        currentMonsterCount++;

        // 몬스터가 파괴될 때 카운트 감소
        Monster monsterScript = monster.GetComponent<Monster>();
        if (monsterScript != null)
        {
            // 몬스터가 파괴될 때 이벤트 처리를 위해 MonoBehaviour.Destroy 대신 커스텀 처리 필요
        }
    }

    // 몬스터가 죽었을 때 호출 (Monster.cs의 Die()에서 호출)
    public void OnMonsterDied()
    {
        currentMonsterCount--;
    }

    // 디버그용: 스폰 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRangeX * 2, spawnRangeY * 2, 0));
    }
}
