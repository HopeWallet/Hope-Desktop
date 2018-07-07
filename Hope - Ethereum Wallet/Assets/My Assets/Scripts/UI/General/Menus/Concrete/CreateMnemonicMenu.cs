using Hope.Security.ProtectedTypes.Types;
using Hope.Utils.EthereumUtils;
using NBitcoin;
using Nethereum.HdWallet;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class used for creating a new ethereum wallet.
/// </summary>
public class CreateMnemonicMenu : Menu<CreateMnemonicMenu>
{
    public GameObject[] objects;

    public Button confirmButton;
    public Button backButton;
    public Button copyMnemonic;
    public Button generateNewWords;

    private DynamicDataCache dynamicDataCache;

    private readonly List<TextMeshProUGUI> wordFields = new List<TextMeshProUGUI>();

    [Inject]
    public void Construct(DynamicDataCache dynamicDataCache) => this.dynamicDataCache = dynamicDataCache;

    /// <summary>
    /// Initializes this class by creating a new mnemonic phrase and setting the text to it.
    /// </summary>
    protected override void OnAwake()
    {
        confirmButton.onClick.AddListener(ConfirmWords);
        backButton.onClick.AddListener(GoBack);
        copyMnemonic.onClick.AddListener(CopyMnemonic);
        generateNewWords.onClick.AddListener(GenerateMnemonic);

        GenerateMnemonic();
        UpdateWordFields();
    }

    /// <summary>
    /// Creates a wallet with the newly created mnemonic phrase.
    /// </summary>
    //public override void LoadWallet() => userWalletManager.CreateWallet(mnemonic);

    public void ConfirmWords()
    {

    }

    public void GenerateMnemonic()
    {
        dynamicDataCache.SetData(new ProtectedString(new Wallet(Wordlist.English, WordCount.Twelve).Phrase), 2);
    }

    public void CopyMnemonic()
    {
        using (var mnemonic = (dynamicDataCache.GetData(2) as ProtectedString)?.CreateDisposableData())
            ClipboardUtils.CopyToClipboard(mnemonic.Value);
    }

    private void UpdateWordFields()
    {
        for (int i = 0; i < objects.Length; i++)
            wordFields.Add(objects[i].transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>());

        using (var mnemonic = (dynamicDataCache.GetData(2) as ProtectedString)?.CreateDisposableData())
        {
            string[] splitWords = mnemonic.Value.GetMnemonicWords();

            for (int i = 0; i < objects.Length; i++)
                wordFields[i].text = splitWords[i];
        }
    }

}
