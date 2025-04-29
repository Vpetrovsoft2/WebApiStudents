namespace WebApiStudents.Models.StudentDTO_s;

public class StudentWithFacultetNameDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }
    public string FacultetName { get; set; } = null!;
}
