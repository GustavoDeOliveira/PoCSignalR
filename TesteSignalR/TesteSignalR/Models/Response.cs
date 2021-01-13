using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TesteSignalR.Models
{
    public class Response
    {
        public string Message { get; set; } = "Operação realizada com sucesso.";
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    }
}
