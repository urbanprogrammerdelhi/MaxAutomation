using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxAutomation.Model
{
    public class Account
    {
        public string UserName { get; set; }
        public string Company { get; set; }
        public int Role { get; set; }
    }
    public class LoginModel
    {
        
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string PassWord { get; set; }
    }
    public enum Role
    {
        Admin=1,  
    }
}
