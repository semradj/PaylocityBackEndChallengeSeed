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

    public async Task<Paycheck> GetPaycheckForEmployee(int employeeId, int year, int paycheckNumber)
    {
        // get employee
        var employee = await _employeesRepository.GetEmployeeById(employeeId);

        // calculate benefits
        var yearlyBenefits = Math.Round(CalculateBenefits(employee, year), _decimalPoints, MidpointRounding.ToEven);

        // split monthly, to 2 decimal points, use bankers rounding
        // todo: check if bankers rounding is the right one to use
        // todo: if this sum doesn't match result fully, we can do small tweaks and add / remove from last months few cents to make it exact value
        var twoWeeksBenefits = Math.Round(yearlyBenefits / _paychecksPerYear, _decimalPoints, MidpointRounding.ToEven); // bankers rounding, up to discussion if this one is correct
        var twoWeeksSalary = Math.Round(employee.Salary / _paychecksPerYear, _decimalPoints, MidpointRounding.ToEven);

        DateTime startDate = new DateTime(year, 1, 1);


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
        // get employee
        var employee = await _employeesRepository.GetEmployeeById(employeeId);

        // calculate benefits
        var yearlyBenefits = Math.Round(CalculateBenefits(employee, year), _decimalPoints, MidpointRounding.ToEven);

        // split monthly, to 2 decimal points, use bankers rounding
        // todo: check if bankers rounding is the right one to use
        // todo: if this sum doesn't match result fully, we can do small tweaks and add / remove from last months few cents to make it exact value
        var twoWeeksBenefits = Math.Round(yearlyBenefits / _paychecksPerYear, _decimalPoints, MidpointRounding.ToEven); // bankers rounding, up to discussion if this one is correct
        var twoWeeksSalary = Math.Round(employee.Salary / _paychecksPerYear, _decimalPoints, MidpointRounding.ToEven);

        List<Paycheck> paychecks = new List<Paycheck>();

        DateTime startDate = new DateTime(year, 1, 1);

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

    private decimal CalculateBenefits(Employee employee, int year)
    {
        // total benefits sum
        decimal tbs = 12 * _employeeBaseCost;

        // if salary > $80.000 add 2% of the salary
        if (employee.Salary > _highSalaryThreshold)
            tbs += employee.Salary * _highSalaryPercentCost;

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
