using System;
using System.Collections.Generic;
using System.Text;

namespace WeiboLike.Models
{
    #region 用户资料数据构造
    public class UserInfoModel
    {
        /// <summary>
        /// 主页
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string Birthday { get; set; }


        /// <summary>
        /// 简介
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
    }
    #endregion
}
