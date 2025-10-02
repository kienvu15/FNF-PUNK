using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float delayAfterCountdown = 0.5f;

    // danh sách màu để thay đổi mỗi lần
    public Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta };

    public AudioClip AudioClip;
    public AudioSource source;
    public void StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        int count = 3;
        source.Play();
        while (count > 0)
        {
 
            ShowText(count.ToString());
            yield return new WaitForSeconds(0.7f);
            count--;
        }

        ShowText("Game!");
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);

        yield return new WaitForSeconds(delayAfterCountdown);

        SongManager.Instance.StartSong();
    }

    private void ShowText(string text)
    {
        countdownText.text = text;

        // đổi sang màu random
        countdownText.color = colors[Random.Range(0, colors.Length)];

        // reset scale nhỏ ban đầu
        countdownText.transform.localScale = Vector3.zero;

        // chạy animation scale to
        StartCoroutine(ScaleText(countdownText.transform));
    }

    private IEnumerator ScaleText(Transform target)
    {
        float duration = 0.5f; // thời gian scale
        float time = 0f;

        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.one * 1.5f; // phóng to 1.5 lần

        while (time < duration)
        {
            float t = time / duration;
            target.localScale = Vector3.Lerp(start, end, t);
            time += Time.deltaTime;
            yield return null;
        }

        target.localScale = end;
    }
}
