using System;
using System.Collections.Generic;
using System.Text;

namespace Af.Core.Model.ViewModels
{
    public class TokenInfoViewModel
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public double ExpiresIn { get; set; }
        public string TokenType { get; set; }
    }
}
