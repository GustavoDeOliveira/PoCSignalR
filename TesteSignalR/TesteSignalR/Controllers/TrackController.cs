using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteSignalR.Areas.Identity.Data;
using TesteSignalR.Data;
using TesteSignalR.Models;

namespace TesteSignalR.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TrackController : ControllerBase
    {
        private readonly TesteSignalRContext _dbContext;
        private readonly UserManager<User> _userManager;

        public TrackController(TesteSignalRContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post(TrackRequestModel model)
        {
            var response = new Response();
            var hub = _dbContext.HubConnections;
            var userB = await _userManager.FindByNameAsync(model.Target);
            if (userB != null)
            {
                var userA = await _userManager.FindByNameAsync(User.Identity.Name);
                var hubConnection = new HubConnection()
                {
                    UserA = userA,
                    UserB = userB
                };
                hub.RemoveRange(hub.Where(c => c.UserA.UserName == User.Identity.Name));
                _dbContext.Add(hubConnection);
                await _dbContext.SaveChangesAsync();
                return Ok(response);
            }
            response.Errors = new string[] { "Não foi possível encontrar o usuário alvo" };
            return BadRequest(response);
        }

        private static bool Related(string a, string b, string u, string t)
        {
            return (a == u && b == t) || (a == t && b == u);
        }
    }
}
