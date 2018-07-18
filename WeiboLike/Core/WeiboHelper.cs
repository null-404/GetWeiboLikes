using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WeiboLike.Models;

namespace WeiboLike.Core
{
    public class WeiboHelper
    {
        #region 获取每页点赞的用户主页链接
        public static LikeModel GetPageLikeLinks(string mid, int page = 1)
        {
            string likehtml = XHttp.GetHttpResult($"https://weibo.com/aj/v6/like/big?ajwvr=6&mid={mid}&page={page}&__rnd=1531705846965");
            likehtml = likehtml.Replace("\\", "");
            var result = new LikeModel();

            //提取总页数
            Regex regtotalpage = new Regex("\"totalpage\":(?<totalpage>.*),\"pagenum\"");
            var regtotalpagemath = regtotalpage.Match(likehtml);
            int.TryParse(regtotalpagemath.Groups["totalpage"].Value, out int totalpage);
            result.TotalPage = totalpage;
            Regex reg = new Regex(@"<a\b[^<>]*?\bhref[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<url>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(likehtml);

            foreach (Match match in matches)
            {
                string data = (match.Groups["url"].Value);
                if (!string.IsNullOrEmpty(data) && result.UserLinks.IndexOf(data) == -1 && data.IndexOf("http") != -1)
                {
                    result.UserLinks.Add(data);
                }
            }
            return result;
        }

        #endregion

        #region 获取数据mid
        public static string GetMid(string uid, string weiboid)
        {
            string mid = "";
            string html = XHttp.GetHttpResult($"https://weibo.com/{uid}/{weiboid}");

            //取出所有script块
            Regex reg = new Regex(@"<script>(?<data>.*)</script>", RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(html);
            foreach (Match match in matches)
            {

                try
                {
                    string data = (match.Groups["data"].Value);
                    data = data.Replace("FM.view(", "");

                    data = data.Substring(0, data.Length - 1);
                    WeiboInfoJsonModel w = JsonConvert.DeserializeObject<WeiboInfoJsonModel>(data);
                    if (w.ns == "pl.content.weiboDetail.index" && w.domid == "Pl_Official_WeiboDetail__73")
                    {
                        w.html = w.html.Replace("\t", "").Replace("\r", "").Replace("\n", "");
                        Regex regmid = new Regex(@"mid=\""(?<mid>.+?)\""", RegexOptions.IgnoreCase);
                        mid = regmid.Match(w.html).Groups["mid"].Value;
                        break;
                    }
                }
                catch
                {

                }
            }

            return mid;
        }
        #endregion

        /// <summary>
        /// 通过微博链接URL获取用户UID和微博ID
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string[] GetUidWeiboID(string url)
        {
            var result = new string[2];
            url = url.Replace("https://weibo.com/", "");
            result[0] = url.Split('/')[0];
            result[1] = url.Split('/')[1];
            if (result[1].IndexOf("?") != -1)
            {
                result[1] = result[1].Split('?')[0];
            }
            return result;
        }
    }
}
