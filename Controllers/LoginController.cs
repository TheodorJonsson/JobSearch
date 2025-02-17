using Azure;
using JobSearch.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
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


        // Initializes the login controller
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

        // Gets the input from the form then sends this to the next method
        [Route("/Login")]
        [HttpGet]
        public IActionResult Login()
        {
            UserModel user = new UserModel();
            return View(user);
        }
        // Gets the user, not before going through checks.
        [Route("/Login")]
        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            UserMethods userMethods = new UserMethods();
            string errmsg;
            // Checking if both fields has been entered
            if(user.Password != null && user.UserName != null)
            {
                // Checks if the username exists in the database
                if (userMethods.HasUser(user, out errmsg))
                {
                    // Gets the username with the password will return null if the password isnt correct-
                    loggedInUser = userMethods.GetUser(user, out errmsg);
                    if (loggedInUser != null)
                    {
                        string userString = JsonConvert.SerializeObject(loggedInUser);
                        string usernameString = JsonConvert.SerializeObject(loggedInUser.UserName);
                        _contx.HttpContext.Session.SetString(UserSessionKey, userString);
                        _contx.HttpContext.Session.SetString(UserNameSessionKey, usernameString);
                        return Redirect("/");
                    }
                    else
                    {
                        TempData["wrongUser"] = errmsg;
                    }
                }
                else
                {
                    TempData["wrongUser"] = errmsg;
                }
            }
            else
            {
                TempData["WrongUser"] = "Please enter your username and password";
            }
            
            return Redirect("/Login");
        }


        // Creates a new user with the information gotten from the form in the view
        [Route("/CreateUser")]
        [HttpGet]
        public IActionResult CreateUser()
        {
            UserModel user = new UserModel();

            return View(user);
        }
        // Creates the user but first checking if the username is unique or not
        [Route("/CreateUser")]
        [HttpPost]
        public IActionResult CreateUser(UserModel user)
        {
            UserMethods userMethods = new UserMethods();
           
            string errmsg;
            if (user.Password == null || user.UserName == null)
            {
                TempData["wrongInput"] = "Please enter username and password";
                return Redirect("/CreateUser");
            }
            if (userMethods.HasUser(user, out errmsg))
            {
                TempData["wrongInput"] = errmsg;
                return Redirect("/CreateUser");
            }
            else
            {
                userMethods.InsertUser(user, out errmsg);
            }

            return Redirect("/Login");
        }


        // Logs out the user and clears the current session.
        [Route("/Logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            string loggingout = "Logging out";
            System.Diagnostics.Debug.WriteLine(loggingout);

            HttpContext.Session.Remove(UserSessionKey);
            HttpContext.Session.Remove(UserNameSessionKey);
            HttpContext.Session.Clear();
            await HttpContext.Session.CommitAsync();   
            return Redirect("/");

        }
     
    }
}
