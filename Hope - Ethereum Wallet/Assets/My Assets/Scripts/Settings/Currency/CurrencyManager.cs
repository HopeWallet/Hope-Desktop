using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class CurrencyManager
{
    private readonly Settings settings;

    public CurrencyType ActiveCurrency { get; private set; }

    public CurrencyManager(Settings settings)
    {
        this.settings = settings;

        if (!SecurePlayerPrefs.HasKey(settings.prefName))
            SecurePlayerPrefs.SetInt(settings.prefName, (int)CurrencyType.USD);


    }

    public void SwitchActiveCurrency()
    {

    }

    public enum CurrencyType
    {
        AUD,
        BRL,
        CAD,
        CHF,
        CLP,
        CNY,
        CZK,
        DKK,
        EUR,
        GBP,
        HKD,
        HUF,
        IDR,
        ILS,
        INR,
        JPY,
        KRW,
        MXN,
        MYR,
        NOK,
        NZD,
        PHP,
        PKR,
        PLN,
        RUB,
        SEK,
        SGD,
        THB,
        TRY,
        TWD,
        USD,
        ZAR
    }

    public sealed class Settings
    {
        [RandomizeText] public string prefName;
    }
}