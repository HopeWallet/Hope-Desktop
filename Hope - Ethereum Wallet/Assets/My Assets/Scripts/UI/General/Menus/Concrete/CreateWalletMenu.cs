using Hope.Security.ProtectedTypes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using Zenject;

public sealed class CreateWalletMenu : Menu<CreateWalletMenu>
{

    public Button createWalletButton;
    public Button backButton;
    public TMP_InputField walletNameField;
    public TMP_InputField passwordField;

    private ProtectedStringDataCache protectedStringDataCache;

    [Inject]
    public void Construct(ProtectedStringDataCache protectedStringDataCache) => this.protectedStringDataCache = protectedStringDataCache;

    private void Start()
    {
        createWalletButton.onClick.AddListener(CreateWalletNameAndPass);
        backButton.onClick.AddListener(OnBackPressed);
    }

    private void CreateWalletNameAndPass()
    {
        protectedStringDataCache.SetData(new ProtectedString(passwordField.text), 0);
        protectedStringDataCache.SetData(new ProtectedString(walletNameField.name), 1);

        // Open next menu
    }

    public override void OnBackPressed() => uiManager.CloseMenu();

}