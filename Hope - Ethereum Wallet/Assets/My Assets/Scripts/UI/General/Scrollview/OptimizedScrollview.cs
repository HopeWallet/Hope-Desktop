using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public sealed class OptimizedScrollview : MonoBehaviour
{
    public static event Action<OptimizedScrollview> OnOptimizedScrollviewInitialized;
    public event Action<List<GameObject>> OnVisibleItemsChanged;

    public string scrollviewKey;

    private static readonly Dictionary<string, OptimizedScrollview> ActiveScrollviews = new Dictionary<string, OptimizedScrollview>();
    private readonly List<GameObject> items = new List<GameObject>();
    private readonly List<bool> previouslyActiveList = new List<bool>();

    private ScrollRect scrollRect;

    private Transform listParent;
    private RectTransform rectTransform;

    private float viewportHeight,
                  prevScrollVal = 1f,
                  buttonSize;

    private int itemCount,
                previousTopIndex,
                previousBottomIndex;

    public List<GameObject> VisibleItemList => items.Where(obj => obj.transform.GetChild(0).gameObject.activeInHierarchy).ToList();

    public static OptimizedScrollview GetScrollview(string scrollviewKey) => ActiveScrollviews.ContainsKey(scrollviewKey) ? ActiveScrollviews[scrollviewKey] : null;

    public void Refresh() => GetEnabledItems();

    private void Start()
    {
        rectTransform = transform as RectTransform;
        listParent = rectTransform.GetChild(0).transform;
        scrollRect = rectTransform.parent.GetComponent<ScrollRect>();
        viewportHeight = rectTransform.rect.size.y;

        Observable.EveryUpdate()
                  .Where(_ => itemCount != listParent.childCount)
                  .Subscribe(_ => ItemCountChanged());

        Observable.EveryUpdate()
                  .Where(_ => !items.Select(obj => obj.transform.GetChild(0).gameObject.activeInHierarchy).SequenceEqual(previouslyActiveList))
                  .Subscribe(_ => ActiveItemsUpdated());

        scrollRect.onValueChanged.AddListener(_ => ScrollRectChanged());

        if (!string.IsNullOrEmpty(scrollviewKey))
            ActiveScrollviews.Add(scrollviewKey, this);

        OnOptimizedScrollviewInitialized?.Invoke(this);
    }

    private void ItemCountChanged()
    {
        int topIndex, bottomIndex;
        float outOfViewContentSize;

        GetEnabledItems();
        GetScrollData(out topIndex, out bottomIndex, out outOfViewContentSize);
        UpdateGlobalItemVisibility(topIndex, bottomIndex);
    }

    private void ScrollRectChanged()
    {
        int topIndex, bottomIndex;
        float outOfViewContentSize;

        GetScrollData(out topIndex, out bottomIndex, out outOfViewContentSize);

        if (ScrolledTooFar(outOfViewContentSize))
            UpdateGlobalItemVisibility(topIndex, bottomIndex);
        else
            UpdateCurrentBorderItemVisibility(topIndex, bottomIndex);
    }

    private void GetScrollData(out int topIndex, out int bottomIndex, out float outOfViewContentSize)
    {
        if (buttonSize == 0f)
            buttonSize = listParent.GetChild(0).GetComponent<RectTransform>().rect.size.y;

        float inViewMin = (1f - scrollRect.verticalNormalizedPosition) * (outOfViewContentSize = (itemCount * buttonSize) - viewportHeight);
        float inViewMax = inViewMin + viewportHeight;

        if ((topIndex = (int)inViewMin / (int)buttonSize) < 0)
            topIndex = 0;

        if ((bottomIndex = (int)inViewMax / (int)buttonSize) >= itemCount)
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

    private bool ScrolledTooFar(float outOfViewContentSize)
    {
        return Mathf.Abs(prevScrollVal - (prevScrollVal = scrollRect.verticalNormalizedPosition)) > 1f / (outOfViewContentSize / buttonSize);
    }

    private void GetEnabledItems()
    {
        if (listParent == null)
            return;

        items.Clear();

        foreach (Transform child in listParent)
            if (child.gameObject.activeInHierarchy)
                items.Add(child.GetChild(0).gameObject);

        itemCount = items.Count;
    }

    private void UpdateGlobalItemVisibility(int topIndex, int bottomIndex)
    {
        for (int i = 0; i < itemCount; i++)
            items[i].SetActive(topIndex <= i && bottomIndex >= i);

        previousTopIndex = topIndex;
        previousBottomIndex = bottomIndex;
    }

    private void ActiveItemsUpdated()
    {
        previouslyActiveList.Clear();
        previouslyActiveList.AddRange(items.Select(obj => obj.transform.GetChild(0).gameObject.activeInHierarchy));

        OnVisibleItemsChanged?.Invoke(VisibleItemList);
    }
}