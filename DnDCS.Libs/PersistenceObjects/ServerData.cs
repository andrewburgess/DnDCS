﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DnDCS.Libs.PersistenceObjects
{
    public class ServerData
    {
        public bool RealTimeFogUpdates { get; set; }
        public bool ShowLog { get; set; }

        public ServerData()
        {
        }
    }
}
