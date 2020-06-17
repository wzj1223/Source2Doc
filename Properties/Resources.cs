// Decompiled with JetBrains decompiler
// Type: Source2Doc.Properties.Resources
// Assembly: Source2Doc, Version=2.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CD5DCFCB-6220-4FBE-9718-8BB8D41E4151
// Assembly location: C:\Users\吴志杰\Desktop\Source2Doc - 副本.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Source2Doc.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) Source2Doc.Properties.Resources.resourceMan, (object) null))
          Source2Doc.Properties.Resources.resourceMan = new ResourceManager("Source2Doc.Properties.Resources", typeof (Source2Doc.Properties.Resources).Assembly);
        return Source2Doc.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return Source2Doc.Properties.Resources.resourceCulture;
      }
      set
      {
        Source2Doc.Properties.Resources.resourceCulture = value;
      }
    }

    internal static byte[] DocX
    {
      get
      {
        return (byte[]) Source2Doc.Properties.Resources.ResourceManager.GetObject(nameof (DocX), Source2Doc.Properties.Resources.resourceCulture);
      }
    }
  }
}
