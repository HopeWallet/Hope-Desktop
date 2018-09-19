using System;

public sealed class CurrencyManager
{
    public event Action OnCurrencyChanged;

    private readonly Settings settings;

    private readonly string[] currencySymbols = new string[]
    {
        "$",
        "R$",
        "$",
        "",
        "$",
        "¥",
        "Kč",
        "Kr.",
        "€",
        "£",
        "$",
        "Ft",
        "Rp",
        "₪",
        "₹",
        "¥",
        "₩",
        "$",
        "RM",
        "kr",
        "$",
        "₱",
        "Rs",
        "zł",
        "₽",
        "kr",
        "$",
        "฿",
        "₺",
        "NT$",
        "$",
        "R"
    };

    public string ActiveCurrencySymbol => currencySymbols[(int)ActiveCurrency];

    public CurrencyType ActiveCurrency { get; private set; }

    public CurrencyManager(Settings settings)
    {
        this.settings = settings;

        if (!SecurePlayerPrefs.HasKey(settings.prefName))
            SecurePlayerPrefs.SetInt(settings.prefName, (int)CurrencyType.USD);

        ActiveCurrency = (CurrencyType)SecurePlayerPrefs.GetInt(settings.prefName);
    }

    public void SwitchActiveCurrency(CurrencyType newActiveCurrency)
    {
        ActiveCurrency = newActiveCurrency;
        SecurePlayerPrefs.SetInt(settings.prefName, (int)newActiveCurrency);

        OnCurrencyChanged?.Invoke();
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

    [Serializable]
    public sealed class Settings
    {
        [RandomizeText] public string prefName;
    }
}