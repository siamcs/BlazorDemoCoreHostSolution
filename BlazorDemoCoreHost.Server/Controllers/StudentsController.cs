using BlazorDemoCoreHost.Server.Data;
using BlazorDemoCoreHost.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorDemoCoreHost.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StudentsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<Student>>> Get()
        {
            return await _db.Students.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> Get(int id)
        {
            var student = await _db.Students.Include(x => x.Addresses)
               .FirstOrDefaultAsync(x => x.Id == id);
            if (student == null)
            {
                return NotFound("Student info not found");
            }
            return student;
        }


        [HttpPost]
        public async Task<ActionResult<int>> Post(Student student)
        {
            _db.Students.Add(student);
            await _db.SaveChangesAsync();
            return student.Id;
        }

        [HttpPut]

        public async Task<ActionResult> Put(Student student)
        {
            _db.Entry(student).State = EntityState.Modified;
            foreach (var address in student.Addresses)
            {
                if (address.Id != 0)
                {
                    _db.Entry(address).State = EntityState.Modified;
                }
                else
                {
                    _db.Entry(address).State = EntityState.Added;
                }
            }
            var idOfAddrersses = student.Addresses.Select(x => x.Id).ToList();
            var addressToDelete = await _db.Addresses.Where(x => !idOfAddrersses
            .Contains(x.Id) && x.StudentId == student.Id).ToListAsync();
            _db.RemoveRange(addressToDelete);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var student = await _db.Students.Include(s => s.Addresses)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            _db.Addresses.RemoveRange(student.Addresses);
            _db.Students.Remove(student);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
