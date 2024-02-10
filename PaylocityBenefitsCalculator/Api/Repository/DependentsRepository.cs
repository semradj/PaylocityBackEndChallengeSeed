using Api.Models;

namespace Api.Repository;

public interface IDependentsRepository
{
    // if proper data source is used like db or some api, following methods would be Tasks so we can use async / await
    Task<Dependent> GetDependentById(int id);
    Task<List<Dependent>> GetAllDependents();
    Task<List<Dependent>> GetAllDependentsForEmployee(int employeeId);


}

public class DependentsRepository : IDependentsRepository
{
    private List<Dependent> _dataSource;

    public DependentsRepository(List<Dependent> dataSource)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
    }

    /// <summary>
    /// Returns Dependent by id
    /// </summary>
    /// <param name="id">Dependent id</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">Thrown if no dependent with provided id is found</exception>
    public async Task<Dependent> GetDependentById(int id)
    {
        // we use Task.FromResult here to simulate async call for data
        var dependent = await Task.FromResult(_dataSource.FirstOrDefault(x => x.Id == id));

        if (dependent == null)
        {
            throw new NullReferenceException(nameof(dependent));
        }

        return dependent;
    }

    /// <summary>
    /// Returns list of all dependents
    /// </summary>
    /// <returns></returns>
    public async Task<List<Dependent>> GetAllDependents()
    {
        return await Task.FromResult(_dataSource);
    }

    /// <summary>
    /// Returns list of all dependens for provided employee
    /// </summary>
    /// <param name="employeeId">Employee id</param>
    /// <returns></returns>
    public async Task<List<Dependent>> GetAllDependentsForEmployee(int employeeId)
    {
        return await Task.FromResult(_dataSource.Where(x => x.EmployeeId == employeeId).ToList());
    }
}
