using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using Microsoft.AspNetCore.Mvc;

namespace CamAI.EdgeBox.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController(EmployeeService employeeService) : Controller
{
    [HttpGet]
    public List<Employee> GetEmployees()
    {
        return GlobalData.Employees;
    }

    [HttpGet("{id}")]
    public Employee GetEmployee([FromRoute] Guid id)
    {
        return GlobalData.Employees.Find(x => x.Id == id)
            ?? throw new Exception("Employee not found");
    }

    [HttpPost]
    public Employee AddEmployee([FromBody] Employee employeeDto)
    {
        var employee = employeeService.UpsertEmployee(employeeDto);
        GlobalData.Employees = employeeService.GetEmployee();
        return employee;
    }

    [HttpPut("{id}")]
    public Employee UpdateEmployee([FromRoute] Guid id, [FromBody] Employee employeeDto)
    {
        employeeDto.Id = id;
        var employee = employeeService.UpsertEmployee(employeeDto);
        GlobalData.Employees = employeeService.GetEmployee();
        return employee;
    }
    //
    // [HttpDelete("{id}")]
    // public void DeleteEmployee([FromRoute] Guid id)
    // {
    //     employeeService.DeleteEmployee(id);
    // }
}
