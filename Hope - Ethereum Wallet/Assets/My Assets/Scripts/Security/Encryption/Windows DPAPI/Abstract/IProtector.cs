using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IProtector
{

    void Protect(string data);

    void Protect(byte[] data);

    void Protect(string data, string entropy);

    void Protect(string data, byte[] entropy);

    void Protect(byte[] data, string entropy);

    void Protect(byte[] data, byte[] entropy);

    void Unprotect(string encryptedData);

    void Unprotect(byte[] encryptedData);

    void Unprotect(string encryptedData, string entropy);

    void Unprotect(string encryptedData, byte[] entropy);

    void Unprotect(byte[] data, string entropy);

    void Unprotect(byte[] data, byte[] entropy);

}