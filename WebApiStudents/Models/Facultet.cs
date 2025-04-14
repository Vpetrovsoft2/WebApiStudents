namespace WebApiStudents.Models;

public class Facultet
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<Student> Students { get; set; } = [];
}
