using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InfraInterfaces
{
    public interface IEncryptionService
    {
        (bool, string) AES_DecryptV2(string cipherText);
        string AES_EncryptV2(string text);
        string SHA512(string input);
    }
}
