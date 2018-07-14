using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptimizedScrollview : MonoBehaviour
{
    private ScrollRect scrollRect;

    private Transform listParent;
    private RectTransform rectTransform;

    private float viewportHeight;

    private void Start()
    {
        rectTransform = transform as RectTransform;
        listParent = rectTransform.GetChild(0).transform;
        scrollRect = rectTransform.parent.GetComponent<ScrollRect>();
        viewportHeight = rectTransform.rect.size.y;

        scrollRect.onValueChanged.AddListener(val => CheckElements());
    }

    private void CheckElements()
    {
        float globalButtonSize = (listParent.childCount * listParent.GetChild(0).GetComponent<RectTransform>().rect.size.y) - viewportHeight;
        float currentTopCutoff = (1f - scrollRect.verticalNormalizedPosition) * globalButtonSize;

        Debug.Log(currentTopCutoff);
    }
}
