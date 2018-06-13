using System;
using System.Linq;
using System.Numerics;
using Nethereum.ABI.Decoders;
using Nethereum.ABI.Util;

namespace Nethereum.ABI.Encoders
{
    public class IntTypeEncoder : ITypeEncoder
    {
        private readonly IntTypeDecoder intTypeDecoder;

        public IntTypeEncoder()
        {
            intTypeDecoder = new IntTypeDecoder();
        }

        public byte[] Encode(object value)
        {
            BigInteger bigInt;

            var stringValue = value as string;

            if (stringValue != null)
                bigInt = intTypeDecoder.Decode<BigInteger>(stringValue);
            else if (value is BigInteger)
                bigInt = (BigInteger) value;
            else if (value.IsNumber())
                bigInt = BigInteger.Parse(value.ToString());
            else
                throw new Exception("Invalid value for type '" + this + "': " + value + " (" + value.GetType() + ")");
            return EncodeInt(bigInt);
        }

        public byte[] EncodeInt(int i)
        {
            return EncodeInt(new BigInteger(i));
        }

        public byte[] EncodeInt(BigInteger bigInt)
        {
            var ret = new byte[32];

            for (var i = 0; i < ret.Length; i++)
                if (bigInt.Sign < 0)
                    ret[i] = 0xFF;
                else
                    ret[i] = 0;

            byte[] bytes;

            //It should always be Big Endian.
            if (BitConverter.IsLittleEndian)
                bytes = bigInt.ToByteArray().Reverse().ToArray();
            else
                bytes = bigInt.ToByteArray().ToArray();

            Array.Copy(bytes, 0, ret, 32 - bytes.Length, bytes.Length);

            return ret;
        }
    }
}