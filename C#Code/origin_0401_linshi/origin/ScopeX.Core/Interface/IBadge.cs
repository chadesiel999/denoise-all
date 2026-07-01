using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public interface IBadge
    {
        ChannelType Type
        {
            get;
        }

        ChannelId Id
        {
            get;
        }

        String Name
        {
            get;
        }

        Color DrawColor
        {
            get;
            set;
        }

        Boolean Active
        {
            get;
            set;
        }
    }
}
