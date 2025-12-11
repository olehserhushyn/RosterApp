namespace RosterApp.API.DTOs.Employees.Responses
{
    public record EmployeeDto(
        int Id,
        string FirstName, 
        string LastName,
        string Email,
        DateTime CreatedAt);
}
