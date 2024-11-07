using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


using APICORE.Models;

// Agregar todo esto
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Cors;

namespace APICORE.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly string cadenaSQL;

        public TodoController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }

        [HttpGet]
        [Route("")]
        public IActionResult Lista()
        {
            List<Todo> lista = new List<Todo>();

            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("spGetTodos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader()) {

                        while (reader.Read()) {
                            lista.Add(new Todo()
                            {
                                id = Convert.ToInt32(reader["id"]),
                                title = Convert.ToString(reader["title"]),
                                completed = Convert.ToBoolean(reader["completed"])
                            });
                        }
                    }

                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = lista });
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message, response = lista });
            }
        }

        [HttpGet]
        [Route("{idTodo:int}")]
        public IActionResult Obtener(int idTodo)
        {
            List<Todo> lista = new List<Todo>();
            Todo todo = new Todo();

            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("spGetTodos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            lista.Add(new Todo()
                            {
                                id = Convert.ToInt32(reader["id"]),
                                title = Convert.ToString(reader["title"]),
                                completed = Convert.ToBoolean(reader["completed"])
                            });
                        }
                    }

                }

                todo = lista.Where(item => item.id == idTodo).FirstOrDefault();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = todo });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message, response = lista });
            }
        }

        [HttpPost]
        [Route("")]
        public IActionResult Guardar([FromBody] Todo objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("spCreateTodo", conexion);
                    cmd.Parameters.AddWithValue("title", objeto.title);
                    cmd.Parameters.AddWithValue("completed", objeto.completed);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();
                }


                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }


        [HttpPut]
        [Route("")]
        public IActionResult Editar([FromBody] Todo objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("spUpdateTodo", conexion);
                    cmd.Parameters.AddWithValue("id", objeto.id == 0 ? DBNull.Value : objeto.id);
                    cmd.Parameters.AddWithValue("title", objeto.title is null ? DBNull.Value : objeto.title);
                    cmd.Parameters.AddWithValue("completed", objeto.completed);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();
                }


                return StatusCode(StatusCodes.Status200OK, new { mensaje = "editado" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }

        [HttpDelete]
        [Route("{idTodo:int}")]
        public IActionResult Eliminar(int idTodo)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("spDeleteTodo", conexion);
                    cmd.Parameters.AddWithValue("id", idTodo);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();
                }


                return StatusCode(StatusCodes.Status200OK, new { mensaje = "eliminado" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }


    }
}
