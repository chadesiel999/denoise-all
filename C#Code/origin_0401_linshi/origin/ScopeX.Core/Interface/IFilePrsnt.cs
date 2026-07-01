using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public interface IFilePrsnt
    {
        public ChannelId WfmSource
        {
            get;
            set;
        }

        public WfmFormat WfmFormat
        {
            get;
            set;
        }

        public PicFormat PicFormat
        {
            get;
            set;
        }

        public PicArea PicRegion
        {
            get;
            set;
        }

        //public String FileName
        //{
        //    get;
        //    set;
        //}

        //public String WfmPath
        //{
        //    get;
        //    set;
        //}

        //public String PicPath
        //{
        //    get;
        //    set;
        //}

        //public String SettingLoadPath
        //{
        //    get;
        //    set;
        //}

        //public String SettingSavePath
        //{
        //    get;
        //    set;
        //}
    }
}
