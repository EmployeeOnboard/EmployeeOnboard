﻿using EmployeeOnboard.Domain.Enums;
using System.ComponentModel.DataAnnotations;


namespace EmployeeOnboard.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EmployeeNumber { get; set; } = string.Empty; //This will be autogenerated
    [Required]
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; //the autogenerated password will be stored here
    public bool IsPasswordChanged { get; set; } = false; //check whether the employee has updated their password
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();
}
