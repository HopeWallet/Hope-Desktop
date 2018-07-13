using System.Collections.Generic;
using System.Threading.Tasks;

public class CoinList
{
	private readonly Dictionary<string, int> coinIDs = new Dictionary<string, int>();

	private const string API_URL = "https://api.coinmarketcap.com/v2/listings/";

	public CoinList()
	{
		GetCoinList();
	}

	public int? GetCoinID(string symbol)
	{
		if (!coinIDs.ContainsKey(symbol))
			return null;

		return coinIDs[symbol];
	}

	private void GetCoinList()
	{
		WebClientUtils.DownloadString(API_URL, CopyData);
	}

	private async void CopyData(string coinList)
	{
		CMCData cmcData = await Task.Run(() => JsonUtils.GetJsonData<CMCData>(coinList)).ConfigureAwait(false);

		await Task.Run(() => cmcData.data.ForEach(coin => coinIDs.Add(coin.symbol, coin.id))).ConfigureAwait(false);
	}
}
