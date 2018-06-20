using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;
using CursorShops.Models;
using System.Collections.Generic;

namespace CursorShops.Controllers
{
    [Authorize]
    public class MainController : Controller
    {
        public IActionResult Index()
        {
            /*
            List<AzureModel> lst = new List<AzureModel>();
            using (SqlConnection con = new SqlConnection())
            {
                using (SqlCommand com = new SqlCommand())
                {
                    con.ConnectionString = "Server=tcp:easystep-sql.database.windows.net,1433;Initial Catalog=TestDB;Persist Security Info=False;" +
                        "User ID=TestUser;Password=Test2010;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                    con.Open();
                    com.Connection = con;
                    com.CommandType = System.Data.CommandType.Text;
                    com.CommandText = "select [ID],[Country],[Product],[Quantity],[Date] from sales";
                    SqlDataReader dr = com.ExecuteReader();
                    while(dr.Read())
                    {
                        AzureModel model = new AzureModel();
                        model.ID = dr.GetInt32(0);
                        model.Country = dr["Country"].ToString();
                        model.Date = dr.GetDateTime(4);
                        model.Product = dr["Product"].ToString();
                        model.Quantity = dr.GetInt32(3);
                        lst.Add(model);
                    }
                }
            }
            */
                return View();
        }
    }
    
}
