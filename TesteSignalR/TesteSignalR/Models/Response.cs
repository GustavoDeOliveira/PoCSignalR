using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TesteSignalR.Models
{
    public class Response
    {
        public Response(string message = "Operação realizada com sucesso.")
        {
            Message = message;
        }
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    }
}
