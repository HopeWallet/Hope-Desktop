﻿using System;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;

namespace Nethereum.RPC.TransactionManagers
{
    public class EtherTransferTransactionInputBuilder
    {
        public static TransactionInput CreateTransactionInput(string fromAddress, string toAddress, decimal etherAmount, decimal? gasPriceGwei = null, BigInteger? gas = null)
        {
            if (toAddress == null) throw new ArgumentNullException(nameof(toAddress));
            if (etherAmount <= 0) throw new ArgumentOutOfRangeException(nameof(etherAmount));
            if (gasPriceGwei <= 0) throw new ArgumentOutOfRangeException(nameof(gasPriceGwei));

            var transactionInput = new TransactionInput()
            {
                To = toAddress,
                From = fromAddress,
                GasPrice = gasPriceGwei == null ? null : new HexBigInteger(UnitConversion.Convert.ToWei(gasPriceGwei.Value, UnitConversion.EthUnit.Gwei)),
                Value = new HexBigInteger(UnitConversion.Convert.ToWei(etherAmount)),
                Gas = gas == null ? null : new HexBigInteger(gas.Value)
            };
            return transactionInput;
        }
    }
}