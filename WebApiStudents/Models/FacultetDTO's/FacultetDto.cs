using WebApiStudents.Models.StudentDTO_s;

namespace WebApiStudents.Models;

public class FacultetDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<StudentSlimDto>? Students { get; set; } = new();
}
