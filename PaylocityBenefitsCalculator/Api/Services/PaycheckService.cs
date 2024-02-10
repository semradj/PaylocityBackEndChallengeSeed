using Api.Models;
using Api.Repository;

namespace Api.Services;

public interface IPaycheckService
{
    Task<Paycheck> GetPaycheckForEmployee(int employeeId, int year, int paycheckNumber);
    Task<List<Paycheck>> GetPaychecksForEmployee(int employeeId, int year);
}

public class PaycheckService : IPaycheckService
{
    // improvement: depending how often is this going to change, we can load it either from config, or database
    /// <summary>
    /// Employee base base cost per month
    /// </summary>
    private readonly decimal _employeeBaseCost = 1000.00m;
    /// <summary>
    /// High employee salary threashold
    /// </summary>
    private readonly decimal _highSalaryThreshold = 80000.00m;
    /// <summary>
    /// High salary benefit increase percentage per year
    /// </summary>
    private readonly decimal _highSalaryPercentCost = 0.02m;
    /// <summary>
    /// Dependent base benefit cost per month
    /// </summary>
    private readonly decimal _dependentBaseCost = 600.00m;
    /// <summary>
    /// Dependent age threashold
    /// </summary>
    private readonly int _dependentAgeThreshold = 50;
    /// <summary>
    /// Dependent age cost increase per month
    /// </summary>
    private readonly decimal _dependentAgeCost = 200.00m;
    /// <summary>
    /// Number of decimal points to use in calculations
    /// </summary>
    private readonly int _decimalPoints = 2;
    /// <summary>
    /// Number of paychecks to be calculated per year
    /// </summary>
    private readonly int _paychecksPerYear = 26;
    /// <summary>
    /// Paycheck period in days
    /// </summary>
    private readonly int _paycheckPeriodInDays = 14;

    private readonly IEmployeesRepository _employeesRepository;

    public PaycheckService(IEmployeesRepository employeesRepository)
    {
        _employeesRepository = employeesRepository;
    }

    /// <summary>
    /// Calculate one paycheck for employee in required year
    /// </summary>
    /// <param name="employeeId">Employee id</param>
    /// <param name="year">year for which paycheck is calculated</param>
    /// <param name="paycheckNumber">Paycheck number in selected year (numbers from 1 to 26)</param>
    /// <returns>Employee paycheck</returns>
    /// <exception cref="NullReferenceException">Thrown if no employee with provided id is found</exception>
    public async Task<Paycheck> GetPaycheckForEmployee(int employeeId, int year, int paycheckNumber)
    {
        (Employee employee, decimal yearlyBenefits, decimal twoWeeksBenefits, decimal twoWeeksSalary) = await PrepareData(employeeId, year);
        DateTime startDate = new(year, 1, 1);

        // improve: sort leap-year as it has one more day, and our last paycheck will end on December 30 rather than December 31
        var paycheck = new Paycheck()
        {
            Employee = employee,
            PaycheckStartDate = startDate.AddDays(_paycheckPeriodInDays * (paycheckNumber - 1)),
            PaycheckEndDate = startDate.AddDays(_paycheckPeriodInDays * paycheckNumber - 1),
            YearlySalary = employee.Salary,
            Salary = twoWeeksSalary,
            YearlyBenefits = yearlyBenefits,
            Benefits = twoWeeksBenefits,
        };

        return paycheck;
    }

    /// <summary>
    /// Calculates paychecks for employee for required year
    /// </summary>
    /// <param name="employeeId">Employee id</param>
    /// <param name="year">Year for which paycheck is calculated</param>
    /// <returns>List of employee paychecks</returns>
    /// <exception cref="NullReferenceException">Thrown if no employee with provided id is found</exception>
    public async Task<List<Paycheck>> GetPaychecksForEmployee(int employeeId, int year)
    {
        (Employee employee, decimal yearlyBenefits, decimal twoWeeksBenefits, decimal twoWeeksSalary) = await PrepareData(employeeId, year);
        DateTime startDate = new(year, 1, 1);
        List<Paycheck> paychecks = new();

        // improve: sort leap-year as it has one more day, and our last paycheck will end on December 30 rather than December 31
        for (int i = 0; i < _paychecksPerYear; i++)
        {
            paychecks.Add(
                new()
                {
                    Employee = employee,
                    PaycheckStartDate = startDate,
                    PaycheckEndDate = startDate.AddDays(_paycheckPeriodInDays),
                    YearlySalary = employee.Salary,
                    Salary = twoWeeksSalary,
                    YearlyBenefits = yearlyBenefits,
                    Benefits = twoWeeksBenefits,
                });

            startDate = startDate.AddDays(_paycheckPeriodInDays);
        }

        return paychecks;
    }

