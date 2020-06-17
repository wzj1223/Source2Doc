using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace XASuMaoUtils
{
  internal class HDKey
  {
    private static string GetCpuID()
    {
      try
      {
        string str = "";
        foreach (ManagementBaseObject instance in new ManagementClass("Win32_Processor").GetInstances())
          str = instance.Properties["ProcessorId"].Value.ToString();
        return str;
      }
      catch
      {
        return "unknow";
      }
    }

    private static string GetDiskID()
    {
      try
      {
        ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher()
        {
          Query = ((ObjectQuery) new SelectQuery("Win32_DiskDrive", "", new string[2]
          {
            "PNPDeviceID",
            "Signature"
          }))
        }.Get().GetEnumerator();
        enumerator.MoveNext();
        return enumerator.Current.Properties["signature"].Value.ToString().Trim();
      }
      catch
      {
        return "unknow";
      }
    }

    public static string GetHDKey()
    {
      return HDKey.UserMd5(HDKey.GetCpuID() + HDKey.GetDiskID() + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName);
    }

    private static string UserMd5(string str)
    {
      string s = str;
      string str1 = "";
      foreach (byte num in MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(s)))
        str1 += num.ToString("X");
      return str1;
    }
  }
}
