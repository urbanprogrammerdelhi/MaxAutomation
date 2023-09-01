using Sams.Extensions.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfDemo.Dal
{
    public class BaseDataAccess
    {
        protected readonly string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ParseToText();
    }
}
