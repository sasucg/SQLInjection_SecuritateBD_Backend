using Microsoft.AspNetCore.Mvc;
using SQLInjection_Backend.DTOs;
using System.Data.SqlClient;

namespace SQLInjection_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SqlInjectionController : Controller
    {
        private readonly ILogger<SqlInjectionController> _logger;

        private string connectionString = "Data Source=DESKTOP-A4GSRAP\\SECURITYEXPRESS;Initial Catalog=SqlInjectionProject_dev;Integrated Security=True";

        public SqlInjectionController(ILogger<SqlInjectionController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("getUsers")]
        public GetUsersEvent GetUsers([FromQuery] GetUsersByFilters filters)
        {
            List<User> users = new List<User>();

            string returnedMessage = null;

            string lastName = filters.LastName;
            string firstName = filters.FirstName;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(connectionString))
                {
                    myConnection.Open();

                    string sqlInstruction = "" +
                        "select * " +
                        "from UserXSubscription USX " +
                        "join [User] U on U.UserId = USX.UserId " +
                        "where U.LastName = '@lastName' " +
                        "and U.FirstName = '@firstName' " +
                        "";

                    SqlCommand sqlCommand = new SqlCommand(sqlInstruction, myConnection);
                    sqlCommand.Parameters.Add("@lastName", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters.Add("@firstName", System.Data.SqlDbType.VarChar);

                    sqlCommand.Parameters["@lastName"].Value = lastName;
                    sqlCommand.Parameters["@firstName"].Value = firstName;

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {   
                        returnedMessage = reader.HasRows ? "Utilizatorul are un abonament activ" : "Utilizatorul NU are un abonament activ";

                        myConnection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                returnedMessage = ex.Message;
            }

            return new GetUsersEvent() { Message = returnedMessage };
        }
    }
}