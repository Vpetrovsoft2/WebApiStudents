namespace WebApiStudents.Models;

public class CreateStudentDto
{
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public int FacultetId { get; set; }
}
