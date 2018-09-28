using Hope.Utils.Ethereum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

/// <summary>
/// Menu which appears which allows the user to confirm several words of the mnemonic phrase to verify it has been saved in some form.
/// </summary>
public sealed class ConfirmMnemonicMenu : WalletLoadMenuBase<ConfirmMnemonicMenu>, IEnterButtonObservable
{
	public override event Action OnWalletLoading;

	[SerializeField] private Button nextButton;

    private DynamicDataCache dynamicDataCache;
	private ButtonClickObserver buttonClickObserver;

	/// <summary>
	/// Adds the DynamicDataCache dependency.
	/// </summary>
	/// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
	/// <param name="buttonClickObserver"> The active ButtonClickObserver. </param>
	[Inject]
    public void Construct(DynamicDataCache dynamicDataCache, ButtonClickObserver buttonClickObserver)
	{
		this.dynamicDataCache = dynamicDataCache;
		this.buttonClickObserver = buttonClickObserver;
	}

	/// <summary>
	/// Subscribes this button observable.
	/// </summary>
	protected override void OnEnable()
	{
		base.OnEnable();
        GetConfirmationNumbers();
        GetConfirmationWords();
        buttonClickObserver.SubscribeObservable(this);
	}

	/// <summary>
	/// Unsubscribes this button observable.
	/// </summary>
	protected override void OnDisable()
	{
		base.OnDisable();
		buttonClickObserver.UnsubscribeObservable(this);
	}

	/// <summary>
	/// Opens the exit confirmation popup and enables the note text.
	/// </summary>
	protected override void OpenExitConfirmationPopup() => popupManager.GetPopup<ExitConfirmationPopup>(true)?.SetDetails(true);

	/// <summary>
	/// Starts to load the wallet.
	/// </summary>
	public override void LoadWallet()
	{
		OnWalletLoading?.Invoke();
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
        string[] correctWords;
        int[] numbers = dynamicDataCache.GetData("confirmation numbers");

        List<int> randomIntList = numbers.ToList();
        List<string> words = WalletUtils.GetMnemonicWords((string)dynamicDataCache.GetData("mnemonic")).ToList();

        correctWords = words.Where(word => numbers.Contains(words.IndexOf(word) + 1))
                            .OrderBy(word => randomIntList.IndexOf(words.IndexOf(word) + 1))
                            .ToArray();

        dynamicDataCache.SetData("confirmation words", correctWords);
    }

	public void EnterButtonPressed(ClickType clickType)
	{
		if (clickType == ClickType.Down && nextButton.interactable && !GetComponent<ConfirmMnemonicMenuAnimator>().OpeningWallet)
			nextButton.Press();
	}
}
