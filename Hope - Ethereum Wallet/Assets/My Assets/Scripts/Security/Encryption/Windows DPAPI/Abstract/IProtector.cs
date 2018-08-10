using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IProtector
{
    string Protect(string data);

    byte[] Protect(byte[] data);

    string Protect(string data, string entropy);

    string Protect(string data, byte[] entropy);

    byte[] Protect(byte[] data, string entropy);

    byte[] Protect(byte[] data, byte[] entropy);

    string Unprotect(string encryptedData);

    byte[] Unprotect(byte[] encryptedData);

    string Unprotect(string encryptedData, string entropy);

    string Unprotect(string encryptedData, byte[] entropy);

    byte[] Unprotect(byte[] encryptedData, string entropy);

    byte[] Unprotect(byte[] encryptedData, byte[] entropy);
}