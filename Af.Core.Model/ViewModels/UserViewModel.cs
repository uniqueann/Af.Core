using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Model.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsEnable { get; set; }
        public DateTime Expiration { get; set; }
    }
}
