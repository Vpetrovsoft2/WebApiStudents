using Microsoft.AspNetCore.Mvc;
using WebApiStudents.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;
using WebApiStudents.Models.StudentDTO_s;

namespace WebApiStudents.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public StudentsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet(nameof(GetStudents))]
    [SwaggerOperation(
        Summary = "Получение данных по всем студентам",
        Description = "Получает данные всех студентов")]
    [SwaggerResponse(StatusCodes.Status200OK, "Студент испешно найден", typeof(StudentWithFacultetNameDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Ошибка валидации запроса")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Студенты не найдены")]
    public ActionResult<IEnumerable<StudentWithFacultetNameDto>> GetStudents()
    {
        var students = _context.Students!
            .Include(x => x.Facultet)
            .ToList();

        if (students is not null && students.Count > 0)
        {
            var result = _mapper.Map<List<StudentWithFacultetNameDto>>(students);
            return Ok(result);
        }
        return NotFound();
    }

    [HttpGet($"{nameof(GetStudent)}/{{id}}")]
    [SwaggerOperation(
        Summary = "Получение данных студента",
        Description = "Получает данные студента")]
    [SwaggerResponse(StatusCodes.Status200OK, "Студент испешно найден", typeof(UpdateStudentDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Ошибка валидации запроса")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Студент не найден")]
    public ActionResult<Student> GetStudent(int id)
    {
        var student = _context.Students!
            .Include(s => s.Facultet) // Включаем факультет в ответ
            .FirstOrDefault(s => s.Id == id);

        if (student is null)
            return NotFound();

        var studentDto = _mapper.Map<StudentDto>(student);

        return Ok(studentDto);
    }

    [HttpPost($"{nameof(CreateStudent)}")]
    [SwaggerOperation(
        Summary = "Создание нового студента",
        Description = "Создает студента и связывает его с факультетом по ID факультета.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Студент успешно создан", typeof(StudentDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Ошибка валидации запроса")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Факультет не найден")]
    public ActionResult<StudentDto> CreateStudent([FromBody] CreateStudentDto createStudentDto)
    {
        var facultet = _context.Facultes!.Find(createStudentDto.FacultetId);

        if (facultet is null)
            return NotFound($"Facultet with ID {createStudentDto.FacultetId} not found.");

        var student = new Student
        {
            Name = createStudentDto.Name,
            LastName = createStudentDto.LastName,
            Age = createStudentDto.Age,
            FacultetId = facultet.Id
        };

        _context.Students!.Add(student);
        _context.SaveChanges();

        // Загружаем студента вместе с факультетом после сохранения
        var studentWithFacultet = _context.Students
            .Include(s => s.Facultet)
            .FirstOrDefault(s => s.Id == student.Id);

        var result = _mapper.Map<StudentDto>(studentWithFacultet);

        return CreatedAtAction(nameof(GetStudent), new { id = result.Id }, result);
    }

    [HttpPut($"{nameof(UpdateStudent)}/{{id}}")]
    [SwaggerOperation(
        Summary = "Изменение существующего студента",
        Description = "Изменяет студента")]
    [SwaggerResponse(StatusCodes.Status200OK, "Студент испешно изменён", typeof(UpdateStudentDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Ошибка валидации запроса")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Факультет не найден")]
    public IActionResult UpdateStudent(int id, [FromBody] UpdateStudentDto newStudent)
    {
        var student = _context.Students!.Find(id);
        if (student is null)
            return BadRequest();

        student.Name = newStudent.Name ?? student.Name;
        student.LastName = newStudent.LastName ?? student.LastName;
        student.Age = newStudent.Age ?? student.Age;
        student.FacultetId = newStudent.FacultetId ?? student.FacultetId;
        _context.SaveChanges();

        var updatedStudent = _context.Students
          .Include(s => s.Facultet)
          .FirstOrDefault(s => s.Id == id);

        var result = _mapper.Map<StudentDto>(updatedStudent);

        return Ok(result);
    }

    [HttpDelete($"{nameof(DeleteStudent)}/{{id}}")]
    [SwaggerOperation(
        Summary = "Удаление студента",
        Description = "Удаляет студента")]
    [SwaggerResponse(StatusCodes.Status200OK, "Студент испешно удалён", typeof(UpdateStudentDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Ошибка валидации запроса")]
    [SwaggerResponse(StatusCodes.Status404NotFound, $"Студент {{id}} не найден")]
    public IActionResult DeleteStudent(int id) 
    {
        var student = _context.Students!.Find(id);
        if (student is null)
            return NotFound($"Student with id: {id} not found");

        _context.Students!.Remove(student);
        _context.SaveChanges();
        return Ok();
    }
}
