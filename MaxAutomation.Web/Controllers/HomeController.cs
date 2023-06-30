using iTextSharp.text;
using iTextSharp.text.pdf;
using MaxAutomation.Business;
using MaxAutomation.Dal.DAL;
using MaxAutomation.Data;
using MaxAutomation.Logger;
using MaxAutomation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MaxAutomation
{
   
     
    public class HomeController : Controller
    {
        private readonly IMasterBusiness _master;
        private readonly ILoggerManager _loggerManager;
        private readonly IAccountData _accountData;
        public HomeController(IMasterBusiness master,ILoggerManager loggerManager,IAccountData accountData)
        {
            _master = master;
            _loggerManager = loggerManager;
            _accountData = accountData;
        }
        public ActionResult Login()
        {
            return View(new LoginModel {UserName=string.Empty,PassWord=string.Empty });
        }
        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var validatedUser= _accountData.AccountByCredentials(loginModel.UserName, loginModel.PassWord);
            
                if (validatedUser!=null)
                {
                    FormsAuthentication.SetAuthCookie(loginModel.UserName, false);
                    Session["UserInfo"] = validatedUser;
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError("", "invalid Username or Password");
            return View();
        }
        public ActionResult Index()
        {
            if (Session == null || Session["UserInfo"] == null)
                return RedirectToAction("Login");
            else
                return RedirectToAction("Index", "Branch");
          
        }
        public ActionResult Logout()
        {
            Session["UserInfo"] = null;

            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }


    }
}