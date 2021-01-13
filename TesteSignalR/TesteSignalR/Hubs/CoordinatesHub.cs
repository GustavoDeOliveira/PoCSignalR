using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TesteSignalR.Areas.Identity.Data;
using TesteSignalR.Data;
using TesteSignalR.Hubs.Clients;
using TesteSignalR.Models;

namespace TesteSignalR.Hubs
{
    public class CoordinatesHub : Hub<ICoordinatesClient>
    {
        private readonly UserManager<User> _userManager;
        private readonly TesteSignalRContext _dbContext;

        public CoordinatesHub(UserManager<User> userManager, TesteSignalRContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        // public virtual Task OnConnectedAsync()

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connections = _dbContext.HubConnections
                    .Where(c => c.UserA.UserName == Context.UserIdentifier || c.UserB.UserName == Context.UserIdentifier);
            var listeners = connections.Where(c => c.UserB.UserName == Context.UserIdentifier).Select(c => c.UserA.UserName).ToArray();
            _dbContext.HubConnections
                .RemoveRange(connections);
            _dbContext.SaveChanges();

            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            string json = JsonSerializer.Serialize(new Coordinates(), options);

            return Clients.Users(listeners).ReceiveCoordinates(json);
        }

        public Task Update(Coordinates coordinates)
        {
            if (coordinates != null)
            {
                if (Context.UserIdentifier != null)
                {
                    try
                    {
                        string[] listeners = Array.Empty<string>();
                        if (Context.Items.ContainsKey(Context.UserIdentifier) && !IsCacheExpired((HubClientCacheItem)Context.Items[Context.UserIdentifier], 10))
                        {
                            listeners = ((HubClientCacheItem)Context.Items[Context.UserIdentifier]).Listeners;
                        }
                        else
                        {
                            listeners = _dbContext.HubConnections
                              .Where(u => u.UserB.UserName == Context.UserIdentifier)
                              .Select(u => u.UserA.UserName).ToArray();
                              //.Concat(_dbContext.HubConnections
                              //    .Where(u => u.UserA.UserName == Context.UserIdentifier)
                              //    .Select(u => u.UserB.UserName))
                            Context.Items[Context.UserIdentifier] = new HubClientCacheItem
                            {
                                CreatedOn = DateTime.Now,
                                Listeners = listeners
                            };
                        }

                        coordinates.UserId = Context.User.Identity.Name;

                        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                        string json = JsonSerializer.Serialize(coordinates, options);

                        return Clients.Users(listeners).ReceiveCoordinates(json);
                    }
                    catch (Exception ex)
                    {
                        return Clients.Caller.ReceiveMessage(ex.Message);
                        throw;
                    }
                }
                else return Clients.Caller.ReceiveMessage("Usuário não está autenticado na API");
            }
            else return Clients.Caller.ReceiveMessage("Informação inválida.");
        }

        private static bool IsCacheExpired(HubClientCacheItem hubClientCacheItem, int secondsToExpire)
        {
            return hubClientCacheItem.CreatedOn.AddSeconds(secondsToExpire) < DateTime.Now;
        }
    }
}