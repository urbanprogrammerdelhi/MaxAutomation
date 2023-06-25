using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfDemo.Model
{
    public class Account
    {
        public string UserName { get; set; }
        public string Company { get; set; }
        public int Role { get; set; }
    }
    public enum Role
    {
        Admin=1,  
    }
}
