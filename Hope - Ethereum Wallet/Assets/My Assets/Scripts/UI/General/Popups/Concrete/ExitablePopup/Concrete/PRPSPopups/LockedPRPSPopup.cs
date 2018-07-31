using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which represents the popup used for displaying current locked purpose.
/// </summary>
public sealed class LockedPRPSPopup : ExitablePopupComponent<LockedPRPSPopup>
{
    public Transform itemSpawnTransform;
    public Button lockPRPSButton;

    private readonly List<LockedPRPSItemButton> lockedPRPSItems = new List<LockedPRPSItemButton>();

    private LockedPRPSManager lockedPRPSManager;
    private LockedPRPSItemButton.Factory lockedPRPSItemFactory;

    [Inject]
    public void Construct(
        LockedPRPSManager lockedPRPSManager,
        LockedPRPSItemButton.Factory lockedPRPSItemFactory)
    {
        this.lockedPRPSManager = lockedPRPSManager;
        this.lockedPRPSItemFactory = lockedPRPSItemFactory;
    }

    protected override void Awake()
    {
        base.Awake();
        lockPRPSButton.onClick.AddListener(() => popupManager.GetPopup<LockPRPSPopup>(true));

        CreateInitialItemList();
        (Animator as LockedPRPSPopupAnimator).LockedPurposeItems = lockedPRPSItems.Select(item => item.transform.parent.gameObject).ToArray();

        lockedPRPSManager.OnLockedPRPSUpdated += UpdateList;
    }

    private void OnDestroy()
    {
        lockedPRPSItems.ForEach(item => item.EndButtonUpdates());
    }

    private void UpdateList()
    {
        var unfulfilledItems = lockedPRPSManager.UnfulfilledItems;
        var ids = unfulfilledItems.Select(item => item.Id);
        lockedPRPSItems.SafeForEach(button =>
        {
            if (!ids.Contains(button.ButtonInfo.Id))
            {
                Destroy(button.transform.parent.gameObject);
                lockedPRPSItems.Remove(button);
            }
        });

        unfulfilledItems.Where(item => !lockedPRPSItems.Select(button => button.ButtonInfo).Contains(item))?.ForEach(item => CreateItem(item));
    }

    private void CreateInitialItemList()
    {
        lockedPRPSManager.UnfulfilledItems.ForEach(CreateItem);
    }

    private void CreateItem(HodlerMimic.Output.Item item)
    {
        LockedPRPSItemButton itemButton = lockedPRPSItemFactory.Create().SetButtonInfo(item);
        itemButton.StartButtonUpdates();

        itemButton.transform.parent.parent = itemSpawnTransform;
        itemButton.transform.parent.localScale = new Vector3(0f, 1f, 1f);
        itemButton.transform.localScale = Vector3.one;

        lockedPRPSItems.Add(itemButton);
    }
}