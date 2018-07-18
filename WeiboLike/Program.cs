using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using WeiboLike.Core;
using WeiboLike.Models;

namespace WeiboLike
{

    class Program
    {
        static string savename = "";
        static string mid = "";
        static LikeModel ulist = new LikeModel();
        static int totalpage { get; set; }
        static int pageindex = 1;

        static List<UserInfoModel> data = new List<UserInfoModel>();

        static Timer timer;
        static int time = 0;
        static void Main(string[] args)
        {
            Console.Title = "WeiboLike";
            timer = new Timer(new TimerCallback(TimerUp), null, Timeout.Infinite, 1000); 

            while (true)
            {
                //Console.WriteLine("请输入用户ID：");
                //string uid = Console.ReadLine();
                //if (string.IsNullOrEmpty(uid))
                //{
                //    continue;
                //}
                //Console.WriteLine("请输入微博ID：");
                //string weiboid = Console.ReadLine();
                //if (string.IsNullOrEmpty(weiboid))
                //{
                //    continue;
                //}
                Console.WriteLine("请粘贴微博URL链接：");
                string[] uidweiboid = WeiboHelper.GetUidWeiboID(Console.ReadLine());
                if(string.IsNullOrEmpty(uidweiboid[0]) || string.IsNullOrEmpty(uidweiboid[1]))
                {
                    Console.WriteLine("从链接获取uid/微博id失败，请重试。");
                    continue;
                }
               
                Console.WriteLine("正在获取Mid...");
                mid = WeiboHelper.GetMid(uidweiboid[0], uidweiboid[1]);
                if (string.IsNullOrEmpty(mid))
                {
                    Console.WriteLine("没有获取到Mid，请重试。");

                    continue;
                }
                Console.WriteLine("已获取到MID：" + mid);
                Console.WriteLine("请输入微博数据保存文件名：");
                savename = Console.ReadLine()+ ".json";
                Console.WriteLine("开始抓取本条微博的点赞用户数据...");
                timer.Change(0, 1000);
                //第一步获取点赞基本数据
                ulist = WeiboHelper.GetPageLikeLinks(mid, 1);
                totalpage = ulist.TotalPage;

                GetWeiboUser getWeiboUser = new GetWeiboUser(ulist.UserLinks, 60);
                getWeiboUser.GetCompleted += GetCompleted;
                getWeiboUser.Get();

                Console.ReadKey();

            }
        }

        private static void TimerUp(object state)
        {
            time++;
        }

        static void GetCompleted(int state, List<UserInfoModel> list)
        {
            //通知完成
            Console.WriteLine($"[{pageindex}/{totalpage}] GetCompleted：" + list.Count);
            //保存数据
            data.AddRange(list);
            File.WriteAllText(savename, JsonConvert.SerializeObject(data));
            //判断是否还有数据
            pageindex++;
            if (pageindex <= totalpage)
            {
                //还有数据
                ulist = WeiboHelper.GetPageLikeLinks(mid, pageindex);
                GetWeiboUser getWeiboUser = new GetWeiboUser(ulist.UserLinks, 30);
                getWeiboUser.GetCompleted += GetCompleted;

                getWeiboUser.Get();
            }
            else
            {
                timer.Change(Timeout.Infinite, 1000);
                //没有
                Console.WriteLine($"抓取已完成，耗时 {time} 秒。总计 {totalpage} 页数据，抓取到用户 {data.Count} 位。");
                File.WriteAllText(savename, JsonConvert.SerializeObject(data));
                Console.WriteLine($"数据保存在程序启动目录：{savename}");
                time = 0;
                pageindex = 1;
                data.Clear();
            }
        }







        #region 分析
        //static void FenXi()
        //{
        //    Console.WriteLine("输入文件名：");
        //    string data = File.ReadAllText(Console.ReadLine() + ".json");

        //    List<UserInfoModel> _userinfolist = JsonConvert.DeserializeObject<List<UserInfoModel>>(data);

        //    List<UserInfoModel> _fenxiresult = new List<UserInfoModel>();
        //    Console.WriteLine("分析已开始！");
        //    int fxs = 0;
        //    foreach (UserInfoModel v in _userinfolist)
        //    {
        //        fxs++;
        //        int rank = 0;
        //        if (v.Birthday != null && v.Birthday.IndexOf("1995年") != -1)
        //        {
        //            rank++;
        //        }
        //        if (v.Birthday != null && v.Birthday.IndexOf("7月30日") != -1)
        //        {
        //            rank++;
        //        }
        //        if (v.NickName != null && v.NickName.IndexOf("蘑菇") != -1)
        //        {
        //            rank++;
        //        }
        //        if (v.Note != null && v.Note.IndexOf("蘑菇") != -1)
        //        {
        //            rank++;
        //        }
        //        if (v.Location != null && v.Location.IndexOf("北京") != -1)
        //        {
        //            rank++;
        //        }

        //        if (rank >= 2)
        //        {
        //            _fenxiresult.Add(v);
        //        }
        //        Console.WriteLine("-> " + v.NickName);
        //    }
        //    Console.WriteLine("-------------------------------");
        //    Console.WriteLine($"分析完毕，一共{fxs}条数据，可能的结果：");
        //    int i = 1;
        //    foreach (UserInfoModel v in _fenxiresult)
        //    {
        //        Console.WriteLine($"[{i}] 昵称：{v.NickName}，生日：{v.Birthday}，地区：{v.Location}，URL：{v.Url}");
        //        i++;
        //    }

        //}
        #endregion

    }


}
