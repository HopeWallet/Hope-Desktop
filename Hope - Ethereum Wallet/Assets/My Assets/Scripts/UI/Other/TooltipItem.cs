using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The icon class that manages the <see cref="InfoPopup"/> details and icon animations
/// </summary>
public sealed class TooltipItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private static int clickId;

	public string infoTitle;
	public string infoText;
	public float itemWidth;
	public bool infoIcon;

	private bool hovering;

	private Button button;

	public PopupManager PopupManager { get; set; }

	private void Awake() => button = GetComponent<Button>();

	/// <summary>
	/// Opens the InfoPopup after a short period of hovering.
	/// </summary>
	private void Update()
	{
		if ((!infoIcon && button.interactable) || !hovering)
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
		clickId++;
		hovering = true;
	}

	/// <summary>
	/// Mouse exited the icon
	/// </summary>
	/// <param name="eventData"> The PointerEventData </param>
	public void OnPointerExit(PointerEventData eventData)
	{
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
			PopupManager.GetPopup<InfoPopup>(true).SetUIElements(infoTitle, infoText, transform.position, itemWidth / 2, infoIcon);
	}
}