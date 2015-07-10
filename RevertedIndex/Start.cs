using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunIndex.Core.Ioc;
using SunIndex.Framework.Index;
using Newtonsoft.Json;
using System.Configuration;
using SunIndex.Core.Index;
using SunIndex.Core.Document;
using SunIndex.Framework.MongoDb;
using SunIndex.Core.Query;
using SunIndex.Framework.Query;
using Autofac;

namespace RevertedIndex
{
    public class Start
    {
        static PanGu.Match.MatchOptions _Options;
        static PanGu.Match.MatchParameter _Parameters;
        public static void Init()
        {
            _Options = new PanGu.Match.MatchOptions();
            _Parameters = new PanGu.Match.MatchParameter();

            _Options.FrequencyFirst = Convert.ToBoolean(ConfigurationManager.AppSettings["checkBoxFreqFirst"]);
            _Options.FilterStopWords = Convert.ToBoolean(ConfigurationManager.AppSettings["FilterStopWords"]);
            _Options.ChineseNameIdentify = Convert.ToBoolean(ConfigurationManager.AppSettings["ChineseNameIdentify"]);
            _Options.MultiDimensionality = Convert.ToBoolean(ConfigurationManager.AppSettings["MultiDimensionality"]);
            _Options.EnglishMultiDimensionality = Convert.ToBoolean(ConfigurationManager.AppSettings["EnglishMultiDimensionality"]);
            _Options.ForceSingleWord = Convert.ToBoolean(ConfigurationManager.AppSettings["ForceSingleWord"]);
            _Options.TraditionalChineseEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["TraditionalChineseEnabled"]);
            _Options.OutputSimplifiedTraditional = Convert.ToBoolean(ConfigurationManager.AppSettings["OutputSimplifiedTraditional"]);
            _Options.UnknownWordIdentify = Convert.ToBoolean(ConfigurationManager.AppSettings["UnknownWordIdentify"]);
            _Options.FilterEnglish = Convert.ToBoolean(ConfigurationManager.AppSettings["FilterEnglish"]);
            _Options.FilterNumeric = Convert.ToBoolean(ConfigurationManager.AppSettings["FilterNumeric"]);
            _Options.IgnoreCapital = Convert.ToBoolean(ConfigurationManager.AppSettings["IgnoreCapital"]);
            _Options.EnglishSegment = Convert.ToBoolean(ConfigurationManager.AppSettings["EnglishSegment"]);
            _Options.SynonymOutput = Convert.ToBoolean(ConfigurationManager.AppSettings["SynonymOutput"]);
            _Options.WildcardOutput = Convert.ToBoolean(ConfigurationManager.AppSettings["WildcardOutput"]);
            _Options.WildcardSegment = Convert.ToBoolean(ConfigurationManager.AppSettings["WildcardSegment"]);
            _Options.CustomRule = Convert.ToBoolean(ConfigurationManager.AppSettings["CustomRule"]);

            _Parameters.Redundancy = Convert.ToInt16(ConfigurationManager.AppSettings["Redundancy"]);
            _Parameters.FilterEnglishLength = Convert.ToInt16(ConfigurationManager.AppSettings["FilterEnglishLength"]);
            _Parameters.FilterNumericLength = Convert.ToInt16(ConfigurationManager.AppSettings["FilterNumericLength"]);


            var config = new Config();
            config.ServerHosts = JsonConvert.DeserializeObject<List<ServerHost>>(ConfigurationManager.ConnectionStrings["mongodb"].ToString());
            config.BaseDbName = "serch_base";
            config.IndexListDbName = "serch_index";
            CoreIoc.Register(o => o.RegisterInstance(config).As<Config>().ExternallyOwned());
            CoreIoc.Register(o=>o.RegisterType<ServerManage>().As<ServerManage>().SingleInstance());
            
            CoreIoc.Register(o=>o.RegisterType<DocumentOp>().As<IDocument>().SingleInstance());
            CoreIoc.Register(o => o.RegisterInstance(new PanguTokenizer(_Options,_Parameters)).As<ITokenizer>().ExternallyOwned());
            CoreIoc.Register(o => o.RegisterType<Index>().As<IIndex>().SingleInstance());
            CoreIoc.Register(o => o.RegisterType<Query>().As<Iquery>().SingleInstance());
            CoreIoc.Build();
        }
    }
}
