using UnityEngine;

public class Monster : MonoBehaviour
{
    private GameObject PlayerObject;
    [Tooltip("몬스터의 이동 속도")]
    public float moveSpeed = 3f;
    [Tooltip("플레이어로부터 멈출 거리")]
    public float stopDistance = 1f; // 멈출 거리
    [Tooltip("몬스터가 죽을 때 드롭할 경험치 아이템 프리팹")]
    public GameObject expItemPrefab; // ExpItem 프리팹

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            PlayerObject = player.gameObject;
        }

        // TODO : 임시이기 떄문에 수정 필요
        // 3초 후에 Die() 호출
        Invoke("Die", 3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerObject != null)
        {
            // target까지의 거리 계산
            float distance = Vector3.Distance(transform.position, PlayerObject.transform.position);

            // 목표 거리보다 멀 때만 이동
            if (distance > stopDistance)
            {
                // target을 향한 방향 계산
                Vector3 direction = (PlayerObject.transform.position - transform.position).normalized;

                // target을 향해 이동
                transform.position += direction * moveSpeed * Time.deltaTime;

                // 이동 방향에 따라 좌우 반전 (전체 구조 뒤집기)
                if (direction.x < 0) // 왼쪽으로 이동
                {
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.y), transform.localScale.y, transform.localScale.z);
                }
                else if (direction.x > 0) // 오른쪽으로 이동
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.y), transform.localScale.y, transform.localScale.z);
                }
            }

        }
    }

    // 몬스터가 죽을 때 호출
    public void Die()
    {
        // ExpItem 생성
        if (expItemPrefab != null)
        {
            Instantiate(expItemPrefab, transform.position, Quaternion.identity);
        }

        // 몬스터 제거
        Destroy(gameObject);
    }
}
