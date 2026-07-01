using System;
using System.Collections.Generic;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    internal interface ISearchTypeModel
    {
        ChannelId Source { get; set; }

        Int32 ResultCount{ get; set; }

        (Double[,] Result, Int32 ResultCount)? ResultPack { get; set; }

        ISearchTypeOptions GetOption();

        public List<String> GetKeyInfos();
    }
   

}
