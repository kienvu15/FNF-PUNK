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

    [Header("Lane Direction")]
    public string laneID;
    public Transform noteSpawnPoint;
    public Lane lane;

    [Header("State (read-only)")]
    private float heldTime = 0f;
    private float fullLength = 0f;        // chiều dài body ban đầu (world units)
    private bool isHoldingPhase = false;
    private bool finished = false;
    private bool playerHolding = false;
    private bool headHidden = false;

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
                spriteUnitHeight = body.sprite.bounds.size.y;
            else
                spriteUnitHeight = 1f;
        }

        if (head) head.enabled = false;
        if (body) body.enabled = false;
        if (tail) tail.enabled = false;

        if (laneID == "Left")
        {
            noteSpawnPoint.position = new Vector3(-4.18f, -2.8f, noteSpawnPoint.position.z);
        }
        else if (laneID == "Down")
        {
            noteSpawnPoint.position = new Vector3(-1.84f, -2.8f, noteSpawnPoint.position.z);
        }
        else if (laneID == "Up")
        {
            noteSpawnPoint.position = new Vector3(1.84f, -2.8f, noteSpawnPoint.position.z);
        }
        else if (laneID == "Right")
        {
            noteSpawnPoint.position = new Vector3(4.18f, -2.8f, noteSpawnPoint.position.z);
        }

        lane = FindFirstObjectByType<Lane>();
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

        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return; 
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

            if (playerHolding)
            {
                heldTime += Time.deltaTime;
            }
            else
            {
                // Nếu người chơi đã nhả ra nhưng note chưa kết thúc thì ẩn head
                if (!headHidden && head != null)
                {
                    head.enabled = false;
                    headHidden = true;
                }
            }

            if (progress >= 1f)
            {
                finished = true;

                float holdRatio = heldTime / holdDuration;

                if (holdRatio >= 0.95f)
                {
                    ScoreManager.Perfect();
                    ParticlePool.Instance.SpawnFromPool(laneID, noteSpawnPoint);
                    lane.ShowFeedback("Perfect", Color.cyan);
                    lane.SpawnParticle(noteSpawnPoint.position, lane.perfectParticlePrefab);
                }
                else if (holdRatio >= 0.7f)
                {
                    ScoreManager.Good();
                    ParticlePool.Instance.SpawnFromPool(laneID, noteSpawnPoint);
                    lane.ShowFeedback("Good", Color.green);
                    lane.SpawnParticle(noteSpawnPoint.position, lane.goodParticlePrefab);

                }
                else if (holdRatio >= 0.2f)
                {
                    ScoreManager.Bad();
                    ParticlePool.Instance.SpawnFromPool(laneID, noteSpawnPoint);
                    lane.ShowFeedback("Bad", Color.yellow);
                    lane.SpawnParticle(noteSpawnPoint.position, lane.goodParticlePrefab);

                }
                else
                {
                    ScoreManager.Miss();
                    ParticlePool.Instance.SpawnFromPool(laneID, noteSpawnPoint);
                    lane.ShowFeedback("Miss", Color.red);
                    lane.SpawnParticle(noteSpawnPoint.position, lane.missParticlePrefab);

                }

                Destroy(gameObject);
            }
        }

    }

    private void ApplyBodyLength(float worldLength)
    {
        if (bodyIsSimple)
        {
            float baseH = spriteUnitHeight;
            if (baseH <= 0.0001f) baseH = 1f;

            float newScaleY = baseH > 0f ? worldLength / baseH : 1f;

            Vector3 ls = body.transform.localScale;
            body.transform.localScale = new Vector3(ls.x, newScaleY, ls.z);

            body.transform.localPosition = Vector3.zero;
        }
        else
        {
            Vector2 s = body.size;
            s.y = worldLength;
            body.size = s;
            body.transform.localPosition = Vector3.zero;
        }

        if (tail != null)
            tail.transform.localPosition = new Vector3(0f, worldLength, 0f);
    }

    public void RegisterHoldInput(bool holding)
    {
        playerHolding = holding;

        if (holding && headHidden && head != null)
        {
            head.enabled = true;
            headHidden = false;
        }
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

        if (!playerHolding && head != null)
        {
            head.enabled = false;
            headHidden = true;
        }
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
