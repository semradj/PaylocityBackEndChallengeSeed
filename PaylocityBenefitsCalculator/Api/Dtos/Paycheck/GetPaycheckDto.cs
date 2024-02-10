namespace Api.Dtos.Paycheck;

public class GetPaycheckDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int DependentsNumber { get; set; }
    public DateTime PaycheckStartDate { get; set; }
    public DateTime PaycheckEndDate { get; set; }
    public decimal YearlySalary { get; set; }
    public decimal Salary { get; set; }
    public decimal YearlyBenefits { get; set; }
    public decimal Benefits { get; set; }
}
