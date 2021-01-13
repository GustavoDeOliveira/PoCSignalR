using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteSignalR.Areas.Identity.Data;

namespace TesteSignalR.Models
{
    public class Coordinates
    {
        public string UserId { get; set; } = "";
        public string TargetId { get; set; } = "";
        public double Longitude { get; set; }
        public double Lattitude { get; set; }
    }
}
