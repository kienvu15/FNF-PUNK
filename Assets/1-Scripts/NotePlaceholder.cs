using UnityEngine;

public class NotePlaceholder : MonoBehaviour
{
    public Transform hitTarget;      // gán HitArea transform
    public float speed = 5f;         // tốc độ di chuyển xuống
    void Update()
    {
        // di chuyển theo hướng xuống (tới vị trí hitTarget)
        Vector3 dir = (hitTarget.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // nếu đã qua (hoặc rất gần) thì tự destroy
        if (Vector3.Distance(transform.position, hitTarget.position) < 0.1f)
            Destroy(gameObject);
    }
}
