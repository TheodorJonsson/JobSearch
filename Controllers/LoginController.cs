using JobSearch.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JobSearch.Controllers
{
    // WORK IN PROGRESS
    public class LoginController : Controller
    {
        private const string UserSessionKey = "Username";
        private const string UserNameSessionKey = "Name";
        private readonly IHttpContextAccessor _contx;
        private UserModel loggedInUser;

        public LoginController(IHttpContextAccessor contx)
        {
            _contx = contx;
            string userKey = _contx.HttpContext.Session.GetString(UserSessionKey);

            if (!string.IsNullOrEmpty(userKey))
            {
                loggedInUser = JsonConvert.DeserializeObject<UserModel>(userKey);

            }
            else
            {
                loggedInUser = new UserModel();
            }
        }

        [Route("/Login")]
        [HttpGet]
        public IActionResult Login()
        {
            UserModel user = new UserModel();
            return View(user);
        }

        [Route("/Login")]
        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            UserMethods userMethods = new UserMethods();
            string errmsg;
            if (userMethods.HasUser(user, out errmsg))
            {
                loggedInUser = userMethods.GetUser(user, out errmsg);
                if (loggedInUser != null)
                {
                    string userString = JsonConvert.SerializeObject(loggedInUser);
                    string usernameString = JsonConvert.SerializeObject(loggedInUser.UserName);
                    _contx.HttpContext.Session.SetString(UserSessionKey, userString);
                    _contx.HttpContext.Session.SetString(UserNameSessionKey, usernameString);
                }
            }
            else
            {
                return Redirect("/Login");
            }
            
            return Redirect("/");
        }

        [Route("/CreateUser")]
        [HttpGet]
        public IActionResult CreateUser()
        {
            UserModel user = new UserModel();

            return View(user);
        }

        [Route("/CreateUser")]
        [HttpPost]
        public IActionResult CreateUser(UserModel user)
        {
            UserMethods userMethods = new UserMethods();
           
            string errmsg;
            if (userMethods.HasUser(user, out errmsg))
            {
                TempData["usernametaken"] = errmsg;
                return Redirect("/CreateUser");
            }
            if(user.Password == null || user.UserName == null)
            {
                return Redirect("/CreateUser");
            }
            else
            {
                userMethods.InsertUser(user, out errmsg);
            }

            return Redirect("/Login");
        }
        [HttpPost]
        public IActionResult Logout()
        {
            _contx.HttpContext.Session.Remove(UserSessionKey);
            _contx.HttpContext.Session.Remove(UserNameSessionKey);
            return Redirect("/");
        }
    }
}
