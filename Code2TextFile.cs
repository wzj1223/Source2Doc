using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Source2Doc
{
  internal class Code2TextFile : IFile
  {
    private bool IsLimit = true;
    private StreamWriter sw;
    private string destFile;
    private int CharCount;

    public bool OpenNewWord()
    {
      this.CharCount = 15000;
      this.IsLimit = RegState.AlreadyReg != RegState.SoftwareState.Valid;
      this.destFile = Environment.CurrentDirectory + "\\cn.xasumao.txt";
      this.sw = new StreamWriter(this.destFile, false, Encoding.UTF8);
      return true;
    }

    public bool FinishAndCloseWord(string pdfallName, string pdf60Name, string Title)
    {
      this.sw.Flush();
      this.sw.Close();
      string str = pdfallName + ".txt";
      File.Copy(this.destFile, str, true);
      try
      {
        Process.Start(str);
      }
      catch
      {
      }
      return true;
    }

    public bool AddText2Doc(string txt)
    {
      if (this.IsLimit)
      {
        if (this.CharCount <= 0)
          return false;
        this.CharCount -= txt.Length;
      }
      this.sw.WriteLine(txt);
      return true;
    }
  }
}
