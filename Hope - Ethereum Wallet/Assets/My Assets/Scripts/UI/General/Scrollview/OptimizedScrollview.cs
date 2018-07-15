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
        float buttonCount = listParent.childCount;
        float buttonSize = listParent.GetChild(0).GetComponent<RectTransform>().rect.size.y;
        float contentSize = buttonCount * buttonSize;
        float outOfViewSize = contentSize - viewportHeight;

        float inViewMin = (1f - scrollRect.verticalNormalizedPosition) * outOfViewSize;
        float inViewMax = inViewMin + viewportHeight;

        // Scrollbar size
        // Mathf.Clamp01(viewportHeight / globalButtonSize);

        Debug.Log(contentSize + " " + outOfViewSize + " " + inViewMin + " " + viewportHeight);

        for (int i = 0; i < buttonCount; i++)
        {
            float buttonPos = i * buttonSize;
            listParent.GetChild(0).GetChild(0).gameObject.SetActive(buttonPos + buttonSize > inViewMin || buttonPos < inViewMax);
        }
    }
}