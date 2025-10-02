using UnityEngine;
using UnityEngine.UI;

public class SimpleSlide : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;

    private int currentIndex = 0;
    private float[] points;

    void Start()
    {
        int count = content.childCount;
        points = new float[count];
        for (int i = 0; i < count; i++)
        {
            points[i] = (float)i / (count - 1); // chia đều slide
        }
    }

    public void NextSlide()
    {
        if (currentIndex < points.Length - 1)
        {
            currentIndex++;
            scrollRect.horizontalNormalizedPosition = points[currentIndex];
        }
    }

    public void PrevSlide()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            scrollRect.horizontalNormalizedPosition = points[currentIndex];
        }
    }
}
