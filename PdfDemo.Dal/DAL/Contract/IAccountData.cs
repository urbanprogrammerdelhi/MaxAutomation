using PdfDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfDemo.Dal.DAL.Contract
{
    interface IAccountData
    {
        IList<Account> ListOfAccounts();
        Account AccountByCredentials(string userName, string password);
    }
}
