using Source2Doc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace XASuMaoUtils
{
  internal class ValidReg
  {
    private string AppHDKey;
    private string AppName;
    private string AppVer;

    public ValidReg()
    {
      this.AppHDKey = HDKey.GetHDKey();
      FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
      this.AppName = versionInfo.ProductName;
      this.AppVer = versionInfo.ProductVersion;
    }

    public bool onlineValid()
    {
      try
      {
        string url = "https://www.xasumao.cn/product/validreg.aspx";
        Encoding encoding = Encoding.GetEncoding("gb2312");
        IDictionary<string, string> parameters = (IDictionary<string, string>) new Dictionary<string, string>();
        parameters.Add("ProduceName", this.AppName);
        parameters.Add("ProduceVer", this.AppVer);
        parameters.Add("HDKey", this.AppHDKey);
        HttpWebResponse postHttpResponse = HttpWebResponseUtility.CreatePostHttpResponse(url, parameters, new int?(), (string) null, encoding, (CookieCollection) null);
        WDLog.WriteTextLog("info", "onlineValid(response) and response==null is " + (object) (postHttpResponse == null));
        if (postHttpResponse != null)
        {
          using (StreamReader streamReader = new StreamReader(postHttpResponse.GetResponseStream()))
          {
            RegState.AlreadyReg = RegState.SoftwareState.NotValid;
            string str1 = streamReader.ReadLine();
            string str2 = streamReader.ReadLine();
            string SignData = streamReader.ReadLine();
            string s = streamReader.ReadLine();
            if (new RSACryption().SignatureDeformatterWithMD5(string.Format("producename={0}&HDKey={1}&DeadLineDate={2}", (object) this.AppName, (object) this.AppHDKey, (object) s), SignData))
            {
              if (!(this.AppHDKey == str2) || !(str1 == "Valid"))
                return false;
              RegState.expiredTime = DateTime.Parse(s);
              RegState.AlreadyReg = RegState.SoftwareState.Valid;
              return true;
            }
          }
        }
        return false;
      }
      catch (Exception ex)
      {
        WDLog.WriteTextLog("info", "onlineValid( exception )" + ex.Message);
        return false;
      }
    }

    public void OpenValidPage()
    {
      Process.Start("https://www.xasumao.cn/product/valid.aspx?" + string.Format("producename={0}&Ver={1}&HDKey={2}", (object) this.AppName, (object) this.AppVer, (object) this.AppHDKey));
    }
  }
}
