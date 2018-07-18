using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using WeiboLike.Models;

namespace WeiboLike.Core
{
    #region 获取微博用户资料
    public class GetWeiboUser
    {

        private List<string> _urllist { get; set; }
        private List<UserInfoModel> _userInfolist = new List<UserInfoModel>();
        private int _threadNumber { get; set; }
        private int _threadeds { get; set; }
        #region 完成委托事件
        public delegate void Handle(int state, List<UserInfoModel> userInfoModel);
        public event Handle GetCompleted;

        private void GetCompleted_(int state)
        {
            GetCompleted?.Invoke(state, _userInfolist);

        }
        #endregion

        public GetWeiboUser(List<string> _urllist, int _threadnumber = 2)
        {
            this._urllist = _urllist;
            this._threadNumber = _threadnumber > _urllist.Count ? _urllist.Count : _threadnumber;
        }

        public void Get()
        {
            if (_urllist.Count == 1)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(Thread_Get));
                thread.Start(0);
            }
            else
            {
                for (int i = 0; i < _threadNumber; i++)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(Thread_Get));
                    thread.Start(i);
                }
            }
        }

        private void Thread_Get(object obj)
        {
            int.TryParse(obj.ToString(), out int index);
            int tn = _urllist.Count / _threadNumber;//获得每线程任务数
            tn = tn == 0 ? 1 : tn;
            int ys = _urllist.Count % _threadNumber;//余数
            int start = index * tn;
            int end = start + tn;
            if (ys > 0 && index == (_threadNumber - 1))
            {
                end = _urllist.Count;
            }

           

            for (int i = start; i < end; i++)
            {
                try
                {
                    _userInfolist.Add(GetUserInfo(_urllist[i]));
                }
                catch
                {

                }
            }
            _threadeds++;
            if (_threadeds >= _threadNumber || _urllist.Count == 1)
            {
                GetCompleted_(0);
            }
        }

        #region 通过主页链接获取用户的资料
        static UserInfoModel GetUserInfo(string url)
        {
            Console.WriteLine("开始获取：" + url);

            var result = new UserInfoModel();
            result.Url = url;
            string userinfohtml = XHttp.GetHttpResult(url);

            //取出昵称
            Regex regnickname = new Regex(@"\$CONFIG\['onick'\]='(?<nickname>.*)';", RegexOptions.IgnoreCase);
            result.NickName = regnickname.Match(userinfohtml).Groups["nickname"].Value;


            //正则取出所有script块
            Regex reg = new Regex(@"<script>(?<data>.*)</script>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串  
            MatchCollection matches = reg.Matches(userinfohtml);


            // 取得匹配项列表  
            foreach (Match match in matches)
            {

                try
                {
                    string data = (match.Groups["data"].Value);
                    data = data.Replace("FM.view(", "");

                    data = data.Substring(0, data.Length - 1);
                    WeiboInfoJsonModel w = JsonConvert.DeserializeObject<WeiboInfoJsonModel>(data);
                    if (w.ns == "pl.header.head.index" && w.domid == "Pl_Official_Headerv6__1")
                    {
                        //取出性别
                        w.html = w.html.Replace("\t", "").Replace("\r", "").Replace("\n", "");

                      
                        Regex regsex = new Regex(@"<a><i class=\""(?<sex>.+?)\""></i></a>", RegexOptions.IgnoreCase);
                        string sexstyle = regsex.Match(w.html).Groups["sex"].Value;
                        result.Sex = sexstyle.IndexOf("female") != -1 ? "女" : "男";
                    }
                    if (w.ns == "pl.content.homeFeed.index" && w.domid == "Pl_Core_UserInfo__6")
                    {


                        //正则取出所有资料
                        Regex reg2 = new Regex(@"<li class=\""item S_line2 clearfix\"">(?<data>.+?)</li>", RegexOptions.IgnoreCase);
                        w.html = w.html.Replace("\t", "").Replace("\r", "").Replace("\n", "");
                    
                        // 搜索匹配的字符串  
                        MatchCollection matches2 = reg2.Matches(w.html);


                        // 取得匹配项列表  
                        foreach (Match match2 in matches2)
                        {
                            string data2 = (match2.Groups["data"].Value);

                            Regex regcontent = new Regex(@"<span class=""item_text W_fl"">(?<content>.+?)</span>", RegexOptions.IgnoreCase);
                            string content = regcontent.Match(data2).Groups["content"].Value;
                            content = content.Replace(" ", "");
                            if (data2.IndexOf("ficon_starmark") != -1)
                            {
                                //等级
                            }
                            else if (data2.IndexOf("ficon_cd_place") != -1)
                            {
                                //地区
                                result.Location = content;
                            }
                            else if (data2.IndexOf("ficon_constellation") != -1)
                            {
                                //生日
                                result.Birthday = content;

                            }
                            else if (data2.IndexOf("ficon_pinfo") != -1)
                            {
                                //简介
                                result.Note = content;

                            }
                            
                        }

                        break;
                    }
                }
                catch
                {

                }
            }
            Console.WriteLine($"--> 昵称：{result.NickName}，性别：{result.Sex}，生日：{result.Birthday}，地区：{result.Location}，主页：{result.Url}");
            return result;
        }


        #endregion
    }
    #endregion
}
