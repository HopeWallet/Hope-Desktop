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

    private int itemCount,
                previousTopIndex,
                previousBottomIndex;

    private void Start()
    {
        rectTransform = transform as RectTransform;
        listParent = rectTransform.GetChild(0).transform;
        scrollRect = rectTransform.parent.GetComponent<ScrollRect>();
        viewportHeight = rectTransform.rect.size.y;

        Observable.EveryUpdate().Where(_ => itemCount != listParent.childCount).Subscribe(_ => ItemCountChanged());

        scrollRect.onValueChanged.AddListener(_ => ScrollRectChanged());
    }

    private void ItemCountChanged()
    {
        int topIndex, bottomIndex;

        itemCount = listParent.childCount;

        GetBorderIndices(out topIndex, out bottomIndex);
        UpdateGlobalItemVisibility(topIndex, bottomIndex);
    }

    private void ScrollRectChanged()
    {
        int topIndex, bottomIndex;

        GetBorderIndices(out topIndex, out bottomIndex);
        UpdateCurrentBorderItemVisibility(topIndex, bottomIndex);
    }

    private void GetBorderIndices(out int topIndex, out int bottomIndex)
    {
        float buttonSize = listParent.GetChild(0).GetComponent<RectTransform>().rect.size.y;
        float contentSize = itemCount * buttonSize;
        float outOfViewSize = contentSize - viewportHeight;

        float inViewMin = (1f - scrollRect.verticalNormalizedPosition) * outOfViewSize;
        float inViewMax = inViewMin + viewportHeight;

        topIndex = (int)inViewMin / (int)buttonSize;
        bottomIndex = (int)inViewMax / (int)buttonSize;

        if (topIndex < 0)
            topIndex = 0;

        if (bottomIndex >= itemCount)
            bottomIndex = itemCount - 1;
    }

    private void UpdateCurrentBorderItemVisibility(int topIndex, int bottomIndex)
    {
        if (topIndex < previousTopIndex)
            listParent.GetChild(topIndex).GetChild(0).gameObject.SetActive(true);
        else if (topIndex > previousTopIndex)
            listParent.GetChild(previousTopIndex).GetChild(0).gameObject.SetActive(false);

        if (bottomIndex > previousBottomIndex)
            listParent.GetChild(bottomIndex).GetChild(0).gameObject.SetActive(true);
        else if (bottomIndex < previousBottomIndex)
            listParent.GetChild(previousBottomIndex).GetChild(0).gameObject.SetActive(false);

        previousTopIndex = topIndex;
        previousBottomIndex = bottomIndex;
    }

    private void UpdateEnableItem(ref int previousIndex, int currentIndex)
    {
        //if (previousIndex < currentIndex)
    }

    private void UpdateGlobalItemVisibility(int topIndex, int bottomIndex)
    {
        for (int i = 0; i < itemCount; i++)
            listParent.GetChild(i).GetChild(0).gameObject.SetActive(topIndex <= i && bottomIndex >= i);

        previousTopIndex = topIndex;
        previousBottomIndex = bottomIndex;
    }
}