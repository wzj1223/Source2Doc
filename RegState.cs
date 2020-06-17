
using System;

namespace Source2Doc
{
  internal class RegState
  {
    public static RegState.SoftwareState AlreadyReg = RegState.SoftwareState.NoConnect;
    public static DateTime expiredTime;

    public enum SoftwareState
    {
      NoConnect,
      Valid,
      NotValid,
    }
  }
}
