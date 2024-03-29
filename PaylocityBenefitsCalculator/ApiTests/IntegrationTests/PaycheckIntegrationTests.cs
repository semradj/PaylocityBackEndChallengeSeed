using Api.Dtos.Paycheck;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ApiTests.IntegrationTests;

public class PaycheckIntegrationTests : IntegrationTest
{
    [Fact]
    public async Task WhenAskedForAnEmployee_ShouldReturnCorrectEmployee()
    {
        var response = await HttpClient.GetAsync("/api/v1/paycheck?employeeId=3&year=2023&paycheckNumber=1");

        var paycheck = new GetPaycheckDto()
        {
            Benefits = 848.62m,
            DateOfBirth = new DateTime(1963, 2, 17),
            DependentsNumber = 1,
            FirstName = "Michael",
            LastName = "Jordan",
            PaycheckStartDate = new DateTime(2023, 1, 1),
            PaycheckEndDate = new DateTime(2023, 1, 14),
            Salary = 5508.12m,
            YearlyBenefits = 22064.22m,
            YearlySalary = 143211.12m
        };

        await response.ShouldReturn(HttpStatusCode.OK, paycheck);
    }
    
    [Fact]
    public async Task WhenAskedForPaychecksForNonExistenEmployee_ShoulReturn404()
    {
        var response = await HttpClient.GetAsync($"/api/v1/paycheck?employeeId={int.MinValue}&year=2023&paycheckNumber=1");
        await response.ShouldReturn(HttpStatusCode.NotFound);
    }
}

