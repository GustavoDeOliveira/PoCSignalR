using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TesteSignalR.Areas.Identity.Data;
using TesteSignalR.Models;
using TesteSignalR.Services;

namespace TesteSignalR.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AccountController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(UserRegistrationModel userModel)
        {
            var response = new Response("Ocorreu um ou mais erros ao cadastrar o usuário.");
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userModel.UserName)) userModel.UserName = userModel.Email;

                var user = _mapper.Map<User>(userModel);

                var result = await _userManager.CreateAsync(user, userModel.Password);
                if (result.Succeeded)
                {
                    response.Message = $"Usuário {userModel.UserName} cadastrado com sucesso!";
                    return Ok(response);
                }
                else
                {
                    response.Errors = result.Errors.Select(e => e.Description);
                }
            }
            else
            {
                response.Errors = ExtractModelStateErrors();
            }
            return BadRequest(response);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginModel userModel)
        {
            var response = new LoginResponse();
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(userModel.UserName);
                if (user != null && await _userManager.CheckPasswordAsync(user, userModel.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    var allClaims = authClaims.Concat(userRoles.Select(c => new Claim(ClaimTypes.Role, c)));
                    var expireDate = DateTime.Now.AddHours(3);
                    response.Token = WriteJwtToken(allClaims, expireDate);
                    response.Expires = expireDate.ToString("yyyy-MM-dd hh:mm:ss");
                    return Ok(response);
                }
                else
                {
                    response.Errors = new string[] { "Not allowed" };
                }
            }
            else
            {
                response.Errors = ExtractModelStateErrors();
            }
            return BadRequest(response);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(new Response());
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Status()
        {
            var user = User.Identity;
            if (user == null || !user.IsAuthenticated)
                return Ok("Not logged in");
            return Ok("Logged in as " + user.Name);
        }

        private IEnumerable<string> ExtractModelStateErrors()
        {
            return ModelState?.Values
                    .SelectMany(v => v.Errors
                        .Select(e => e.ErrorMessage))
                    ?? Enumerable.Empty<string>();
        }

        private string WriteJwtToken(IEnumerable<Claim> authClaims, DateTime expires)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: expires,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
