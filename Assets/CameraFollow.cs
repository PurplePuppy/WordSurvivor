using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Refs")]
    [Tooltip("플레이어 Transform. 카메라가 이 오브젝트를 따라갑니다.")]
    public Transform target;

    [Header("Settings")]
    [Tooltip("카메라가 타겟을 따라갈 때의 부드러움 정도 (0 = 즉시 이동, 값이 클수록 부드럽게)")]
    [Range(0f, 1f)]
    public float smoothing = 0.1f;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        
        if (smoothing > 0f)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, 1f - smoothing);
        }
        else
        {
            transform.position = desiredPosition;
        }
    }
}
