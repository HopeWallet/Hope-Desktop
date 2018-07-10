using Hope.Security.ProtectedTypes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using Zenject;

public sealed class UnlockWalletMenu : WalletLoadMenuBase<UnlockWalletMenu>
{

    public Button unlockWalletButton,
                  backButton;

    public TMP_InputField passwordField;

    private DynamicDataCache dynamicDataCache;

    [Inject]
    public void Construct(DynamicDataCache dynamicDataCache) => this.dynamicDataCache = dynamicDataCache;

    protected override void OnAwake()
    {
        unlockWalletButton.onClick.AddListener(LoadWallet);
        backButton.onClick.AddListener(GoBack);
    }

    public override void LoadWallet()
    {
        dynamicDataCache.SetData("pass", new ProtectedString(passwordField.text));
        userWalletManager.UnlockWallet();
    }
}