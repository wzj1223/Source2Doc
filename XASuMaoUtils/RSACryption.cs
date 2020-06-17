using System;
using System.Security.Cryptography;
using System.Text;

namespace XASuMaoUtils
{
  internal class RSACryption
  {
    private string PublicKey = "<RSAKeyValue><Modulus>7xTruEHV2AdlTKAfJAN5szCZe40VBGunCSTZS6XLUf/+HNj5yBwR407GsyaihpzW21PH42OveWXhPK8aX+BljdjZ9IwMgIV7/AUk0CD4ROShyl5OEDQfIy99h3hWDWev3hMr/kjqzUm6JFWRrPWE+fesd0u/QPKUdqN1A7kxUj8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    private bool GetHash(string strSource, ref byte[] HashData)
    {
      try
      {
        HashAlgorithm hashAlgorithm = HashAlgorithm.Create("MD5");
        byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(strSource);
        HashData = hashAlgorithm.ComputeHash(bytes);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public bool SignatureDeformatterWithMD5(string Deformatter, string SignData)
    {
      try
      {
        byte[] HashData = new byte[256];
        this.GetHash(Deformatter, ref HashData);
        byte[] DeformatterData = Convert.FromBase64String(SignData);
        return this.SignatureDeformatter(HashData, DeformatterData, "");
      }
      catch
      {
        return false;
      }
    }

    private bool SignatureDeformatter(
      byte[] HashbyteDeformatter,
      byte[] DeformatterData,
      string strKeyPublic = "")
    {
      try
      {
        if (strKeyPublic == "")
          strKeyPublic = this.PublicKey;
        RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider();
        cryptoServiceProvider.FromXmlString(strKeyPublic);
        RSAPKCS1SignatureDeformatter signatureDeformatter = new RSAPKCS1SignatureDeformatter((AsymmetricAlgorithm) cryptoServiceProvider);
        signatureDeformatter.SetHashAlgorithm("MD5");
        return signatureDeformatter.VerifySignature(HashbyteDeformatter, DeformatterData);
      }
      catch
      {
        return false;
      }
    }
  }
}
