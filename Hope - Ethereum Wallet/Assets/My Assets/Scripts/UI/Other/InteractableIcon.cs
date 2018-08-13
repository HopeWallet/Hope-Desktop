using UnityEngine.EventSystems;
using UnityEngine;

/// <summary>
/// The icon class that manages the <see cref="InfoPopup"/> details and icon animations
/// </summary>
public sealed class InteractableIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static int clickId;

    public string infoTitle;
    public string infoText;
    public IconType iconType;

	private bool hoverable;
    private bool hovering;

	private Texture2D handCursor;

    public PopupManager PopupManager { get; set; }

	/// <summary>
	/// Sets the hoverable bool depending on the IconType
	/// </summary>
	private void Awake()
	{
		handCursor = Resources.Load("UI/Graphics/Textures/Other/Icons/HandCursor_Icon") as Texture2D;

		hoverable = iconType == IconType.Info || iconType == IconType.Error || iconType == IconType.Question;
	}

	/// <summary>
	/// Opens the InfoPopup after a short period of hovering.
	/// </summary>
	private void Update()
    {
        if (!hovering)
            return;

        int val = clickId;
		OpenPopup(val);
	}

    /// <summary>
    /// Mouse entered the icon
    /// </summary>
    /// <param name="eventData"> The PointerEventData </param>
    public void OnPointerEnter(PointerEventData eventData)
    {
		if (!hoverable) return;

		Cursor.SetCursor(handCursor, new Vector2(12f, 0f), CursorMode.Auto);

		clickId++;
        hovering = true;
    }

    /// <summary>
    /// Mouse exited the icon
    /// </summary>
    /// <param name="eventData"> The PointerEventData </param>
    public void OnPointerExit(PointerEventData eventData)
    {
		if (!hoverable) return;

		Cursor.SetCursor(null, new Vector2(12f, 0f), CursorMode.Auto);

		clickId++;
        hovering = false;

		CloseInfoPopup();
	}

	/// <summary>
	/// Animates the info popup out and then closes the popup
	/// </summary>
	private void CloseInfoPopup() => PopupManager.GetPopup<InfoPopup>()?.Animator?.AnimateDisable(() => PopupManager.KillActivePopup(typeof(InfoPopup)));

	/// <summary>
	/// Animates the icon
	/// </summary>
	/// <param name="value"> The end value of the graphic and scale being animated to </param>
	public void AnimateIcon(float value)
	{
		if (hovering && value == 0f)
			CloseInfoPopup();

		gameObject.AnimateGraphicAndScale(value, value, 0.1f);
	}

	/// <summary>
	/// Opens the popup if the current id is still the same as the static click id.
	/// </summary>
	/// <param name="currentId"> The current click id. </param>
	private void OpenPopup(int currentId)
    {
        if (currentId == clickId)
            PopupManager.GetPopup<InfoPopup>(true).SetUIElements(infoTitle, infoText, iconType, transform.position);
    }

	/// <summary>
	/// The icon type
	/// </summary>
	public enum IconType { Info, Error, Question, Checkmark }
}