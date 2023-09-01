using Sams.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sams.Extensions.Dal
{
    public interface IAccountData
    {
        IList<Account> ListOfAccounts();
        Account AccountByCredentials(string userName, string password);
    }
}
