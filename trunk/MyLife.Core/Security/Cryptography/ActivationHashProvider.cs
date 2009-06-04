using System;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration;

namespace MyLife.Security.Cryptography
{
    [ConfigurationElementType(typeof (CustomHashProviderData))]
    public class ActivationHashProvider : IHashProvider
    {
        #region IHashProvider Members

        public bool CompareHash(byte[] plaintext, byte[] hashedtext)
        {
            throw new NotImplementedException();
        }

        public byte[] CreateHash(byte[] plaintext)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}