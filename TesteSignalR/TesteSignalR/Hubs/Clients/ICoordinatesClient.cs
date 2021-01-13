using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteSignalR.Models;

namespace TesteSignalR.Hubs.Clients
{
    public interface ICoordinatesClient
    {
        Task ReceiveMessage(string value);
        Task ReceiveCoordinates(string json);
    }
}
