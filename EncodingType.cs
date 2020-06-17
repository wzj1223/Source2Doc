using System.IO;
using System.Text;

namespace Source2Doc
{
  public class EncodingType
  {
    public static Encoding GetType(string FILE_NAME)
    {
      FileStream fileStream = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
      Encoding encoding = Encoding.Default;
      BinaryReader binaryReader = new BinaryReader((Stream) fileStream, Encoding.Default);
      int result;
      int.TryParse(fileStream.Length.ToString(), out result);
      byte[] data = binaryReader.ReadBytes(result);
      if (EncodingType.IsUTF8Bytes(data, FILE_NAME) || data.Length > 3 && data[0] == (byte) 239 && (data[1] == (byte) 187 && data[2] == (byte) 191))
        encoding = Encoding.UTF8;
      else if (data.Length > 3 && data[0] == (byte) 254 && (data[1] == byte.MaxValue && data[2] == (byte) 0))
        encoding = Encoding.BigEndianUnicode;
      else if (data.Length > 3 && data[0] == byte.MaxValue && (data[1] == (byte) 254 && data[2] == (byte) 65))
        encoding = Encoding.Unicode;
      binaryReader.Close();
      return encoding;
    }

    private static bool IsUTF8Bytes(byte[] data, string FILE_NAME)
    {
      int num1 = 1;
      for (int index = 0; index < data.Length; ++index)
      {
        byte num2 = data[index];
        if (num1 == 1)
        {
          if (num2 >= (byte) 128)
          {
            while (((int) (num2 <<= 1) & 128) != 0)
              ++num1;
            if (num1 == 1 || num1 > 6)
              return false;
          }
        }
        else
        {
          if (((int) num2 & 192) != 128)
            return false;
          --num1;
        }
      }
      return num1 <= 1;
    }
  }
}
