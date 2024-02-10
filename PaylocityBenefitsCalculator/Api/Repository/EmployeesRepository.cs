using Api.Models;

namespace Api.Repository;

public interface IEmployeesRepository
{
    // if proper data source is used like db or some api, following methods would be Tasks so we can use async / await
    Task<Employee> GetEmployeeById(int id);
    Task<List<Employee>> GetAllEmployees();


}

public class EmployeesRepository : IEmployeesRepository
{
    private List<Employee> _dataSource;

    public EmployeesRepository(List<Employee> dataSource)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
    }

    /// <summary>
    /// Returns Employee by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Employee</returns>
    /// <exception cref="NullReferenceException">Thrown if no employee with provided id is found</exception>
    public async Task<Employee> GetEmployeeById(int id)
    {
        var employee = await Task.FromResult(_dataSource.FirstOrDefault(x => x.Id == id));

        if (employee == null)
        {
            throw new NullReferenceException(nameof(employee));
        }

        return employee;
    }

    /// <summary>
    /// Returns list of all employees
    /// </summary>
    /// <returns>List of employees, if there are not employees it returns empty list</returns>
    public async Task<List<Employee>> GetAllEmployees()
    {
        return await Task.FromResult(_dataSource);
    }
}
