using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptimizedScrollview : MonoBehaviour
{

    [SerializeField]
    private Transform listParent;

    private ScrollRect scrollRect;
    private RectTransform rectTransform;

    private float ySize;

    private void Start()
    {
        rectTransform = transform as RectTransform;
        scrollRect = GetComponent<ScrollRect>();
        ySize = rectTransform.rect.size.y;

        scrollRect.onValueChanged.AddListener(val => CheckElements());
    }

    private void CheckElements()
    {
        //Debug.Log(scrollRect.verticalNormalizedPosition + " ");
        //Debug.Log(rectTransform.rect.yMin + " " + rectTransform.rect.yMax);
        //Debug.Log(rectTransform.position + " " + rectTransform.localPosition + " " + rectTransform.anchoredPosition + " " + rectTransform.rect.size);
        //Debug.Log(rectTransform.rect.size + " => " + scrollRect.verticalNormalizedPosition);

        float globalButtonSize = listParent.childCount * listParent.GetChild(0).GetComponent<RectTransform>().rect.size.y;
        float currentTopCutoff = (1f - scrollRect.verticalNormalizedPosition) * globalButtonSize;
        Debug.Log(currentTopCutoff);
    }
}