    /// <summary>
    /// Splits benefits and salary into 26 (almost) equal parts
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    private async Task<(Employee Employee, decimal YearlyBenefits, decimal TwoWeeksBenefits, decimal TwoWeeksSalary)> PrepareData(int employeeId, int year)
    {

        // get employee
        var employee = await _employeesRepository.GetEmployeeById(employeeId);

        // calculate benefits
        var yearlyBenefits = Math.Round(CalculateBenefits(employee, year), _decimalPoints);

        // improve: .net uses bankers rounding MidpointRounding.ToEven, check if it is the right one to use
        // improve: if this sum doesn't match result fully, we can do small tweaks and add / remove from last months few cents to make it exact value
        var twoWeeksBenefits = Math.Round(yearlyBenefits / _paychecksPerYear, _decimalPoints);
        var twoWeeksSalary = Math.Round(employee.Salary / _paychecksPerYear, _decimalPoints);

        return (Employee: employee, YearlyBenefits: yearlyBenefits, TwoWeeksBenefits: twoWeeksBenefits, TwoWeeksSalary: twoWeeksSalary);
    }

    /// <summary>
    /// Calculate benefits based on requirements
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    private decimal CalculateBenefits(Employee employee, int year)
    {
        // total benefits sum
        decimal tbs = 12 * _employeeBaseCost;

        // if salary > $80.000 add 2% of the salary
        if (employee.Salary > _highSalaryThreshold)
            tbs += employee.Salary * _highSalaryPercentCost;

        // if employee didn't contain list of all the dependents, we'll have to query dependents repository
        // question: in requirements, it says that employee has only one partner / spouse, I assume that it is checked during employee creation, so there's no need to do this check here
        if (employee.Dependents.Any())
        {
            foreach (var d in employee.Dependents)
            {
                // check if dependent was alive
                if (d.DateOfBirth.Year < year)
                {
                    // for whole year
                    tbs += 12 * _dependentBaseCost;
                }
                else if (d.DateOfBirth.Year == year)
                {
                    // only part of the year
                    tbs += CalculateBenefitMonthProportion(year, d.DateOfBirth, _dependentBaseCost);
                }

                // check if elder dependent was old enough
                if (d.DateOfBirth.Year < year - _dependentAgeThreshold)
                {
                    // older for whole period
                    tbs += 12m * _dependentAgeCost;
                }
                else if (d.DateOfBirth.Year == year - _dependentAgeThreshold)
                {
                    // only for some part of the year
                    tbs += CalculateBenefitMonthProportion(year, d.DateOfBirth, _dependentAgeCost);
                }
            }
        }

        return tbs;
    }

    /// <summary>
    /// Calculates proportional benefit value for non-complete months
    /// </summary>
    /// <param name="year">Year for which paycheck is calculated</param>
    /// <param name="dob">Dependent date of birth</param>
    /// <param name="monthlyBenefit">Benefit value</param>
    /// <returns>Proportional benefit cost value</returns>
    private decimal CalculateBenefitMonthProportion(int year, DateTime dob, decimal monthlyBenefit)
    {
        decimal tbs = 0m;

        // calculate whole months
        tbs += monthlyBenefit * (12 - dob.Month);

        // calculate that one uncomplete month
        var daysInMonth = DateTime.DaysInMonth(year, dob.Month);
        decimal daysActive = daysInMonth - (dob.Day - 1); // day of birth is inlcuded remove -1 to exclude that day
        decimal prop = daysActive / daysInMonth;
        tbs += monthlyBenefit * prop;

        return tbs;
    }
}
