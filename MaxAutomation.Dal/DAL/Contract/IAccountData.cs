using MaxAutomation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxAutomation.Dal.DAL
{
    public interface IAccountData
    {
        IList<Account> ListOfAccounts();
        Account AccountByCredentials(string userName, string password);
    }
}
