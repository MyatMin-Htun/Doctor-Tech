using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Doc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Doc.Controllers
{
    public class UseCreditController : Controller
    {
        public IConfiguration Configuration { get; }

        public UseCreditController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public CreditDetail creditDetail = new CreditDetail();

        public IActionResult Index()
        {
            // get member id from Session
            ViewData["memberID"] = HttpContext.Session.GetString("memberID");
            // get error message from Session
            ViewData["error"] = HttpContext.Session.GetString("error");
            // set Session["error"] null
            HttpContext.Session.SetString("error", "");
            return View();
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
        public IActionResult InsertCredit(CreditDetail creditDetail)
        {
            if (ModelState.IsValid)
            {
                UserInfo userInfo = getUserInfo(HttpContext.Session.GetString("memberID"));
                // get available credit amount from database
                int availableCredits = Convert.ToInt32(userInfo.availableCredits);
                // get credit amount from razor view
                int creditAmount = Convert.ToInt32(creditDetail.creditAmount);

                if (availableCredits >= creditAmount) {
                    if(availableCredits - creditAmount >= 0)
                    {
                        string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {

                            string sql = $"INSERT INTO [transaction] (memberID, creditAmount, transactionDate) VALUES ('{userInfo.memberID}', {creditAmount},'{DateTime.Now.ToString("yyyy-MM-dd")}')";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.CommandType = CommandType.Text;
                                connection.Open();
                                command.ExecuteNonQuery();

                                connection.Close();
                            }

                            sql = $"UPDATE [user] SET availableCredits={ availableCredits - creditAmount } WHERE memberID='{ HttpContext.Session.GetString("memberID") }'";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.CommandType = CommandType.Text;
                                connection.Open();
                                command.ExecuteNonQuery();

                                connection.Close();
                            }
                        }

                        // clear the Session
                        HttpContext.Session.Clear();
                        // redirect to main menu page
                        return RedirectToAction("Index", "MainMenu");
                    }
                    else
                    {
                        // set error message to Session
                        HttpContext.Session.SetString("error", "You don't have enough credit amount");
                        return RedirectToAction("Index", "UseCredit");
                    }
                }
                else
                {
                    // set error message to Session
                    HttpContext.Session.SetString("error", "You don't have enough credit amount");
                    return RedirectToAction("Index", "UseCredit");
                }
            }
            else
            {
                // set error message to Session
                HttpContext.Session.SetString("error", "You don't have enough credit amount");
                return RedirectToAction("Index", "UseCredit");
            }
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
