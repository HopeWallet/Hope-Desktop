using UnityEngine.EventSystems;
using UnityEngine;
using System;

public sealed class Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static int clickId;

    public string infoTitle;
    public string infoText;
    public IconType iconType;

    private bool hoverableIcon;
    private bool hovering;

    public PopupManager PopupManager { get; set; }

    public static event Action AnimatePopupOut;

    private void Awake() => hoverableIcon = iconType == IconType.Info || iconType == IconType.Error;

    /// <summary>
    /// Opens the InfoPopup after a short period of hovering.
    /// </summary>
    private void Update()
    {
        if (!hovering)
            return;

        int val = clickId;
        CoroutineUtils.ExecuteAfterWait(0.1f, () => OpenPopup(val));
    }

    /// <summary>
    /// Mouse entered the icon
    /// </summary>
    /// <param name="eventData"> The PointerEventData </param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!hoverableIcon)
            return;

        clickId++;
        hovering = true;
    }

    /// <summary>
    /// Mouse exited the icon
    /// </summary>
    /// <param name="eventData"> The PointerEventData </param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!hoverableIcon)
            return;

        clickId++;
        hovering = false;

        PopupManager.GetPopup<InfoPopup>().Animator.AnimateDisable(() => PopupManager.KillActivePopup());
    }

    /// <summary>
    /// Animates the icon
    /// </summary>
    /// <param name="value"> The end value of the graphic and scale being animated to </param>
    public void AnimateIcon(float value)
    {
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

    public enum IconType { Info, Error, CheckMark }
}