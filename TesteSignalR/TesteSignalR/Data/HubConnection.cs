using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TesteSignalR.Areas.Identity.Data;

namespace TesteSignalR.Data
{
    public class HubConnection
    {
        [Required]
        public string UserAId { get; set; }
        public virtual User UserA { get; set; }

        [Required]
        public string UserBId { get; set; }
        public virtual User UserB { get; set; }
    }
}
