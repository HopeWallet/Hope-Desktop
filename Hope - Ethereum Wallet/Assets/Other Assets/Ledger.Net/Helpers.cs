using NBitcoin;
using System;
using System.IO;

namespace Ledger.Net
{
    public static class Helpers
    {
        #region Public Methods
        public static byte[] GetDerivationPathData(string path)
        {
            return GetDerivationPathByteData(new KeyPath(path).Indexes);
        }
        #endregion

        #region Internal Methods
        internal static byte[] GetRequestDataPacket(Stream stream, int packetIndex)
        {
            using (var returnStream = new MemoryStream())
            {
                var position = (int)returnStream.Position;
                returnStream.WriteByte((Constants.DEFAULT_CHANNEL >> 8) & 0xff);
                returnStream.WriteByte(Constants.DEFAULT_CHANNEL & 0xff);
                returnStream.WriteByte(Constants.TAG_APDU);
                returnStream.WriteByte((byte)((packetIndex >> 8) & 0xff));
                returnStream.WriteByte((byte)(packetIndex & 0xff));

                if (packetIndex == 0)
                {
                    returnStream.WriteByte((byte)((stream.Length >> 8) & 0xff));
                    returnStream.WriteByte((byte)(stream.Length & 0xff));
                }

                var headerLength = (int)(returnStream.Position - position);
                var blockLength = Math.Min(Constants.LEDGER_HID_PACKET_SIZE - headerLength, (int)stream.Length - (int)stream.Position);

                var packetBytes = stream.ReadAllBytes(blockLength);

                returnStream.Write(packetBytes, 0, packetBytes.Length);

                while ((returnStream.Length % Constants.LEDGER_HID_PACKET_SIZE) != 0)
                {
                    returnStream.WriteByte(0);
                }

                return returnStream.ToArray();
            }
        }

        internal static byte[] GetResponseDataPacket(byte[] data, int packetIndex, ref int remaining)
        {
            using (var returnStream = new MemoryStream())
            using (var input = new MemoryStream(data))
            {
                var position = (int)input.Position;
                var channel = input.ReadAllBytes(2);

                int thirdByte = input.ReadByte();
                if (thirdByte != Constants.TAG_APDU)
                {
                    return null;
                }

                int fourthByte = input.ReadByte();
                if (fourthByte != ((packetIndex >> 8) & 0xff))
                {
                    return null;
                }

                int fifthByte = input.ReadByte();
                if (fifthByte != (packetIndex & 0xff))
                {
                    return null;
                }

                if (packetIndex == 0)
                {
                    remaining = ((input.ReadByte()) << 8);
                    remaining |= input.ReadByte();
                }

                var headerSize = input.Position - position;
                var blockSize = (int)Math.Min(remaining, Constants.LEDGER_HID_PACKET_SIZE - headerSize);

                var commandPart = new byte[blockSize];

                if (input.Read(commandPart, 0, commandPart.Length) != commandPart.Length)
                {
                    return null;
                }

                returnStream.Write(commandPart, 0, commandPart.Length);

                remaining -= blockSize;

                return returnStream.ToArray();
            }
        }
        #endregion

        #region Private Methods
        private static byte[] GetDerivationPathByteData(uint[] indices)
        {
            byte[] addressIndicesData;
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.WriteByte((byte)indices.Length);
                for (var i = 0; i < indices.Length; i++)
                {
                    var data = indices[i].ToBytes();
                    memoryStream.Write(data, 0, data.Length);
                }
                addressIndicesData = memoryStream.ToArray();
            }

            return addressIndicesData;
        }
        #endregion
    }
}
