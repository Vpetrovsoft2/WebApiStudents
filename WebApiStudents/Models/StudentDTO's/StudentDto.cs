namespace WebApiStudents.Models;

public class StudentDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }
    public FacultetDto Facultet { get; set; } = null!;
}
