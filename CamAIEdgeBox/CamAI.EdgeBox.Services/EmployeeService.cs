using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;

namespace CamAI.EdgeBox.Services;

public class EmployeeService(UnitOfWork unitOfWork)
{
    public List<Employee> GetEmployee()
    {
        return unitOfWork.Employees.GetAll();
    }

    public Employee GetEmployeeById(Guid id)
    {
        return unitOfWork.Employees.GetById(id) ?? throw new Exception("Employee not found");
    }

    public Employee UpsertEmployee(Employee employee)
    {
        if (!unitOfWork.Employees.IsExisted(employee.Id))
            // insert
            unitOfWork.Employees.Add(employee);
        else
        {
            // update
            unitOfWork.Employees.Update(employee);
        }
        unitOfWork.Complete();
        return employee;
    }
}
