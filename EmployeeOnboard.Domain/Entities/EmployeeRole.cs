namespace EmployeeOnboard.Domain.Entities;

public class EmployeeRole
{
    public Guid EmployeeId { get; set; }
    public Employee employee { get; set; } = null!;
    public int RoleId { get; set; }
    public Role role { get; set; } = null!;

}
