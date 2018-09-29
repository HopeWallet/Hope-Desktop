using Nethereum.Signer;
using Transaction = Nethereum.Signer.Transaction;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Hex.HexTypes;
using System.Collections;
using System;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.Web3.Accounts;

namespace Nethereum.JsonRpc.UnityClient
{
    public class TransactionSignedUnityRequest:UnityRequest<string>
    {
        private readonly string _url;
        private readonly string _signedTxData;

        private readonly Account _account;
        private readonly TransactionSigner _transactionSigner;
        private readonly EthGetTransactionCountUnityRequest _transactionCountRequest;
        private readonly EthSendRawTransactionUnityRequest _ethSendTransactionRequest;
        private readonly EthEstimateGasUnityRequest _ethEstimateGasUnityRequest;
        private readonly EthGasPriceUnityRequest _ethGasPriceUnityRequest;

        public bool EstimateGas { get; set; } = true;

        public TransactionSignedUnityRequest(Account account, string url)
        {
            _url = url;
            _account = account;
            _transactionSigner = new TransactionSigner(); 
            _ethSendTransactionRequest = new EthSendRawTransactionUnityRequest(_url);
            _transactionCountRequest = new EthGetTransactionCountUnityRequest(_url);
            _ethEstimateGasUnityRequest = new EthEstimateGasUnityRequest(_url);
            _ethGasPriceUnityRequest = new EthGasPriceUnityRequest(_url);
        }

        public TransactionSignedUnityRequest(string signedTxData, string url)
        {
            _url = url;
            _signedTxData = signedTxData;
            _ethSendTransactionRequest = new EthSendRawTransactionUnityRequest(_url);
        }

        public IEnumerator SignAndSendTransaction<TContractFunction>(TContractFunction function, string contractAdress) where TContractFunction : FunctionMessage
        {
            var transactionInput = function.CreateTransactionInput(contractAdress);
            yield return SignAndSendTransaction(transactionInput);
        }

        public IEnumerator SignAndSendDeploymentContractTransaction<TDeploymentMessage>(TDeploymentMessage deploymentMessage)
            where TDeploymentMessage : ContractDeploymentMessage
        {
            var transactionInput = deploymentMessage.CreateTransactionInput();
            yield return SignAndSendTransaction(transactionInput);
        }

        public IEnumerator SignAndSendDeploymentContractTransaction<TDeploymentMessage>()
            where TDeploymentMessage : ContractDeploymentMessage, new()
        {
            var deploymentMessage = new TDeploymentMessage();
            yield return SignAndSendDeploymentContractTransaction(deploymentMessage);
        }

        public IEnumerator SignAndSendTransaction(TransactionInput transactionInput)
        {
            if (transactionInput == null) throw new ArgumentNullException("transactionInput");

            if (!string.IsNullOrEmpty(_signedTxData))
            {
                yield return _ethSendTransactionRequest.SendRequest(_signedTxData);
            }
            else
            {
                if (transactionInput.Gas == null)
                {
                    if (EstimateGas)
                    {
                        yield return _ethEstimateGasUnityRequest.SendRequest(transactionInput);

                        if (_ethEstimateGasUnityRequest.Exception == null)
                        {
                            var gas = _ethEstimateGasUnityRequest.Result;
                            transactionInput.Gas = gas;
                        }
                        else
                        {
                            this.Exception = _ethEstimateGasUnityRequest.Exception;
                            yield break;
                        }
                    }
                    else
                    {
                        transactionInput.Gas = new HexBigInteger(Transaction.DEFAULT_GAS_LIMIT);
                    }
                }

                if (transactionInput.GasPrice == null)
                {
                    yield return _ethGasPriceUnityRequest.SendRequest();

                    if (_ethGasPriceUnityRequest.Exception == null)
                    {
                        var gasPrice = _ethGasPriceUnityRequest.Result;
                        transactionInput.GasPrice = gasPrice;
                    }
                    else
                    {
                        this.Exception = _ethGasPriceUnityRequest.Exception;
                        yield break;
                    }
                }

                var nonce = transactionInput.Nonce;

                if (nonce == null)
                {
                    yield return _transactionCountRequest.SendRequest(_account.Address, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

                    if (_transactionCountRequest.Exception == null)
                    {
                        nonce = _transactionCountRequest.Result;
                    }
                    else
                    {
                        this.Exception = _transactionCountRequest.Exception;
                        yield break;
                    }
                }

                if (string.IsNullOrEmpty(transactionInput.From))
                    transactionInput.From = _account.Address;

                var value = transactionInput.Value;
                if (value == null)
                    value = new HexBigInteger(0);

                var signedTransaction = _transactionSigner.SignTransaction(_account.PrivateKey, transactionInput.To, value.Value, nonce,
                    transactionInput.GasPrice.Value, transactionInput.Gas.Value, transactionInput.Data);

                yield return _ethSendTransactionRequest.SendRequest(signedTransaction);

                _account.PrivateKey.ClearBytes();
                GC.Collect();
            }

            if (_ethSendTransactionRequest.Exception == null)
            {
                Result = _ethSendTransactionRequest.Result;
            }
            else
            {
                Exception = _ethSendTransactionRequest.Exception;
            }
        }
    }
}
