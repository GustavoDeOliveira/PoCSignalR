using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TesteSignalR.Models
{
    public class UserRegistrationModel
    {
        public string UserName { get; set; }

        [Required(ErrorMessage = "O e-mail é necessário")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é necessária")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "A senha e a confirmação não batem.")]
        public string ConfirmPassword { get; set; }
    }
}
