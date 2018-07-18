using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace WeiboLike.Core
{
    #region HTTP帮助类
    public static class XHttp
    {
        //微博Cookie
        static string cookie = "";

        #region gethttp
        public static String GetHttpResult(string url)
        {
            var handler = new HttpClientHandler() { UseCookies = true };
            HttpClient httpClient = new HttpClient(handler);
            //请求超时59s，不知道另外1s去哪了。
            httpClient.Timeout = new TimeSpan(0, 0, 0, 59, 0);
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");        
            httpClient.DefaultRequestHeaders.Add("cookie", cookie);
            String result = "";
            HttpResponseMessage response = httpClient.GetAsync(new Uri(url)).Result;
            result = response.Content.ReadAsStringAsync().Result;
            httpClient.Dispose();

            return result;
        }
        #endregion
    }
    #endregion
}
