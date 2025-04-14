using Microsoft.AspNetCore.Mvc;
using WebApiStudents.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApiStudents.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet(nameof(GetStudents))]
    public ActionResult<IEnumerable<Student>> GetStudents() => _context.Students!.ToList();

    [HttpGet($"{nameof(GetStudent)}/{{id}}")]
    public ActionResult<Student> GetStudent(int id)
    {
        var student = _context.Students!
            .Include(s => s.Facultet) // Включаем факультет в ответ
            .FirstOrDefault(s => s.Id == id);
        if (student is null)
            return NotFound();
        return Ok(student);
    }

    [HttpPost($"{nameof(CreateStudent)}")]
    public ActionResult<Student> CreateStudent(CreateStudentDto createStudentDto)
    {
        var facultet = _context.Facultes!.Find(createStudentDto.FacultetId);
        if (facultet == null)
        {
            return NotFound("Факультет не найден");
        }

        // Создаем нового студента
        var student = new Student
        {
            Name = createStudentDto.Name,
            LastName = createStudentDto.LastName,
            Age = createStudentDto.Age,
            FacultetId = createStudentDto.FacultetId
        };

        // Добавляем студента в базу данных
        _context.Students!.Add(student);
        _context.SaveChanges();

        // Возвращаем созданного студента
        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
    }

    [HttpPut($"{nameof(UpdateStudent)}/{{id}}")]
    public IActionResult UpdateStudent(int id, Student student)
    {
        if (id != student.Id)
            return BadRequest();

        _context.Entry(student).State = EntityState.Modified;
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete($"{nameof(DeleteStudent)}/{{id}}")]
    public IActionResult DeleteStudent(int id) 
    {
        var student = _context.Students!.Find(id);
        if (student is null)
            return NotFound();

        _context.Students!.Remove(student);
        _context.SaveChanges();
        return NoContent();
    }
}
