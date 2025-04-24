namespace WebApiStudents.Models;

public class CreateStudentDto
{
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }
    public int FacultetId { get; set; }
    //public string? FacultetName { get; set; }
}
