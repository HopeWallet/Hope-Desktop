using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class OptimizedScrollview : MonoBehaviour
{

    private ScrollRect scrollRect;

    private Transform listParent;
    private RectTransform rectTransform;

    private float viewportHeight;

    private int itemCount;

    private void Start()
    {
        rectTransform = transform as RectTransform;
        listParent = rectTransform.GetChild(0).transform;
        scrollRect = rectTransform.parent.GetComponent<ScrollRect>();
        viewportHeight = rectTransform.rect.size.y;

        Observable.EveryUpdate().Where(_ => CheckItemCount()).Subscribe(_ => CheckElements());

        scrollRect.onValueChanged.AddListener(val => CheckElements());
    }

    private bool CheckItemCount()
    {
        int oldItemCount = itemCount;
        itemCount = listParent.childCount;

        return oldItemCount != itemCount;
    }

    private void CheckElements()
    {
        float buttonCount = listParent.childCount;
        float buttonSize = listParent.GetChild(0).GetComponent<RectTransform>().rect.size.y;
        float contentSize = buttonCount * buttonSize;
        float outOfViewSize = contentSize - viewportHeight;

        float inViewMin = (1f - scrollRect.verticalNormalizedPosition) * outOfViewSize;
        float inViewMax = inViewMin + viewportHeight;

        for (int i = 0; i < buttonCount; i++)
        {
            float buttonPos = i * buttonSize;
            bool inView = buttonPos + buttonSize > inViewMin && buttonPos < inViewMax;
            listParent.GetChild(i).GetChild(0).gameObject.SetActive(inView);
        }
    }
}