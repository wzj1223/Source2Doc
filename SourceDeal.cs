using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Forms;

namespace Source2Doc
{
  internal class SourceDeal
  {
    private Code2Doc WP;

    private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
      string str = (args.Name.Contains(",") ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "")).Replace(".", "_");
      if (str.EndsWith("_resources"))
        return (Assembly) null;
      int num1 = (int) MessageBox.Show(str);
      ResourceManager resourceManager = new ResourceManager(this.GetType().Namespace + ".Properties.Resources", Assembly.GetExecutingAssembly());
      int num2 = (int) MessageBox.Show("get :" + resourceManager.ToString());
      byte[] rawAssembly = (byte[]) null;
      try
      {
        rawAssembly = (byte[]) resourceManager.GetObject(str);
      }
      catch (Exception ex)
      {
        int num3 = (int) MessageBox.Show("exception:" + ex.Message);
      }
      return Assembly.Load(rawAssembly);
    }

    public SourceDeal(string filename, string title)
    {
      this.WP = new Code2Doc();
      this.WP.OpenNewWord(filename, title);
      string str = Environment.CurrentDirectory + "\\Clean";
      if (Directory.Exists(str))
        this.DelectDirFile(str);
      Directory.CreateDirectory(str);
    }

    private void DelectDirFile(string srcPath)
    {
      try
      {
        DirectoryInfo directoryInfo = new DirectoryInfo(srcPath);
        foreach (FileSystemInfo fileSystemInfo in directoryInfo.GetFileSystemInfos())
        {
          if (fileSystemInfo is DirectoryInfo)
            new DirectoryInfo(fileSystemInfo.FullName).Delete(true);
          else
            File.Delete(fileSystemInfo.FullName);
        }
        directoryInfo.Delete(true);
      }
      catch
      {
      }
    }

    public bool AddFile2Word(string filename)
    {
      string DestFileName = "";
      if (this.Clean(filename, ref DestFileName))
        this.ReadCode2Word(DestFileName);
      return true;
    }

    public string Finish()
    {
      string openFilename = this.WP.OpenFilename;
      try
      {
        this.WP.FinishAndCloseWord();
        this.WP = (Code2Doc) null;
        this.DelectDirFile("Clean");
      }
      catch
      {
      }
      return openFilename;
    }

    private bool ReadCode2Word(string filename)
    {
      try
      {
        if (RegState.AlreadyReg != RegState.SoftwareState.Valid)
          this.WP.AddText2Doc("未注册只能生成部分文档，注册后可以生成完整生成软著申请文档。下载最新版请到www.xasumao.cn！");
        StreamReader streamReader = new StreamReader(filename, Encoding.UTF8);
        string txt;
        while ((txt = streamReader.ReadLine()) != null)
          this.WP.AddText2Doc(txt);
        streamReader.Close();
        return true;
      }
      catch
      {
        return false;
      }
    }

    private bool Clean(string filename, ref string DestFileName)
    {
      string extension = Path.GetExtension(filename);
      DestFileName = Environment.CurrentDirectory + "\\Clean\\1" + extension;
      Encoding type = EncodingType.GetType(filename);
      if (type != Encoding.UTF8)
        this.Convert2UTF8(filename, type, DestFileName);
      else
        File.Copy(filename, DestFileName, true);
      string lower = extension.ToLower();
      return lower.IndexOf("xml") < 0 & lower.IndexOf("xaml") < 0 ? this.CleanCode(filename, ref DestFileName) : this.CleanXML(filename, ref DestFileName);
    }

    private bool CleanXML(string filename, ref string DestFileName)
    {
      string str = Environment.CurrentDirectory + "\\Clean\\2" + Path.GetExtension(filename);
      this.CleanXMLComment(DestFileName, str);
      this.CleanBlankLine(str, DestFileName);
      return true;
    }

    private bool CleanCode(string filename, ref string DestFileName)
    {
      string str = Environment.CurrentDirectory + "\\Clean\\2" + Path.GetExtension(filename);
      this.CleanComment(DestFileName, str);
      this.CleanBlankLine(str, DestFileName);
      return true;
    }

    private bool Convert2UTF8(string Orifilename, Encoding type, string DestFilename)
    {
      StreamReader streamReader = new StreamReader(Orifilename, type);
      string end = streamReader.ReadToEnd();
      streamReader.Close();
      StreamWriter streamWriter = new StreamWriter(DestFilename, false, Encoding.UTF8);
      streamWriter.Write(end);
      streamWriter.Flush();
      streamWriter.Close();
      return true;
    }

    private bool CleanComment(string InFilename, string OutFilename)
    {
      StreamReader streamReader = new StreamReader(InFilename, Encoding.UTF8);
      string end = streamReader.ReadToEnd();
      streamReader.Close();
      string str1 = "";
      SourceDeal.State state = SourceDeal.State.CODE;
      for (int startIndex = 0; startIndex < end.Length; ++startIndex)
      {
        string str2 = end.Substring(startIndex, 1);
        switch (state)
        {
          case SourceDeal.State.CODE:
            if (str2 == "/")
            {
              state = SourceDeal.State.SLASH;
              break;
            }
            str1 += str2;
            if (str2 == "'")
            {
              state = SourceDeal.State.CODE_CHAR;
              break;
            }
            if (str2 == "\"")
            {
              state = SourceDeal.State.CODE_STRING;
              break;
            }
            break;
          case SourceDeal.State.SLASH:
            if (str2 == "*")
            {
              state = SourceDeal.State.NOTE_MULTILINE;
              break;
            }
            if (str2 == "/")
            {
              state = SourceDeal.State.NOTE_SINGLELINE;
              break;
            }
            str1 = str1 + "/" + str2;
            state = SourceDeal.State.CODE;
            break;
          case SourceDeal.State.NOTE_MULTILINE:
            if (str2 == "*")
            {
              state = SourceDeal.State.NOTE_MULTILINE_STAR;
              break;
            }
            if (str2 == "\n")
              str1 += "\r\n";
            state = SourceDeal.State.NOTE_MULTILINE;
            break;
          case SourceDeal.State.NOTE_MULTILINE_STAR:
            state = !(str2 == "/") ? (!(str2 == "*") ? SourceDeal.State.NOTE_MULTILINE : SourceDeal.State.NOTE_MULTILINE_STAR) : SourceDeal.State.CODE;
            break;
          case SourceDeal.State.NOTE_SINGLELINE:
            if (str2 == "\\")
            {
              state = SourceDeal.State.BACKSLASH;
              break;
            }
            if (str2 == "\n")
            {
              str1 += "\r\n";
              state = SourceDeal.State.CODE;
              break;
            }
            state = SourceDeal.State.NOTE_SINGLELINE;
            break;
          case SourceDeal.State.BACKSLASH:
            if (str2 == "\\" || str2 == "\r" || str2 == "\n")
            {
              if (str2 == "\n")
                str1 += "\r\n";
              state = SourceDeal.State.BACKSLASH;
              break;
            }
            state = SourceDeal.State.NOTE_SINGLELINE;
            break;
          case SourceDeal.State.CODE_CHAR:
            str1 += str2;
            state = !(str2 == "\\") ? (!(str2 == "'") ? SourceDeal.State.CODE_CHAR : SourceDeal.State.CODE) : SourceDeal.State.CHAR_ESCAPE_SEQUENCE;
            break;
          case SourceDeal.State.CHAR_ESCAPE_SEQUENCE:
            str1 += str2;
            state = SourceDeal.State.CODE_CHAR;
            break;
          case SourceDeal.State.CODE_STRING:
            str1 += str2;
            state = !(str2 == "\\") ? (!(str2 == "\"") ? SourceDeal.State.CODE_STRING : SourceDeal.State.CODE) : SourceDeal.State.STRING_ESCAPE_SEQUENCE;
            break;
          case SourceDeal.State.STRING_ESCAPE_SEQUENCE:
            str1 += str2;
            state = SourceDeal.State.CODE_STRING;
            break;
        }
      }
      StreamWriter streamWriter = new StreamWriter(OutFilename, false, Encoding.UTF8);
      streamWriter.Write(str1);
      streamWriter.Flush();
      streamWriter.Close();
      return true;
    }

    private bool CleanXMLComment(string InFilename, string OutFilename)
    {
      StreamReader streamReader = new StreamReader(InFilename, Encoding.UTF8);
      StreamWriter streamWriter = new StreamWriter(OutFilename, false, Encoding.UTF8);
      string str1 = "";
      SourceDeal.XMLState xmlState = SourceDeal.XMLState.CODE;
      string str2 = streamReader.ReadToEnd();
      while (str2.Length > 2)
      {
        switch (xmlState)
        {
          case SourceDeal.XMLState.CODE:
            int length = str2.IndexOf("<!--");
            if (length >= 0)
            {
              str1 += str2.Substring(0, length);
              str2 = str2.Substring(length + 4);
              xmlState = SourceDeal.XMLState.COMMENT;
              continue;
            }
            str1 += str2;
            str2 = string.Empty;
            continue;
          case SourceDeal.XMLState.COMMENT:
            int num = str2.IndexOf("-->");
            if (num >= 0)
            {
              xmlState = SourceDeal.XMLState.CODE;
              str2 = str2.Substring(num + 3);
              continue;
            }
            continue;
          default:
            continue;
        }
      }
      streamReader.Close();
      streamWriter.Write(str1);
      streamWriter.Flush();
      streamWriter.Close();
      return true;
    }

    private bool CleanBlankLine(string InFilename, string OutFilename)
    {
      StreamReader streamReader = new StreamReader(InFilename, Encoding.UTF8);
      StreamWriter streamWriter = new StreamWriter(OutFilename, false, Encoding.UTF8);
      while (!streamReader.EndOfStream)
      {
        string str = streamReader.ReadLine();
        if (string.Copy(str).Replace("\t", " ").Trim().Length > 0)
          streamWriter.WriteLine(str);
      }
      streamReader.Close();
      streamWriter.Flush();
      streamWriter.Close();
      return true;
    }

    private enum State
    {
      CODE,
      SLASH,
      NOTE_MULTILINE,
      NOTE_MULTILINE_STAR,
      NOTE_SINGLELINE,
      BACKSLASH,
      CODE_CHAR,
      CHAR_ESCAPE_SEQUENCE,
      CODE_STRING,
      STRING_ESCAPE_SEQUENCE,
    }

    private enum XMLState
    {
      CODE,
      COMMENT,
    }
  }
}
