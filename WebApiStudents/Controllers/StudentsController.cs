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
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<Student>> GetStudents()
    {
        var result = _context.Students!.ToList();
        if (result is not null && result.Count > 0)
            return Ok(result);
        return BadRequest("Students not found");
    }

    [HttpGet($"{nameof(GetStudent)}/{{id}}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Student> GetStudent(int id)
    {
        var student = _context.Students!
            .Include(s => s.Facultet) // Включаем факультет в ответ
            .FirstOrDefault(s => s.Id == id);

        if (student is null)
            return BadRequest($"Student: {id} not found");
        return Ok(student);
    }

    [HttpPost($"{nameof(CreateStudent)}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Student> CreateStudent(CreateStudentDto createStudentDto)
    {
        var facultet = _context.Facultes!.Find(createStudentDto.FacultetId);
        if (facultet == null)
            return NotFound("Facultet not found");

        // Создаем нового студента
        var student = new Student
        {
            Name = createStudentDto.Name,
            LastName = createStudentDto.LastName,
            Age = createStudentDto.Age,
            FacultetId = createStudentDto.FacultetId,
            Facultet = facultet
        };

        // Добавляем студента в базу данных
        _context.Students!.Add(student);
        _context.SaveChanges();

        var studentWithFacultet = _context.Students
            .Include(s => s.Facultet)
            .FirstOrDefault(s => s.Id == student.Id);

        // Возвращаем созданного студента
        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, studentWithFacultet);
    }

    [HttpPut($"{nameof(UpdateStudent)}/{{id}}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdateStudent(int id, Student student)
    {
        if (id != student.Id)
            return BadRequest("Something went wrong");

        _context.Entry(student).State = EntityState.Modified;
        _context.SaveChanges();
        
        var result = _context.Students!
            .Include(s => s.Facultet)
            .FirstOrDefault(s => s.Id == id);

        if (result is not null)
            return Ok(result);
        return BadRequest("Failed to update student");
    }

    [HttpDelete($"{nameof(DeleteStudent)}/{{id}}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult DeleteStudent(int id) 
    {
        var student = _context.Students!.Find(id);
        if (student is null)
            return BadRequest($"Student with id: {id} not found");

        _context.Students!.Remove(student);
        _context.SaveChanges();
        return Ok();
    }
}
