using UnityEngine;

public class ExpItem : ItemTemplate
{
    public int expAmount = 10; // 경험치 양

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 경험치 아이템이 수집되었을 때
    protected override void OnCollected()
    {
        // TODO: 플레이어 경험치 증가 로직
        // Player player = playerObject.GetComponent<Player>();
        // player.AddExp(expAmount);

        Debug.Log($"경험치 {expAmount} 획득!");
    }
}
