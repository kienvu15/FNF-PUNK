using System.Collections.Generic;
using UnityEngine;

public class TestParser : MonoBehaviour
{
    void Start()
    {
        // load file csv từ Resources/Songs
        TextAsset csv = Resources.Load<TextAsset>("Songs/song_map_example");

        if (csv == null)
        {
            Debug.LogError("Không tìm thấy file CSV!");
            return;
        }

        List<SongMap.NoteInfo> notes = SongMap.Parse(csv);

        foreach (var note in notes)
        {
            Debug.Log($"time={note.time}, lane={note.lane}, type={note.type}");
        }
    }
}
