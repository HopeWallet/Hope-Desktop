using Hid.Net;
using Ledger.Net.Requests;
using Ledger.Net.Responses;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ledger.Net
{
    public sealed class LedgerManager
    {
        private readonly SemaphoreSlim _SemaphoreSlim = new SemaphoreSlim(1, 1);

        public IHidDevice LedgerHidDevice { get; }
        public ICoinUtility CoinUtility { get; }
        public ICoinInfo CurrentCoin { get; private set; }

        public LedgerManager(IHidDevice ledgerHidDevice) : this(ledgerHidDevice, null)
        {
        }

        public LedgerManager(IHidDevice ledgerHidDevice, ICoinUtility coinUtility)
        {
            LedgerHidDevice = ledgerHidDevice;
            CoinUtility = coinUtility ?? new DefaultCoinUtility();

            SetCoinNumber(60);
        }

        private async Task WriteRequestAsync<TWrite>(TWrite message) where TWrite : RequestBase
        {
            var packetIndex = 0;
            byte[] data = null;
            using (var memoryStream = new MemoryStream(message.ToAPDU()))
            {
                do
                {
                    data = Helpers.GetRequestDataPacket(memoryStream, packetIndex);
                    packetIndex++;
                    await LedgerHidDevice.WriteAsync(data);
                } while (memoryStream.Position != memoryStream.Length);
            }
        }

        private async Task<byte[]> ReadResponseAsync()
        {
            var remaining = 0;
            var packetIndex = 0;
            using (var response = new MemoryStream())
            {
                do
                {
                    var packet = await LedgerHidDevice.ReadAsync().ConfigureAwait(false);

                    var responseDataPacket = Helpers.GetResponseDataPacket(packet, packetIndex, ref remaining);
                    packetIndex++;

                    if (responseDataPacket == null)
                    {
                        return null;
                    }

                    response.Write(responseDataPacket, 0, responseDataPacket.Length);

                } while (remaining != 0);

                return response.ToArray();
            }
        }

        private async Task<TResponse> SendRequestAsync<TResponse>(RequestBase request) where TResponse : ResponseBase
        {
            await _SemaphoreSlim.WaitAsync();

            try
            {
                await WriteRequestAsync(request);
                var responseData = await ReadResponseAsync();
                return (TResponse)Activator.CreateInstance(typeof(TResponse), responseData);
            }
            finally
            {
                _SemaphoreSlim.Release();
            }
        }

        public void SetCoinNumber(uint coinNumber)
        {
            CurrentCoin = CoinUtility.GetCoinInfo(coinNumber);
        }

        /// <summary>
        /// This will set the coin based on the currently open app. Note: this only currently works with Bitcoin based Ledger apps.
        /// </summary>
        public async Task SetCoinNumber()
        {
            var getCoinVersionRequest = await SendRequestAsync<GetCoinVersionResponse, GetCoinVersionRequest>(new GetCoinVersionRequest());
            CurrentCoin = CoinUtility.GetCoinInfo(getCoinVersionRequest.ShortCoinName);
        }

        public async Task<EthereumAppGetPublicKeyResponse> GetPublicKeyResponse(string path, bool getChainCode, bool showDisplay)
        {
            byte[] data = Helpers.GetDerivationPathData(path);

            EthereumAppGetPublicKeyResponse response = await SendRequestAsync<EthereumAppGetPublicKeyResponse, EthereumAppGetPublicKeyRequest>(new EthereumAppGetPublicKeyRequest(showDisplay, getChainCode, data));

            return response.IsSuccess ? response : null;
        }

        public Task<TResponse> SendRequestAsync<TResponse, TRequest>(TRequest request)
           where TResponse : ResponseBase
           where TRequest : RequestBase
        {
            return SendRequestAsync<TResponse>(request);
        }
    }
}
