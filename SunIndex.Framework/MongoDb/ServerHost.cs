using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunIndex.Framework.MongoDb
{
    public class ServerHost
    {

        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName
        {
            get;
            set;
        }
        /// <summary>
        /// 组里服务器
        /// </summary>
        public List<string> Hosts
        {
            get;
            set;
        }
        /// <summary>
        /// 账号
        /// </summary>
        public string UserId
        {
            get;
            set;
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord
        {
            get;
            set;
        }
        /// <summary>
        /// 是否为基础数据库
        /// </summary>
        public bool IsBase
        {
            get;
            set;
        }
        /// <summary>
        /// 状态0:不可用,1:正常,2:不可写(满载)
        /// </summary>
        public int State
        {
            get;
            set;
        }
    }
}
