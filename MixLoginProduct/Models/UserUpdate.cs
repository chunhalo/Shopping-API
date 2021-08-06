using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MixLoginProduct.Models
{
    public class UserUpdate
    {
        public string old_password { get; set; }
        public string password { get; set; }
    }
}
