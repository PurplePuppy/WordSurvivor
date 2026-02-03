using UnityEngine;

public class ExpItem : ItemTemplate
{
    [Tooltip("획득 시 제공되는 경험치 양")]
    public int expAmount = 10; // 경험치 양

    // 경험치 아이템이 수집되었을 때
    protected override void OnCollected()
    {
        // TODO: 플레이어 경험치 증가 로직
        PlayerCtrl player = playerObject.GetComponent<PlayerCtrl>();
        player.AddExp(expAmount);

        Debug.Log($"경험치 {expAmount} 획득!");
    }
}
