using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiStudents.Models;

public class Student
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }
    public int FacultetId { get; set; }
    public Facultet? Facultet { get; set; }
}
