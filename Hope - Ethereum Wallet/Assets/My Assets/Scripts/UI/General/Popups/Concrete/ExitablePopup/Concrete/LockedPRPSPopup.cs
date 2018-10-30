using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which represents the popup used for displaying current locked purpose.
/// </summary>
public sealed class LockedPRPSPopup : ExitablePopupComponent<LockedPRPSPopup>
{
    [SerializeField] private Transform itemSpawnTransform;

    [SerializeField]
    private Button lockPRPSButton,
                                    nextButton,
                                    previousButton;

    [SerializeField] private TMP_Text timeLeftText;

    private readonly List<LockedPRPSItemButton> lockedPRPSItems = new List<LockedPRPSItemButton>();

    private LockedPRPSManager lockedPRPSManager;
    private LockedPRPSItemButton.Factory lockedPRPSItemFactory;
    private LockedPRPSPopupAnimator lockedPRPSAnimator;

    private bool useUnlockableTime;

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

        SwitchUnlockDateSection(false);
        CreateInitialItemList();

        lockedPRPSAnimator = Animator as LockedPRPSPopupAnimator;
        lockedPRPSAnimator.LockedPurposeItems = lockedPRPSItems.Select(item => item.transform.parent.gameObject).ToArray();

        lockPRPSButton.onClick.AddListener(() => popupManager.GetPopup<LockPRPSPopup>(true));
        nextButton.onClick.AddListener(() => SwitchUnlockDateSection(false));
        previousButton.onClick.AddListener(() => SwitchUnlockDateSection(true));

        lockedPRPSManager.OnLockedPRPSUpdated += UpdateList;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        lockedPRPSManager.OnLockedPRPSUpdated -= UpdateList;
        lockedPRPSItems.ForEach(item => item.EndButtonUpdates());
    }

    private void SwitchUnlockDateSection(bool previousButtonPressed)
    {
        useUnlockableTime = previousButtonPressed;
        lockedPRPSItems.ForEach(item => item.UseUnlockableTime = useUnlockableTime);

        timeLeftText.text = useUnlockableTime ? "Unlock Date" : "Time Left";

        previousButton.gameObject.SetActive(!previousButtonPressed);
        nextButton.gameObject.SetActive(previousButtonPressed);
    }

    private void UpdateList()
    {
        var unfulfilledItems = lockedPRPSManager.UnfulfilledItems;
        var ids = unfulfilledItems.Select(item => item.Id);
        lockedPRPSItems.SafeForEach(button =>
        {
            if (!ids.Contains(button.ButtonInfo.Id))
            {
                GameObject objToDestroy = button.transform.parent.gameObject;
                lockedPRPSAnimator.AnimateWalletOut(objToDestroy, () => Destroy(objToDestroy));
                lockedPRPSItems.Remove(button);
            }
        });

        unfulfilledItems.Where(item => !lockedPRPSItems.Select(button => button.ButtonInfo).Contains(item))?.ForEach(item => CreateItem(item, false));
    }

    private void CreateInitialItemList()
    {
        lockedPRPSManager.UnfulfilledItems.ForEach(item => CreateItem(item));
    }

    private void CreateItem(Hodler.Output.Item item, bool initialWalletLoad = true)
    {
        LockedPRPSItemButton itemButton = lockedPRPSItemFactory.Create().SetButtonInfo(item);
        itemButton.UseUnlockableTime = useUnlockableTime;

        Transform componentTransform = itemButton.transform;
        Transform parentTransform = componentTransform.parent;
        itemButton.StartButtonUpdates();

        parentTransform.parent = itemSpawnTransform;
        parentTransform.localScale = new Vector3(0f, 1f, 1f);
        componentTransform.localScale = Vector3.one;

        if (!initialWalletLoad)
            lockedPRPSAnimator.AnimateWalletIn(parentTransform.gameObject);

        lockedPRPSItems.Add(itemButton);
    }
}