using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiStudents.Models;

namespace WebApiStudents.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FacultetsController : ControllerBase
{
    private readonly AppDbContext _context;

    public FacultetsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet(nameof(GetFacultets))]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<Facultet>> GetFacultets() 
    { 
        List<Facultet>? result = _context.Facultes!.ToList(); 
        if (result is not null && result.Count > 0) 
            return Ok(result);
        else
            return BadRequest("Facultets not found");
    }

    [HttpGet($"{nameof(GetFacultet)}/{{id}}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Facultet> GetFacultet(int id)
    {
        var facultet = _context.Facultes!.Find(id);
        if (facultet == null) 
            return BadRequest("Facultet not found");
        return Ok(facultet);
    }

    [HttpPost(nameof(CreateFacultet))]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdateFacultet(int id, Facultet facultet)
    {
        if (id != facultet.Id)
            return BadRequest("Something went wrong");

        _context.Entry(facultet).State = EntityState.Modified;
        _context.SaveChanges();

        return Ok(_context.Facultes!.Find(id));
    }

    [HttpDelete($"{nameof(DeleteFacultet)}/{{id}}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
