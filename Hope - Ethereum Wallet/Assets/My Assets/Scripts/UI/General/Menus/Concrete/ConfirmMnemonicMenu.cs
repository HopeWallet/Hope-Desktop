using Hope.Security.ProtectedTypes.Types;
using Hope.Utils.EthereumUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed class ConfirmMnemonicMenu : WalletLoadMenuBase<ConfirmMnemonicMenu>
{

    public Button backButton;

    private DynamicDataCache dynamicDataCache;

    [Inject]
    public void Construct(DynamicDataCache dynamicDataCache) => this.dynamicDataCache = dynamicDataCache;

    protected override void OnAwake()
    {
        GetConfirmationNumbers();
        GetConfirmationWords();
    }

    private void Start()
    {
        backButton.onClick.AddListener(GoBack);
    }

    public override void LoadWallet()
    {

    }

    private void GetConfirmationNumbers()
    {
        int[] numbers;

        do
        {
            numbers = new int[4] { Random.Range(1, 13), Random.Range(1, 13), Random.Range(1, 13), Random.Range(1, 13) };
        } while (numbers.Distinct().Count() < 4);

        dynamicDataCache.SetData("confirmation numbers", numbers);
    }

    private void GetConfirmationWords()
    {
        ProtectedString[] correctWords;
        int[] numbers = dynamicDataCache.GetData("confirmation numbers");

        using (var mnemonic = (dynamicDataCache.GetData("mnemonic") as ProtectedString)?.CreateDisposableData())
        {
            List<int> randomIntList = numbers.ToList();
            List<string> words = mnemonic.Value.GetMnemonicWords().ToList();

            correctWords = words.Where(word => numbers.Contains(words.IndexOf(word) + 1))
                                .OrderBy(word => randomIntList.IndexOf(words.IndexOf(word) + 1))
                                .Select(word => new ProtectedString(word))
                                .ToArray();
        }

        dynamicDataCache.SetData("confirmation words", correctWords);
    }

}
