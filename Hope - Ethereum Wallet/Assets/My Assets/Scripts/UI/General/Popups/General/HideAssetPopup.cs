using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class used for displaying the remove button on a TradableAsset.
/// </summary>
public class HideAssetPopup : FactoryPopup<HideAssetPopup>, ILateUpdater, ILeftClickObservable, IRightClickObservable
{

    // ==========
    // TODO:
    // - Possibly test out the implementation of the hide button in a fixed location below the asset button.
    //===========

    private static int IDCounter;

    public Button button;

    private UpdateManager updateManager;
    private PopupManager popupManager;
    private MouseClickObserver clickObserver;

    private RectTransform rectTransform;
    private Rect buttonBounds;
    private Vector3 position;

    private int popupId;

    private const int MAX_RANGE = 250;

    /// <summary>
    /// The TradableAsset selected to be removed.
    /// </summary>
    public TradableAsset TradableAsset { get; set; }

    /// <summary>
    /// Initializes the position of this popup.
    /// </summary>
    private void Awake() => SetupPosition();

    /// <summary>
    /// Removes this popup from the UpdateManager.
    /// </summary>
    protected void OnDestroy()
    {
        updateManager.RemoveLateUpdater(this);
        clickObserver.UnsubscribeObservable(this);
    }

    /// <summary>
    /// Starts the HideAssetPopup.
    /// </summary>
    private void Start()
    {
        popupId = ++IDCounter;
        button.onClick.AddListener(RemoveButtonDependencies);

        updateManager.AddLateUpdater(this);
        clickObserver.SubscribeObservable(this);
    }

    /// <summary>
    /// Adds the dependencies needed for the HideAssetPopup.
    /// </summary>
    /// <param name="updateManager"> The active UpdateManager. </param>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="clickObserver"> The active ClickObserver. </param>
    [Inject]
    public void Construct(UpdateManager updateManager, PopupManager popupManager, MouseClickObserver clickObserver)
    {
        this.updateManager = updateManager;
        this.popupManager = popupManager;
        this.clickObserver = clickObserver;
    }

    /// <summary>
    /// Removes the button from the tradabable assets and the address from the contract manager.
    /// </summary>
    private void RemoveButtonDependencies()
    {
        popupManager.CloseActivePopup();
        popupManager.GetPopup<ConfirmHideAssetPopup>().AssetToRemove = TradableAsset;
    }

    /// <summary>
    /// Sets up the position of the popup.
    /// </summary>
    private void SetupPosition()
    {
        rectTransform = transform as RectTransform;
        position = new Vector2(Input.mousePosition.x + rectTransform.rect.size.x / 2.1f, Input.mousePosition.y - rectTransform.rect.size.y / 2.1f);
        rectTransform.position = position;
        buttonBounds = button.GetButtonBounds();
    }

    /// <summary>
    /// Closes the popup if a boolean predicate is true.
    /// </summary>
    /// <param name="predicate"> The statement to check for truth. </param>
    private void CloseIfTrue(bool predicate)
    {
        if (predicate)
            popupManager.CloseActivePopup();
    }

    /// <summary>
    /// Checks the position of the mouse with the position of the button and closes the popup if it is too far.
    /// </summary>
    public void LateUpdaterUpdate() => CloseIfTrue(Vector3.Magnitude(position - Input.mousePosition) > MAX_RANGE);

    /// <summary>
    /// Checks if the event was a right click down and if the popup id is equal to the counter.
    /// </summary>
    /// <param name="clickType"> The type of right click. </param>
    public void OnRightClick(ClickType clickType) => CloseIfTrue(clickType == ClickType.Down && popupId == IDCounter);

    /// <summary>
    /// Checks if the left click was outside of the button bounds and closes the popup if so.
    /// </summary>
    /// <param name="clickType"> The type of left click. </param>
    public void OnLeftClick(ClickType clickType) => CloseIfTrue(!buttonBounds.Contains(Input.mousePosition));

}
