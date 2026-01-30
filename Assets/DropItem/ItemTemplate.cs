using Unity.VisualScripting;
using UnityEngine;

public class ItemTemplate : MonoBehaviour
{
    protected GameObject playerObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 플레이어를 자동으로 찾기
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            playerObject = player.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 아이템이 수집되었을 때 호출 (자식 클래스에서 오버라이드)
    protected virtual void OnCollected()
    {
        // 기본 동작 (필요시 추가)
    }

    // 플레이어와 충돌했을 때 (Trigger)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 태그로 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player detected! Collecting item...");
            OnCollected();
            Destroy(gameObject);
        }
    }
}

