using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace UTSCitas_API.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public HealthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("sql")]
        public IActionResult Sql()
        {
            var connStr = _config.GetConnectionString("DefaultConnection");

            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT 1", conn))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null && Convert.ToInt32(result) == 1)
                            return Ok(new { status = "Healthy" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Unhealthy", error = ex.Message });
            }

            return StatusCode(500, new { status = "Unhealthy" });
        }
    }
}
