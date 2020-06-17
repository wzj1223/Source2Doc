namespace Source2Doc
{
  internal interface IFile
  {
    bool OpenNewWord();

    bool FinishAndCloseWord(string pdfallName, string pdf60Name, string Title);

    bool AddText2Doc(string txt);
  }
}
