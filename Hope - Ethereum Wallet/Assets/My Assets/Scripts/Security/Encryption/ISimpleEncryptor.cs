using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ISimpleEncryptor
{
    byte[] Encrypt(byte[] data, byte[] entropy);

    byte[] Decrypt(byte[] encryptedData, byte[] entropy);
}