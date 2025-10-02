using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lane : MonoBehaviour
{
    [Header("Note Settings")]
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input;
    public GameObject tapNotePrefab;
    public GameObject holdNotePrefab;

    private List<object> notes = new List<object>(); // chứa cả Note và HoldNote
    public List<double> timeStamps = new List<double>();
    private List<double> noteLengths = new List<double>(); // song song với timeStamps

    private int spawnIndex = 0;
    private int inputIndex = 0;

    [Header("Feedback Settings")]
    public GameObject feedbackPrefab;
    public Transform canvasTransform;

    [Header("Particle Prefabs")]
    public GameObject perfectParticlePrefab;
    public GameObject goodParticlePrefab;
    public GameObject missParticlePrefab;

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(
                    note.Time,
                    SongManager.midiFile.GetTempoMap()
                );

                var lengthSpan = TimeConverter.ConvertTo<MetricTimeSpan>(
                    note.Length,
                    SongManager.midiFile.GetTempoMap()
                );

                double timeInSeconds =
                    (double)metricTimeSpan.Minutes * 60f
                    + metricTimeSpan.Seconds
                    + (double)metricTimeSpan.Milliseconds / 1000f;

                double lengthInSeconds =
                    (double)lengthSpan.Minutes * 60f
                    + lengthSpan.Seconds
                    + (double)lengthSpan.Milliseconds / 1000f;

                timeStamps.Add(timeInSeconds);
                noteLengths.Add(lengthInSeconds);
            }
        }
    }

    private void Update()
    {
        HandleNoteSpawning();
        HandleInput();
    }

    private void HandleNoteSpawning()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
            {
                double length = noteLengths[spawnIndex];
                if (length > 0.3f) // coi là hold nếu dài hơn 0.15s
                {
                    var obj = Instantiate(holdNotePrefab, transform);
                    obj.transform.localPosition = Vector3.up * SongManager.Instance.noteSpawnY; // 👈 spawn đúng chỗ

                    var hold = obj.GetComponent<HoldNote>();
                    hold.assignedTime = (float)timeStamps[spawnIndex];
                    hold.holdDuration = (float)length;
                    hold.ActiveSprite();
                    notes.Add(hold);


                }
                else
                {
                    var obj = Instantiate(tapNotePrefab, transform);
                    var note = obj.GetComponent<Note>();
                    note.assignedTime = (float)timeStamps[spawnIndex];
                    notes.Add(note);
                }
                spawnIndex++;
            }
        }
    }

    private void HandleInput()
    {
        if (inputIndex >= notes.Count) return;

        double marginOfError = SongManager.Instance.marginOfError;
        double audioTime =
            SongManager.GetAudioSourceTime()
            - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

        var currentNote = notes[inputIndex];

        // ----- Tap Note -----
        if (currentNote is Note tapNote)
        {
            double timeStamp = tapNote.assignedTime;

            if (Input.GetKeyDown(input))
            {
                double hitDiff = Math.Abs(audioTime - timeStamp);

                if (hitDiff < 0.05f) // Perfect
                {
                    PerfectHit();
                    Destroy(tapNote.gameObject);
                    inputIndex++;
                }
                else if (hitDiff < 0.1f) // Good
                {
                    GoodHit();
                    Destroy(tapNote.gameObject);
                    inputIndex++;
                }
                else
                {
                    Debug.Log($"Inaccurate tap hit on {inputIndex} note (diff: {hitDiff})");
                }
            }

            if (timeStamp + marginOfError <= audioTime)
            {
                Miss();
                inputIndex++;
            }
        }
        // ----- Hold Note -----
        else if (currentNote is HoldNote holdNote)
        {
            holdNote.RegisterHoldInput(Input.GetKey(input));

            double endTime = holdNote.assignedTime + holdNote.holdDuration;

            if (audioTime > endTime + marginOfError)
            {
                inputIndex++;
            }
        }


    }

    // ===== Scoring / Feedback =====
    private void PerfectHit()
    {
        ScoreManager.Perfect();
        ShowFeedback("Perfect", Color.cyan);
        SpawnParticle(perfectParticlePrefab);
    }

    private void GoodHit()
    {
        ScoreManager.Good();
        ShowFeedback("Good", Color.green);
        SpawnParticle(goodParticlePrefab);
    }

    private void Miss()
    {
        ScoreManager.Miss();
        ShowFeedback("Miss", Color.red);
        SpawnParticle(missParticlePrefab);
    }

    private void ShowFeedback(string text, Color color)
    {
        var obj = Instantiate(feedbackPrefab, canvasTransform);
        var popup = obj.GetComponent<TextMeshProUGUI>();
        popup.text = text;
        popup.color = color;
        StartCoroutine(AnimateFeedback(popup));
    }

    private IEnumerator AnimateFeedback(TextMeshProUGUI popup)
    {
        float lifetime = 0.5f;
        float moveUp = 50f;
        float scaleUp = 1.5f;

        RectTransform rect = popup.GetComponent<RectTransform>();
        Vector3 startPos = rect.anchoredPosition;
        Vector3 endPos = startPos + new Vector3(0, moveUp, 0);
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.one * scaleUp;

        float timer = 0f;
        while (timer < lifetime)
        {
            timer += Time.deltaTime;
            float t = timer / lifetime;

            rect.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
            rect.localScale = Vector3.Lerp(startScale, endScale, t);

            Color c = popup.color;
            c.a = 1 - t;
            popup.color = c;

            yield return null;
        }
        Destroy(popup.gameObject);
    }

    private void SpawnParticle(GameObject particlePrefab)
    {
        Vector3 spawnPos = new Vector3(
            transform.position.x,
            SongManager.Instance.noteTapY,
            transform.position.z
        );
        var particle = Instantiate(particlePrefab, spawnPos, Quaternion.identity);
        Destroy(particle, 1f);
    }
}
