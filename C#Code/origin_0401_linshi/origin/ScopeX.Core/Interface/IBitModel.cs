using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    interface IBitModel
    {
        public ChannelType Type
        {
            get;
        }

        //public ChannelId Id
        //{
        //    get;
        //}

        public String Name
        {
            get;
        }

        public Color DrawColor
        {
            get;
            set;
        }

        public Boolean Active
        {
            get;
            set;
           
        }

        public String Label
        {
            get;
            set;
        }

        public Int32 ScaleIndex
        {
            get;
            set;
        }

    }
}
