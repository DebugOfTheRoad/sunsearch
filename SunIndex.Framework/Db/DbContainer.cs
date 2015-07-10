using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunIndex.Core.DBContext;
using System.Configuration;

namespace SunIndex.Framework.Db
{
   public class DbContainer
    {
        public static Database GetDatabase(string conName)
        {
            return new Database(ConfigurationManager.ConnectionStrings[conName].ToString(), ConfigurationManager.ConnectionStrings[conName].ProviderName);
        }
    }
}
