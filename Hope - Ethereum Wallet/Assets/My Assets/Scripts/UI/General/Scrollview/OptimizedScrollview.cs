using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptimizedScrollview : MonoBehaviour
{
    private ScrollRect scrollRect;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = transform as RectTransform;
        scrollRect = GetComponent<ScrollRect>();

        scrollRect.onValueChanged.AddListener(val => CheckElements());
    }

    private void CheckElements()
    {
        //Debug.Log(scrollRect.verticalNormalizedPosition + " ");
        //Debug.Log(rectTransform.rect.yMin + " " + rectTransform.rect.yMax);
        Debug.Log(rectTransform.position + " " + rectTransform.localPosition + " " + rectTransform.anchoredPosition + " " + rectTransform.rect.size);
    }
}
