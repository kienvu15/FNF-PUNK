using UnityEngine;

public class HoldNote : MonoBehaviour
{
    private double timeInstantiated;      // thời điểm spawn
    public float assignedTime;            // thời điểm head tới tap line
    public float holdDuration;            // thời gian giữ

    [Header("Sprites (assign in prefab)")]
    public SpriteRenderer head;
    public SpriteRenderer body;
    public SpriteRenderer tail;

    private float fullLength = 0f;        // chiều dài body ban đầu (world units)
    private bool isHoldingPhase = false;
    private bool finished = false;
    private bool playerHolding = false;

    // info cho sprite simple
    private bool bodyIsSimple;
    private float spriteUnitHeight = 1f; // chiều cao sprite khi scale = 1 (world units)

    private void Awake()
    {
        if (body == null)
        {
            Debug.LogError("HoldNote: body SpriteRenderer chưa gán!");
            return;
        }

        bodyIsSimple = (body.drawMode == SpriteDrawMode.Simple);

        if (bodyIsSimple)
        {
            if (body.sprite != null)
                spriteUnitHeight = body.sprite.bounds.size.y; // world units at scale = 1
            else
                spriteUnitHeight = 1f;
        }

        // ẩn tạm — Start sẽ bật lại
        if (head) head.enabled = false;
        if (body) body.enabled = false;
        if (tail) tail.enabled = false;
    }

    private bool initialized = false;

    private void Start()
    {
        timeInstantiated = SongManager.GetAudioSourceTime();

        float headToTap = Mathf.Abs(SongManager.Instance.noteSpawnY - SongManager.Instance.noteTapY);
        float noteTime = SongManager.Instance.noteTime;
        if (noteTime <= 0f) noteTime = 1f;

        float unitsPerSecond = headToTap / noteTime;
        fullLength = Mathf.Max(0f, holdDuration * unitsPerSecond);

        ApplyBodyLength(fullLength);

        

        initialized = true; // 👈 báo hiệu đã setup xong
    }

    private void Update()
    {
        if (!initialized) return; // 👈 chặn Update nếu chưa setup
        if (finished) return;

        double timeSinceInst = SongManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInst / (SongManager.Instance.noteTime * 2f));

        if (!isHoldingPhase)
        {
            if (t > 1f)
            {
                Miss();
                Destroy(gameObject);
                return;
            }

            transform.localPosition = Vector3.Lerp(
                Vector3.up * SongManager.Instance.noteSpawnY,
                Vector3.up * SongManager.Instance.noteDespawnY,
                t
            );

            if (transform.localPosition.y <= SongManager.Instance.noteTapY + 0.01f)
            {
                StartHolding();
            }
        }
        else
        {
            float elapsed = (float)(SongManager.GetAudioSourceTime() - assignedTime);
            float progress = holdDuration > 0f ? elapsed / holdDuration : 1f;
            progress = Mathf.Clamp01(progress);

            float currentLength = Mathf.Max(0f, fullLength * (1f - progress));
            ApplyBodyLength(currentLength);

            if (progress >= 1f)
            {
                finished = true;
                if (playerHolding) ScoreManager.Perfect();
                else ScoreManager.Miss();

                Destroy(gameObject);
            }
        }
    }

    // Apply chiều dài (world units) cho body + đặt tail (pivot bottom assumed)
    private void ApplyBodyLength(float worldLength)
    {
        if (bodyIsSimple)
        {
            // spriteUnitHeight là chiều cao sprite khi scale = 1 (world units)
            float baseH = spriteUnitHeight;
            if (baseH <= 0.0001f) baseH = 1f;

            float newScaleY = baseH > 0f ? worldLength / baseH : 1f;

            Vector3 ls = body.transform.localScale;
            body.transform.localScale = new Vector3(ls.x, newScaleY, ls.z);

            // Với pivot bottom: bottom tại local y = 0
            body.transform.localPosition = Vector3.zero;
        }
        else
        {
            // drawMode Sliced/Tiled: body.size là world units
            Vector2 s = body.size;
            s.y = worldLength;
            body.size = s;
            body.transform.localPosition = Vector3.zero;
        }

        // Tail đặt ở đỉnh body (nếu pivot tail center thì center sẽ ở y = worldLength)
        if (tail != null)
            tail.transform.localPosition = new Vector3(0f, worldLength, 0f);
    }

    public void RegisterHoldInput(bool holding)
    {
        playerHolding = holding;
    }

    private void StartHolding()
    {
        if (isHoldingPhase) return;
        isHoldingPhase = true;

        transform.localPosition = new Vector3(
            transform.localPosition.x,
            SongManager.Instance.noteTapY,
            transform.localPosition.z
        );
    }

    private void Miss()
    {
        ScoreManager.Miss();
    }

    public void ActiveSprite()
    {
        head.enabled = true;
        body.enabled = true;
        tail.enabled = true;
    }
}
