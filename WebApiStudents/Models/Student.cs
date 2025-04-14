namespace WebApiStudents.Models;

public class Student
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }
    public int FacultetId { get; set; }
    public Facultet? Facultet { get; set; }
}
