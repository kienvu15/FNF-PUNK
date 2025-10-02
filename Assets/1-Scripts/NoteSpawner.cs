using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public AudioSource audioSource;          // gán AudioSource có nhạc
    public Transform[] lanes;                // Lane0..Lane3 (chứa vị trí spawn X)
    public Transform hitArea;                // nơi notes cần chạm
    public GameObject notePrefab;            // prefab note
    public float noteSpeed = 5f;             // tốc độ rơi
    public TextAsset songMapCSV;             // file CSV map

    private List<SongMap.NoteInfo> notes;    // danh sách note từ parser
    private double songStartDspTime;         // thời gian bắt đầu nhạc (DSP)
    private int nextNoteIndex = 0;           // index note tiếp theo sẽ spawn
    private float travelTime;                // thời gian để note đi từ spawn tới HitArea

    void Start()
    {
        // parse file CSV
        notes = SongMap.Parse(songMapCSV);

        // travel time = khoảng cách / tốc độ
        travelTime = Mathf.Abs(lanes[0].position.y - hitArea.position.y) / noteSpeed;

        // lấy DSP time hiện tại và schedule nhạc
        songStartDspTime = AudioSettings.dspTime;
        audioSource.PlayScheduled(songStartDspTime);
    }

    void Update()
    {
        if (nextNoteIndex >= notes.Count) return;

        // lấy DSP time hiện tại
        double songTime = AudioSettings.dspTime - songStartDspTime;

        // lấy note kế tiếp
        var note = notes[nextNoteIndex];

        // nếu đã đến lúc spawn (note.time - travelTime <= songTime)
        if (songTime >= note.time - travelTime)
        {
            SpawnNote(note);
            nextNoteIndex++;
        }
    }

    void SpawnNote(SongMap.NoteInfo note)
    {
        // tạo instance note ở lane đúng
        Vector3 spawnPos = lanes[note.lane].position;
        GameObject obj = Instantiate(notePrefab, spawnPos, Quaternion.identity);

        // cấu hình note
        var noteScript = obj.GetComponent<NotePlaceholder>();
        noteScript.hitTarget = hitArea;
        noteScript.speed = noteSpeed;
    }
}
