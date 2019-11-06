using System;
using Doc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Doc.Controllers
{
    public class MainMenuController : Controller
    {
        public IConfiguration Configuration { get; }

        public MainMenuController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            // get userinfo by passing memberID
            UserInfo userInfo = getUserInfo("m0001");
            return View(userInfo);
        }

        [HttpPost]
        public IActionResult menuHome()
        {
            return RedirectToAction("Index", "MainMenu");
        }

        [HttpPost]
        public IActionResult menuSearch()
        {
            return RedirectToAction("Index", "MainMenu");
        }

        [HttpPost]
        public IActionResult menuLogin()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult useCredits()
        {
            UserInfo userInfo = getUserInfo("m0001");
            // store member id detail in session
            HttpContext.Session.SetString("memberID", userInfo.memberID);

            //return View("./UseCredit/Index");
            return RedirectToAction("Index", "UseCredit");
        }

        public UserInfo getUserInfo(string memberID)
        {
            // connection string from appsettings.json
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                UserInfo userInfo = new UserInfo();
                //SqlDataReader
                connection.Open();

                string sql = "SELECT * FROM [user] WHERE memberID='" + memberID + "'";
                SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        userInfo.memberID = Convert.ToString(dataReader["memberID"]);
                        userInfo.userName = Convert.ToString(dataReader["userName"]);
                        userInfo.address = Convert.ToString(dataReader["address"]);
                        userInfo.dob = Convert.ToDateTime(dataReader["dob"]);
                        userInfo.employerName = Convert.ToString(dataReader["employerName"]);
                        userInfo.availableCredits = Convert.ToInt32(dataReader["availableCredits"]);
                    }
                }
                connection.Close();
                // return user info detail
                return userInfo;
            }
        }
    }
}
