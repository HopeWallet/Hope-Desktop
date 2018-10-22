using System;
using System.Threading.Tasks;
using Trezor.Net;
using Zenject;

public sealed class OpenTrezorWalletMenu : OpenHardwareWalletMenu<OpenTrezorWalletMenu, TrezorWallet>
{
    public event Action TrezorPINSectionOpening;
    public event Action ReloadPINSection;

    public event Action CheckingPIN;

    private TrezorWallet trezorWallet;

    private bool pinSectionOpen;

    public TrezorPINSection TrezorPINSection { get; private set; }

    [Inject]
    public void Construct(TrezorWallet trezorWallet)
    {
        this.trezorWallet = trezorWallet;
    }

    protected override void OnAwake()
    {
        TrezorPINSection = GetComponentInChildren<TrezorPINSection>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        trezorWallet.PINIncorrect += PINIncorrect;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        trezorWallet.PINIncorrect -= PINIncorrect;
    }

    public void UpdatePINSection()
    {
        if (!pinSectionOpen)
        {
            pinSectionOpen = true;

            MainThreadExecutor.QueueAction(() => TrezorPINSectionOpening?.Invoke());
        }
        else
        {
            MainThreadExecutor.QueueAction(() => ReloadPINSection?.Invoke());
        }
    }

    public void CheckPIN()
    {
        MainThreadExecutor.QueueAction(() => CheckingPIN?.Invoke());
    }

    protected override async Task<bool> IsHardwareWalletConnected()
    {
        var trezorManager = await Task<TrezorManager>.Factory.StartNew(() => TrezorConnector.GetWindowsConnectedTrezor(null)).ConfigureAwait(false);

        return trezorManager != null;
    }

    private void PINIncorrect()
    {
        TrezorPINSection.PINInputField.Error = true;
        TrezorPINSection.PINInputField.Text = string.Empty;
        //TrezorPINSection.PINInputField.UpdateVisuals();
    }
}