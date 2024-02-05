using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<USER>>> GetUsers()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SELECT * FROM [TBL_USERS]", connection))
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        var users = new List<USER>();

                        while (await reader.ReadAsync())
                        {
                            // Assuming you have a User model with properties Id, Username, and Email
                            var user = new USER
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                FNAME = reader["FNAME"].ToString(),
                                LNAME = reader["LNAME"].ToString(),
                                EMAIL = reader["EMAIL"].ToString(),
                            };

                            users.Add(user);
                        }

                        return Ok(users);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database Error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("AddNewUser")]
        public async Task<ActionResult<USER>> InsertUser([FromBody] USER user)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    if (user.ID == -1)
                    {
                        using (SqlCommand command = new SqlCommand(
                            "INSERT INTO [TBL_USERS] (FNAME, LNAME, EMAIL) VALUES (@FNAME, @LNAME, @EMAIL); SELECT SCOPE_IDENTITY()",
                            connection))
                        {
                            command.Parameters.AddWithValue("@FNAME", user.FNAME);
                            command.Parameters.AddWithValue("@LNAME", user.LNAME);
                            command.Parameters.AddWithValue("@EMAIL", user.EMAIL);

                            int newUserId = Convert.ToInt32(await command.ExecuteScalarAsync());

                            user.ID = newUserId;

                            return Ok(user);
                        }
                    }
                    else
                    {
                        using (SqlCommand command = new SqlCommand(
                            "UPDATE [TBL_USERS] SET FNAME = @FNAME, LNAME = @LNAME, EMAIL = @EMAIL WHERE ID = @ID",
                            connection))
                        {
                            command.Parameters.AddWithValue("@FNAME", user.FNAME);
                            command.Parameters.AddWithValue("@LNAME", user.LNAME);
                            command.Parameters.AddWithValue("@EMAIL", user.EMAIL);
                            command.Parameters.AddWithValue("@ID", user.ID);

                            await command.ExecuteNonQueryAsync();

                            return Ok(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database Error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public async Task<ActionResult<USER>> DeleteUser(int id)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(
                        "DELETE FROM [TBL_USERS] WHERE ID = @ID",
                        connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        // ExecuteNonQueryAsync is used for operations that do not return a result set
                        await command.ExecuteNonQueryAsync();

                        return Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database Error: {ex.Message}");
            }
        }
    }
}
