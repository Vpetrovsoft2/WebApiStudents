using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
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
    public ActionResult<IEnumerable<Facultet>> GetFacultets() => _context.Facultes!.ToList();

    [HttpGet($"{nameof(GetFacultet)}/{{id}}")]
    public ActionResult<Facultet> GetFacultet(int id)
    {
        var facultet = _context.Facultes!.Find(id);
        if (facultet == null) 
            return NotFound();
        return Ok(facultet);
    }

    [HttpPost(nameof(CreateFacultet))]
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
        return CreatedAtAction(nameof(GetFacultet), new { id = facultet.Id }, facultet);
    }

    [HttpPut($"{nameof(UpdateFacultet)}/{{id}}")]
    public IActionResult UpdateFacultet(int id, Facultet facultet)
    {
        if (id != facultet.Id)
            return BadRequest();

        _context.Entry(facultet).State = EntityState.Modified;
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete($"{nameof(DeleteFacultet)}/{{id}}")]
    public IActionResult DeleteFacultet(int id) 
    { 
        var facultet = _context.Facultes!.Find(id);
        if (facultet is null)
            return NotFound();

        var students = _context.Students!.Where(x => x.FacultetId == id).ToList();
        _context.Students!.RemoveRange(students);

        _context.Facultes.Remove(facultet);
        _context.SaveChanges();
        return NoContent();
    }
}
