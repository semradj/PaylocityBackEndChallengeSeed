namespace Api.Models;

public class Paycheck
{
    public Employee? Employee { get; set; }
    public DateTime PaycheckEndDate { get; set; }
    public DateTime PaycheckStartDate { get; set; }
    public decimal YearlyBenefits { get; set; }
    public decimal Benefits { get; set; }
    public decimal YearlySalary { get; set; }
    public decimal Salary { get; set; }
}