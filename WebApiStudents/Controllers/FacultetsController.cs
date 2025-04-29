using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using WebApiStudents.Models;
using WebApiStudents.Models.FacultetDTO_s;
using WebApiStudents.Models.StudentDTO_s;

namespace WebApiStudents.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FacultetsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public FacultetsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet(nameof(GetFacultets))]
    [SwaggerOperation(
        Summary = "Получение всех факультетов",
        Description = "Получает все факультеты")]
    [SwaggerResponse(StatusCodes.Status200OK, "Факультеты успешно найдены", typeof(FacultetDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Ошибка валидации запроса")]
    [SwaggerResponse(StatusCodes.Status404NotFound, $"Ни один факультет не найден")]
    public ActionResult<IEnumerable<FacultetDto>> GetFacultets() 
    { 
        List<Facultet>? facultets = _context.Facultes!
            .Include(x => x.Students)
            .ToList(); 

        if (facultets is not null && facultets.Count > 0)
        {
            var fullResult = _mapper.Map<List<FacultetDto>>(facultets);
            return Ok(fullResult);
        }
        else
            return BadRequest("Facultets not found");
    }

    [HttpGet($"{nameof(GetFacultet)}/{{id}}")]
    [SwaggerOperation(
        Summary = "Получение факультета",
        Description = "Получает факультет")]
    [SwaggerResponse(StatusCodes.Status200OK, $"Факультет успешно найден", typeof(UpdateStudentDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Ошибка валидации запроса")]
    [SwaggerResponse(StatusCodes.Status404NotFound, $"Факультет не найден")]
    public ActionResult<Facultet> GetFacultet(int id)
    {
        var facultet = _context.Facultes!
            .Include(x => x.Students)
            .FirstOrDefault(x => x.Id == id);

        if (facultet is null) 
            return NotFound();

        var result = _mapper.Map<FacultetDto>(facultet);
        return Ok(result);
    }

    [HttpPost(nameof(CreateFacultet))]
    [SwaggerOperation(
        Summary = "Создание факультета",
        Description = "Создаёт факультет")]
    [SwaggerResponse(StatusCodes.Status200OK, $"Факультет успешно создан", typeof(UpdateStudentDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound, $"Не удалось создать факультет")]
    public ActionResult<Facultet> CreateFacultet(string facultetName)
    {
        var facultet = new Facultet
        {
            Name = facultetName
        };

        // Добавляем факультет в базу данных
        _context.Facultes!.Add(facultet);
        _context.SaveChanges();

        // Возвращаем созданный факультет
        var result = CreatedAtAction(nameof(GetFacultet), new { id = facultet.Id }, facultet);
        if (result is null)
            return BadRequest($"Failed to create {facultet.Name}");
        return Ok(result);
    }

    [HttpPut($"{nameof(UpdateFacultet)}/{{id}}")]
    [SwaggerOperation(
        Summary = $"Факультет успешно изменён",
        Description = "Изменяет факультет")]
    [SwaggerResponse(StatusCodes.Status200OK, $"Факультет успешно изменён", typeof(UpdateFacultetDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Ошибка валидации запроса")]
    [SwaggerResponse(StatusCodes.Status404NotFound, $"Не удалось изменить факультет")]
    public IActionResult UpdateFacultet(int id, UpdateFacultetDto facultet)
    {
        var oldFacultet = _context.Facultes!
            .Include(x => x.Students)
            .FirstOrDefault(x => x.Id == id);

        if (oldFacultet is null)
            return BadRequest();

        oldFacultet.Name = facultet.Name;
        _context.SaveChanges();

        var result = _mapper.Map<FacultetDto>(oldFacultet);

        return Ok(result);
    }

    [HttpDelete($"{nameof(DeleteFacultet)}/{{id}}")]
    [SwaggerOperation(
        Summary = $"Удаление факультета",
        Description = "Удаляет факультет")]
    [SwaggerResponse(StatusCodes.Status200OK, $"Факультет успешно удалён", typeof(UpdateStudentDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Ошибка валидации запроса")]
    [SwaggerResponse(StatusCodes.Status404NotFound, $"Не удалось удалить факультет")]
    public IActionResult DeleteFacultet(int id) 
    { 
        var facultet = _context.Facultes!.Find(id);
        if (facultet is null)
            return NotFound();

        var students = _context.Students!.Where(x => x.FacultetId == id).ToList();
        _context.Students!.RemoveRange(students);

        _context.Facultes.Remove(facultet);
        _context.SaveChanges();

        return Ok();
    }
}
