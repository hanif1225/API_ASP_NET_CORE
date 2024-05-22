using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using System.Numerics;
using TestAPI2024.Data;
using TestAPI2024.Models;

namespace TestAPI2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly DataContext _context;

        public StudentController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            return Ok(await _context.students.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> StoreData(RequestData request)
        {
            var data = new Student()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Nim = request.Nim,
            };

            await _context.AddAsync(data);
            await _context.SaveChangesAsync();

            return Ok(data);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetDetail([FromRoute] Guid id)
        {
            var data = await _context.students.FindAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, RequestData request)
        {
            var data = await _context.students.FindAsync(id);

            if (data != null)
            {
                data.Name = request.Name;
                data.Nim = request.Nim;

                await _context.SaveChangesAsync();
                return Ok(data);
            }

            return NotFound();
        }


        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var data = await _context.students.FindAsync(id);

            if (data != null)
            {
                _context.Remove(data);
                await _context.SaveChangesAsync();
                return Ok(data);
            }

            return NotFound();
        }

        //Contoh Query
        [HttpGet("ApiBaru/getStudent")]
        [ProducesResponseType(typeof(List<Student>), (int)HttpStatusCode.OK)]
        public JsonResult getStudent()
        {
            var sql = "SELECT * FROM students";

            var results = _context.students.FromSqlRaw(sql).ToList();
            return new JsonResult(results);
        }

        [Route("Store/DataStudentNew")]
        [HttpPost]
        [ProducesResponseType(typeof(List<Student>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> StoreData2([FromBody] RequestData request)
        {
            Guid Id = Guid.NewGuid();
            if (!PostStudent(Id, request.Name, request.Nim))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return Ok();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public bool PostStudent(Guid Id, string Name, string Nim)
        {
            try
            {
                var sql = "INSERT INTO students (Id, Name, Nim) VALUES (@id, @name, @nim)";
                var parameters = new[] {
                    new SqlParameter("@id", SqlDbType.UniqueIdentifier) { Value = Id },
                    new SqlParameter("@name", SqlDbType.NVarChar) { Value = Name },
                    new SqlParameter("@nim", SqlDbType.NVarChar) { Value = Nim },
                };

                _context.Database.ExecuteSqlRaw(sql, parameters);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
