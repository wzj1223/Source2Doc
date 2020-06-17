
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Source2Doc
{
  public class HttpWebResponseUtility
  {
    private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

    public static HttpWebResponse CreatePostHttpResponse(
      string url,
      IDictionary<string, string> parameters,
      int? timeout,
      string userAgent,
      Encoding requestEncoding,
      CookieCollection cookies)
    {
      if (string.IsNullOrEmpty(url))
      {
        WDLog.WriteTextLog(nameof (CreatePostHttpResponse), "url 为空");
        throw new ArgumentNullException(nameof (url));
      }
      if (requestEncoding == null)
      {
        WDLog.WriteTextLog(nameof (CreatePostHttpResponse), "requestEncoding 为空,无编码格式");
        throw new ArgumentNullException(nameof (requestEncoding));
      }
      HttpWebRequest httpWebRequest;
      if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
      {
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpWebResponseUtility.CheckValidationResult);
        httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
        httpWebRequest.ProtocolVersion = HttpVersion.Version10;
      }
      else
        httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
      httpWebRequest.Method = "POST";
      httpWebRequest.ContentType = "application/x-www-form-urlencoded";
      httpWebRequest.UserAgent = string.IsNullOrEmpty(userAgent) ? HttpWebResponseUtility.DefaultUserAgent : userAgent;
      if (timeout.HasValue)
        httpWebRequest.Timeout = timeout.Value;
      if (cookies != null)
      {
        httpWebRequest.CookieContainer = new CookieContainer();
        httpWebRequest.CookieContainer.Add(cookies);
      }
      StringBuilder stringBuilder = new StringBuilder();
      if (parameters != null && parameters.Count != 0)
      {
        int num = 0;
        foreach (string key in (IEnumerable<string>) parameters.Keys)
        {
          if (num > 0)
            stringBuilder.AppendFormat("&{0}={1}", (object) key, (object) parameters[key]);
          else
            stringBuilder.AppendFormat("{0}={1}", (object) key, (object) parameters[key]);
          ++num;
        }
      }
      WDLog.WriteTextLog("info", "CreatePostHttpResponse( before try )");
      try
      {
        byte[] bytes = requestEncoding.GetBytes(stringBuilder.ToString());
        WDLog.WriteTextLog("info", "CreatePostHttpResponse( requestEncoding.GetBytes)");
        using (Stream requestStream = httpWebRequest.GetRequestStream())
          requestStream.Write(bytes, 0, bytes.Length);
        return httpWebRequest.GetResponse() as HttpWebResponse;
      }
      catch (Exception ex)
      {
        WDLog.WriteTextLog("CreatePostHttpResponse:", ex.Message);
        return (HttpWebResponse) null;
      }
    }

    private static bool CheckValidationResult(
      object sender,
      X509Certificate certificate,
      X509Chain chain,
      SslPolicyErrors errors)
    {
      return !(DateTime.Now > Convert.ToDateTime(certificate.GetExpirationDateString())) && certificate.Subject.IndexOf("www.xasumao.cn") >= 0;
    }
  }
}
