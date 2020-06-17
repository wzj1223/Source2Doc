using System;
using System.IO;
using System.Text;

namespace Source2Doc
{
  internal class WDLog
  {
    public static bool IsDebug;

    public static void WriteTextLog(string action, string strMessage)
    {
      if (!WDLog.IsDebug)
        return;
      string path1 = AppDomain.CurrentDomain.BaseDirectory + "Log\\";
      if (!Directory.Exists(path1))
        Directory.CreateDirectory(path1);
      DateTime now = DateTime.Now;
      string path2 = path1 + now.ToString("yyyy-MM-dd") + ".System.txt";
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Time:    " + now.ToString() + "\r\n");
      stringBuilder.Append("Action:  " + action + "\r\n");
      stringBuilder.Append("Message: " + strMessage + "\r\n");
      stringBuilder.Append("-----------------------------------------------------------\r\n\r\n");
      StreamWriter streamWriter = File.Exists(path2) ? File.AppendText(path2) : File.CreateText(path2);
      streamWriter.WriteLine(stringBuilder.ToString());
      streamWriter.Close();
    }
  }
}
