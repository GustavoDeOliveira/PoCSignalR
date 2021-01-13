using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TesteSignalR.Hubs
{
    public class HubClientCacheItem
    {
        public string[] Listeners { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
