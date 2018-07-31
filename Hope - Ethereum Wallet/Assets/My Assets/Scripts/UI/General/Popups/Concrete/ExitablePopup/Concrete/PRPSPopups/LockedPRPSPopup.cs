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
        (Animator as LockedPRPSPopupAnimator).LockedPurposeItems = lockedPRPSItems.Select(item => item.gameObject).ToArray();
    }

    private void OnDestroy()
    {
        lockedPRPSItems.ForEach(item => item.EndButtonUpdates());
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
        itemButton.transform.parent.localScale = Vector3.one;
        itemButton.transform.localScale = Vector3.one;

        lockedPRPSItems.Add(itemButton);
    }
}