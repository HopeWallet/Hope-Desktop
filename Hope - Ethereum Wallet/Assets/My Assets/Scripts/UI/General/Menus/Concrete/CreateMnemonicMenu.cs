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
public sealed class CreateMnemonicMenu : Menu<CreateMnemonicMenu>
{
    public GameObject[] objects;

    public Button confirmButton;
    public Button backButton;
    public Button copyMnemonic;
    public Button generateNewWords;

    private DynamicDataCache dynamicDataCache;

    private readonly List<TextMeshProUGUI> wordFields = new List<TextMeshProUGUI>();

    /// <summary>
    /// Adds the DynamicDataCache dependency.
    /// </summary>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
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
    /// Opens the confirm words menu.
    /// </summary>
    public void ConfirmWords()
    {

    }

    /// <summary>
    /// Generates the mnemonic phrase.
    /// </summary>
    public void GenerateMnemonic()
    {
        dynamicDataCache.SetData(new ProtectedString(new Wallet(Wordlist.English, WordCount.Twelve).Phrase), 2);
    }

    /// <summary>
    /// Copies the mnemonic phrase to the clipboard.
    /// </summary>
    public void CopyMnemonic()
    {
        using (var mnemonic = (dynamicDataCache.GetData(2) as ProtectedString)?.CreateDisposableData())
            ClipboardUtils.CopyToClipboard(mnemonic.Value);
    }

    /// <summary>
    /// Updates the word text objects with the initial mnemonic phrase.
    /// </summary>
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
