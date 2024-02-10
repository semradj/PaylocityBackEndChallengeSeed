using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Dtos.Paycheck;

namespace Api.Dtos;

internal static class DtoExtensions
{
    public static GetEmployeeDto ToGetEmployeeDto(this Models.Employee employee)
    {
        return new GetEmployeeDto()
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            DateOfBirth = employee.DateOfBirth,
            Salary = employee.Salary,
            Dependents = employee.Dependents.Select(x => x.ToGetDependentDto()).ToList(),
        };
    }

    public static GetDependentDto ToGetDependentDto(this Models.Dependent dependent)
    {
        return new GetDependentDto()
        {
            Id = dependent.Id,
            FirstName = dependent.FirstName,
            LastName = dependent.LastName,
            DateOfBirth = dependent.DateOfBirth,
            Relationship = dependent.Relationship,
        };
    }

    public static GetPaycheckDto ToGetPaycheckDto(this Models.Paycheck paycheck)
    {
        return new GetPaycheckDto()
        {
            FirstName = paycheck.Employee.FirstName,
            LastName = paycheck.Employee.LastName,
            DateOfBirth = paycheck.Employee.DateOfBirth,
            DependentsNumber = paycheck.Employee.Dependents.Count,
            PaycheckStartDate = paycheck.PaycheckStartDate,
            PaycheckEndDate = paycheck.PaycheckEndDate,
            YearlySalary = paycheck.YearlySalary,
            Salary = paycheck.Salary,
            YearlyBenefits = paycheck.YearlyBenefits,
            Benefits = paycheck.Benefits,
        };
    }
}