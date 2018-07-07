using UnityEngine;

/// <summary>
/// Class which animates the ChooseWalletMenu.
/// </summary>
public sealed class ChooseWalletMenuAnimator : MenuAnimator
{

    [SerializeField] private GameObject form;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject ledgerButton;
    [SerializeField] private GameObject hopeButton;

    /// <summary>
    /// Animates the UI elements of the form into view
    /// </summary>
    protected override void AnimateIn()
    {
        form.AnimateGraphicAndScale(1f, 1f, 0.2f,
            () => title.AnimateGraphicAndScale(0.85f, 1f, 0.2f,
            () => ledgerButton.AnimateGraphicAndScale(1f, 1f, 0.2f,
            () => hopeButton.AnimateGraphicAndScale(1f, 1f, 0.2f, FinishedAnimating))));
    }

    /// <summary>
    /// Animates the UI elements of the form out of view
    /// </summary>
    protected override void AnimateOut()
    {
        hopeButton.AnimateGraphicAndScale(0f, 0.1f, 0.2f);
        ledgerButton.AnimateGraphicAndScale(0f, 0.1f, 0.2f);
        title.AnimateGraphicAndScale(0f, 0.1f, 0.2f,
            () => form.AnimateGraphicAndScale(0f, 0.1f, 0.2f, FinishedAnimating));
    }
}
