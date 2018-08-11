using Hope.Security.ProtectedTypes.Types;
using Hope.Utils.Ethereum;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu which appears which allows the user to confirm several words of the mnemonic phrase to verify it has been saved in some form.
/// </summary>
public sealed class ConfirmMnemonicMenu : WalletLoadMenuBase<ConfirmMnemonicMenu>
{
    public Button backButton;

    private DynamicDataCache dynamicDataCache;

    /// <summary>
    /// Adds the DynamicDataCache dependency.
    /// </summary>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    [Inject]
    public void Construct(DynamicDataCache dynamicDataCache) => this.dynamicDataCache = dynamicDataCache;

    /// <summary>
    /// Gets the word numbers and words of the part of the mnemonic that needs confirming.
    /// </summary>
    protected override void OnAwake()
    {
        GetConfirmationNumbers();
        GetConfirmationWords();
    }

	/// <summary>
	/// Opens the exit confirmation popup and enables the note text.
	/// </summary>
	protected override void OpenExitConfirmationPopup() => popupManager.GetPopup<ExitConfirmationPopup>(true).SetDetails(true);

	/// <summary>
	/// Adds the back button listener.
	/// </summary>
	private void Start()
    {
        backButton.onClick.AddListener(GoBack);
    }

    /// <summary>
    /// Starts to load the wallet.
    /// </summary>
    public override void LoadWallet()
    {
        userWalletManager.CreateWallet();
    }

    /// <summary>
    /// Gets the numbers of the words that need to be confirmed.
    /// </summary>
    private void GetConfirmationNumbers()
    {
        int[] numbers;

        do
        {
            numbers = new int[4] { Random.Range(1, 13), Random.Range(1, 13), Random.Range(1, 13), Random.Range(1, 13) };
        } while (numbers.Distinct().Count() < 4);

        dynamicDataCache.SetData("confirmation numbers", numbers);
    }

    /// <summary>
    /// Gets the words that need to be confirmed.
    /// </summary>
    [SecureCallEnd]
    [ReflectionProtect]
    private void GetConfirmationWords()
    {
        ProtectedString[] correctWords;
        int[] numbers = dynamicDataCache.GetData("confirmation numbers");

        using (var mnemonic = (dynamicDataCache.GetData("mnemonic") as ProtectedString)?.CreateDisposableData())
        {
            List<int> randomIntList = numbers.ToList();
            List<string> words = WalletUtils.GetMnemonicWords(mnemonic.Value).ToList();

            correctWords = words.Where(word => numbers.Contains(words.IndexOf(word) + 1))
                                .OrderBy(word => randomIntList.IndexOf(words.IndexOf(word) + 1))
                                .Select(word => new ProtectedString(word))
                                .ToArray();
        }

        dynamicDataCache.SetData("confirmation words", correctWords);
    }
}
