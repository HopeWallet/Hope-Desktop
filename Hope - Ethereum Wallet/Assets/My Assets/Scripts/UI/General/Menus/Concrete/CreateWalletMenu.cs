using Hope.Security.ProtectedTypes.Types;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu which lets the user create a new wallet by first choosing a password and name for the wallet.
/// </summary>
public sealed class CreateWalletMenu : Menu<CreateWalletMenu>
{

    [SerializeField] private Button createWalletButton;
	[SerializeField] private Button backButton;
	[SerializeField] private TMP_InputField walletNameField;
	[SerializeField] private TMP_InputField passwordField;

	[SerializeField] private InfoMessage walletNameInfoIcon;
	[SerializeField] private InfoMessage walletNameErrorIcon;
	[SerializeField] private InfoMessage passwordErrorIcon;

	private CreateWalletMenuAnimator createWalletMenuAnimator;
    private DynamicDataCache dynamicDataCache;

	public UserWalletInfoManager UserWalletInfoManager { get; private set; }

	public string PasswordErrorMessage
	{
		set { passwordErrorIcon.bodyText = value; }
	}

	public string WalletNameErrorMessage
	{
		set { walletNameErrorIcon.bodyText = value; }
	}

	/// <summary>
	/// Adds the required dependencies into this class.
	/// </summary>
	/// <param name="dynamicDataCache"> The active ProtectedStringDataCache. </param>
	/// <param name="userWalletInfoManager"> The active UserWalletInfoManager. </param>
	[Inject]
    public void Construct(DynamicDataCache dynamicDataCache, UserWalletInfoManager userWalletInfoManager, PopupManager popupManager)
	{
		this.dynamicDataCache = dynamicDataCache;
		this.UserWalletInfoManager = userWalletInfoManager;

		walletNameInfoIcon.PopupManager = popupManager;
		walletNameErrorIcon.PopupManager = popupManager;
		passwordErrorIcon.PopupManager = popupManager;
	}

	/// <summary>
	/// Adds the button listeners.
	/// </summary>
	protected override void OnAwake()
	{
		createWalletMenuAnimator = transform.GetComponent<CreateWalletMenuAnimator>();

		createWalletButton.onClick.AddListener(CreateWalletNameAndPass);
		backButton.onClick.AddListener(GoBack);
	}

	/// <summary>
	/// Sets up the wallet name and password and opens the next menu.
	/// </summary>
	private void CreateWalletNameAndPass()
    {
        dynamicDataCache.SetData("pass", new ProtectedString(passwordField.text));
        dynamicDataCache.SetData("name", walletNameField.text);

        uiManager.OpenMenu<ImportOrCreateMnemonicMenu>();
    }
}