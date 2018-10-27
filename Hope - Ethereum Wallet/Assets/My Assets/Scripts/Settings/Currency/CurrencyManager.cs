using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

public sealed class CurrencyManager
{
    public event Action OnCurrencyChanged;

    private readonly string[] euroTypes = new string[] { "ca", "ca-ES-valencia", "de", "de-de", "el", "es", "es-ES", "et", "eu", "fi", "fr", "fr-FR", "ga", "it", "it-it", "lb", "lt", "lv", "mt", "nl", "nl-BE", "pt-PT", "sk", "sl" };
    private readonly string[] poundTypes = new string[] { "cy", "en-GB", "gd-Latn" };
    private readonly string[] rupeeTypes = new string[] { "as", "bn-IN", "hi", "kn", "kok", "ml", "mr", "or", "ta", "te" };
    private readonly string[] kroneTypes = new string[] { "nb", "nn" };

    public CurrencyType ActiveCurrency { get; private set; }

    public CurrencyManager()
    {
        if (!SecurePlayerPrefs.HasKey(PlayerPrefConstants.SETTING_CURRENCY))
            SecurePlayerPrefs.SetInt(PlayerPrefConstants.SETTING_CURRENCY, (int)CurrencyType.USD);

        ActiveCurrency = (CurrencyType)SecurePlayerPrefs.GetInt(PlayerPrefConstants.SETTING_CURRENCY);
    }

    public void SwitchActiveCurrency(CurrencyType newActiveCurrency)
    {
        ActiveCurrency = newActiveCurrency;
        SecurePlayerPrefs.SetInt(PlayerPrefConstants.SETTING_CURRENCY, (int)newActiveCurrency);

        OnCurrencyChanged?.Invoke();
    }

    public string GetCurrencyFormattedValue(decimal value)
    {
        switch (ActiveCurrency)
        {
            case CurrencyType.AUD:
            case CurrencyType.CAD:
            case CurrencyType.CLP:
            case CurrencyType.HKD:
            case CurrencyType.MXN:
            case CurrencyType.NZD:
            case CurrencyType.SGD:
            case CurrencyType.USD:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("en-US")) + " <style=Symbol>" + ActiveCurrency + "</style>";
            case CurrencyType.BRL:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
            case CurrencyType.CHF:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("en-US")).Replace("$", "fr");
            case CurrencyType.CNY:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("zh-Hans")).Replace("￥", "¥");
            case CurrencyType.CZK:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("cs"));
            case CurrencyType.DKK:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("da"));
            case CurrencyType.EUR:
                return value.ToString("C", GetCurrentCulture(euroTypes, "de"));
            case CurrencyType.GBP:
                return value.ToString("C", GetCurrentCulture(poundTypes, "en-GB"));
            case CurrencyType.HUF:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("hu"));
            case CurrencyType.IDR:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("id"));
            case CurrencyType.ILS:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("he"));
            case CurrencyType.INR:
                return value.ToString("C", GetCurrentCulture(rupeeTypes, "bn-IN"));
            case CurrencyType.JPY:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("ja")).Replace("￥", "¥");
            case CurrencyType.KRW:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("ko"));
            case CurrencyType.MYR:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("en-US")).Replace("$", "RM");
            case CurrencyType.NOK:
                return value.ToString("C", GetCurrentCulture(kroneTypes, "nn"));
            case CurrencyType.PHP:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("en-US")).Replace("$", "₱");
            case CurrencyType.PKR:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("en-US")).Replace("$", "Rs");
            case CurrencyType.PLN:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("pl"));
            case CurrencyType.RUB:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("fil-Latn"));
            case CurrencyType.SEK:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("sv"));
            case CurrencyType.THB:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("en-US")).Replace("$", "฿");
            case CurrencyType.TRY:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("tr"));
            case CurrencyType.TWD:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("en-US")).Replace("$", "NT$");
            case CurrencyType.ZAR:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("en-US")).Replace("$", "R");
            default:
                return value.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
        }
    }

    private CultureInfo GetCurrentCulture(string[] validCultureNames, string fallbackCultureName)
    {
        var currentCulture = Array.Find(CultureInfo.GetCultures(CultureTypes.AllCultures), x => x.EnglishName.Equals(Application.systemLanguage.ToString()));
        return validCultureNames.Contains(currentCulture.Name) ? currentCulture : CultureInfo.CreateSpecificCulture(fallbackCultureName);
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
}