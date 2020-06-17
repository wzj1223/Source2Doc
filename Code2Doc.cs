using Novacode;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Source2Doc
{
  internal class Code2Doc
  {
    private bool IsLimit = true;
    private DocX document;
    private int CharCount;
    private Paragraph par;

    public string OpenFilename { get; private set; }

    public bool OpenNewWord(string filename, string Title)
    {
      bool flag = this.exportDocFile(filename);
      this.document = DocX.Load(this.OpenFilename);
      this.CharCount = 2000;
      this.IsLimit = RegState.AlreadyReg != RegState.SoftwareState.Valid;
      while (Title.Length < 60)
        Title += " ";
      ((Container) this.document.Headers.odd).Paragraphs[0].InsertText(0, Title, false, (Formatting) null);
      return flag;
    }

    public void FinishAndCloseWord()
    {
      this.document.Save();
      this.document.Dispose();
    }

    public bool AddText2Doc(string txt)
    {
      try
      {
        if (this.IsLimit)
        {
          if (this.CharCount <= 0)
            return false;
          this.CharCount -= txt.Length;
        }
        if (this.par == null)
          this.par = ((Container) this.document).Paragraphs[0];
        this.par.InsertText(txt + "\n", false, (Formatting) null);
        return true;
      }
      catch
      {
        return false;
      }
    }

    private bool exportDocFile(string filename)
    {
      Stream manifestResourceStream = Assembly.GetEntryAssembly().GetManifestResourceStream("Source2Doc.a.bin");
      byte[] buffer = new byte[manifestResourceStream.Length];
      manifestResourceStream.Read(buffer, 0, (int) manifestResourceStream.Length);
      manifestResourceStream.Close();
      try
      {
        this.OpenFilename = filename;
        FileStream fileStream = new FileStream(Environment.CurrentDirectory + "\\" + filename, FileMode.Create);
        BinaryWriter binaryWriter = new BinaryWriter((Stream) fileStream, Encoding.UTF8);
        binaryWriter.Write(buffer);
        binaryWriter.Flush();
        fileStream.Close();
        return true;
      }
      catch
      {
        this.OpenFilename = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".docx";
        FileStream fileStream = new FileStream(Environment.CurrentDirectory + "\\" + this.OpenFilename, FileMode.Create);
        BinaryWriter binaryWriter = new BinaryWriter((Stream) fileStream, Encoding.UTF8);
        binaryWriter.Write(buffer);
        binaryWriter.Flush();
        fileStream.Close();
        return false;
      }
    }
  }
}
