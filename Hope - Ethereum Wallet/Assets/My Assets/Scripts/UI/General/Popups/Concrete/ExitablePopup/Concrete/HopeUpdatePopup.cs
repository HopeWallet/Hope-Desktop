using TMPro;
using UnityEngine;
using Zenject;

/// <summary>
/// Popup used for displaying an update notification.
/// </summary>
public sealed class HopeUpdatePopup : OkCancelPopupComponent<HopeUpdatePopup>
{
    [SerializeField] private TMP_Text updateAvailableText;

    private WalletVersionManager walletVersionManager;

    /// <summary>
    /// Injects the required wallet version manager dependency.
    /// </summary>
    /// <param name="walletVersionManager"> The active WalletVersionManager. </param>
    [Inject]
    public void Construct(WalletVersionManager walletVersionManager) => this.walletVersionManager = walletVersionManager;

    /// <summary>
    /// Sets the update text to reflect the latest version.
    /// </summary>
    protected override void OnStart() => updateAvailableText.text = $"New Hope update available! (v{walletVersionManager.LatestVersion})";

    /// <summary>
    /// Opens the github releases page containing the latest wallet version.
    /// </summary>
    protected override void OnOkClicked() => Application.OpenURL(walletVersionManager.LatestVersionUrl);
}