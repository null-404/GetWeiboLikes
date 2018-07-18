using System;
using System.Collections.Generic;
using System.Text;

namespace WeiboLike.Models
{
    #region 点赞数据构造
    public class LikeModel
    {
        /// <summary>
        /// 用户主页链接
        /// </summary>
        public List<string> UserLinks { get; set; } = new List<string>();

        /// <summary>
        /// 点赞数据总页数
        /// </summary>
        public int TotalPage { get; set; }
    }

    #endregion
}
