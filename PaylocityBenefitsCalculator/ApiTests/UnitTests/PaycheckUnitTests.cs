using Api.Models;
using Api.Repository;
using Api.Services;
using Bogus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ApiTests.UnitTests;

public class PaycheckUnitTests
{
    // note: this is only unit test class added in this exercise, but for real project, there will be more unit tests for repositories, ...

    private JsonSerializerSettings _serializerSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

    #region All yearly paychecks
    [Fact]
    public async void WhenAskedForPaychecksForYear_ShouldReturnPaychecksForYear_NoDependents_LowSalary()
    {
        var employeeFaker = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 75000m)
            .RuleFor(r => r.Dependents, new List<Dependent>());

        var e = employeeFaker.Generate();
        var ds = new List<Employee>() { e };

        // improve: in real scenario use mocking library like Moq or NSubstitute
        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        var startDate = new DateTime(2023, 1, 1);

        var expected = new List<Paycheck>();

        for (int i = 0; i < 26; i++)
        {
            expected.Add(new Paycheck()
            {
                Benefits = 461.54m,
                Employee = e,
                PaycheckStartDate = startDate.AddDays(i * 14),
                PaycheckEndDate = startDate.AddDays((i + 1) * 14),
                Salary = 2884.62m,
                YearlyBenefits = 12000.00m,
                YearlySalary = e.Salary
            });
        }

        var result = await sut.GetPaychecksForEmployee(e.Id, 2023);

        Assert.NotNull(result);
        Assert.Equal(26, result.Count());
        Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void WhenAskedForPaychecksForYear_ShouldReturnPaychecksForYear_NoDependents_HighSalary()
    {
        var employeeFaker = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 100000m)
            .RuleFor(r => r.Dependents, new List<Dependent>());

        var e = employeeFaker.Generate();
        var ds = new List<Employee>() { e };

        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        var startDate = new DateTime(2023, 1, 1);

        var expected = new List<Paycheck>();

        for (int i = 0; i < 26; i++)
        {
            expected.Add(new Paycheck()
            {
                Benefits = 538.46m,
                Employee = e,
                PaycheckStartDate = startDate.AddDays(i * 14),
                PaycheckEndDate = startDate.AddDays((i + 1) * 14),
                Salary = 3846.15m,
                YearlyBenefits = 14000.00m,
                YearlySalary = e.Salary
            });
        }

        var result = await sut.GetPaychecksForEmployee(e.Id, 2023);

        Assert.NotNull(result);
        Assert.Equal(26, result.Count());
        Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void WhenAskedForPaychecksForYear_ShouldReturnPaychecksForYear_OneDependent_HighSalary()
    {
        var employeeFaker = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 100000m)
            .RuleFor(r => r.Dependents, new List<Dependent>() { });

        var e = employeeFaker.Generate();


        var dependentsFaker = new Faker<Dependent>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.DateOfBirth, new DateTime(2020, 03, 03))
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.Relationship, Relationship.Spouse)
            .RuleFor(r => r.Employee, e)
            .RuleFor(r => r.EmployeeId, e.Id);

        e.Dependents = dependentsFaker.Generate(1);

        var ds = new List<Employee>() { e };

        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        var startDate = new DateTime(2023, 1, 1);

        var expected = new List<Paycheck>();

        for (int i = 0; i < 26; i++)
        {
            expected.Add(new Paycheck()
            {
                Benefits = 815.38m,
                Employee = e,
                PaycheckStartDate = startDate.AddDays(i * 14),
                PaycheckEndDate = startDate.AddDays((i + 1) * 14),
                Salary = 3846.15m,
                YearlyBenefits = 21200.00m,
                YearlySalary = e.Salary
            });
        }

        var result = await sut.GetPaychecksForEmployee(e.Id, 2023);

        Assert.NotNull(result);
        Assert.Equal(26, result.Count());
        Assert.Equal(JsonConvert.SerializeObject(expected, _serializerSettings), JsonConvert.SerializeObject(result, _serializerSettings));
    }

    [Fact]
    public async void WhenAskedForPaychecksForYear_ShouldReturnPaychecksForYear_ThreeDependent_LowSalary()
    {
        var employeeFaker = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 60000m)
            .RuleFor(r => r.Dependents, new List<Dependent>() { });

        var e = employeeFaker.Generate();


        var dependentsFaker = new Faker<Dependent>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.DateOfBirth, new DateTime(2020, 03, 03))
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.Relationship, Relationship.Spouse)
            .RuleFor(r => r.Employee, e)
            .RuleFor(r => r.EmployeeId, e.Id);

        e.Dependents = dependentsFaker.Generate(3);

        var ds = new List<Employee>() { e };

        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        var startDate = new DateTime(2023, 1, 1);

        var expected = new List<Paycheck>();

        for (int i = 0; i < 26; i++)
        {
            expected.Add(new Paycheck()
            {
                Benefits = 1292.31m,
                Employee = e,
                PaycheckStartDate = startDate.AddDays(i * 14),
                PaycheckEndDate = startDate.AddDays((i + 1) * 14),
                Salary = 2307.69m,
                YearlyBenefits = 33600.00m,
                YearlySalary = e.Salary
            });
        }

        var result = await sut.GetPaychecksForEmployee(e.Id, 2023);

        Assert.NotNull(result);
        Assert.Equal(26, result.Count());
        Assert.Equal(JsonConvert.SerializeObject(expected, _serializerSettings), JsonConvert.SerializeObject(result, _serializerSettings));
    }

    [Fact]
    public async void WhenAskedForPaychecksForYear_ShouldReturnPaychecksForYear_OneDependentOldPartTime_LowSalary()
    {
        var employeeFaker = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 60000m)
            .RuleFor(r => r.Dependents, new List<Dependent>() { });

        var e = employeeFaker.Generate();


        var dependentsFaker = new Faker<Dependent>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.DateOfBirth, new DateTime(1973, 4, 17))
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.Relationship, Relationship.Spouse)
            .RuleFor(r => r.Employee, e)
            .RuleFor(r => r.EmployeeId, e.Id);

        e.Dependents = dependentsFaker.Generate(1);

        var ds = new List<Employee>() { e };

        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        var startDate = new DateTime(2023, 1, 1);

        var expected = new List<Paycheck>();

        for (int i = 0; i < 26; i++)
        {
            expected.Add(new Paycheck()
            {
                Benefits = 803.59m,
                Employee = e,
                PaycheckStartDate = startDate.AddDays(i * 14),
                PaycheckEndDate = startDate.AddDays((i + 1) * 14),
                Salary = 2307.69m,
                YearlyBenefits = 20893.33m,
                YearlySalary = e.Salary
            });
        }

        var result = await sut.GetPaychecksForEmployee(e.Id, 2023);

        Assert.NotNull(result);
        Assert.Equal(26, result.Count());
        Assert.Equal(JsonConvert.SerializeObject(expected, _serializerSettings), JsonConvert.SerializeObject(result, _serializerSettings));
    }

    [Fact]
    public async void WhenAskedForPaychecksForYear_ShouldReturnPaychecksForYear_OneDependentYoungPartTime_HighSalary()
    {
        var employeeFaker = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 100000m)
            .RuleFor(r => r.Dependents, new List<Dependent>() { });

        var e = employeeFaker.Generate();


        var dependentsFaker = new Faker<Dependent>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.DateOfBirth, new DateTime(2023, 6, 14))
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.Relationship, Relationship.Spouse)
            .RuleFor(r => r.Employee, e)
            .RuleFor(r => r.EmployeeId, e.Id);

        e.Dependents = dependentsFaker.Generate(1);

        var ds = new List<Employee>() { e };

        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        var startDate = new DateTime(2023, 1, 1);

        var expected = new List<Paycheck>();

        for (int i = 0; i < 26; i++)
        {
            expected.Add(new Paycheck()
            {
                Benefits = 690.00m,
                Employee = e,
                PaycheckStartDate = startDate.AddDays(i * 14),
                PaycheckEndDate = startDate.AddDays((i + 1) * 14),
                Salary = 3846.15m,
                YearlyBenefits = 17940.00m,
                YearlySalary = e.Salary
            });
        }

        var result = await sut.GetPaychecksForEmployee(e.Id, 2023);

        Assert.NotNull(result);
        Assert.Equal(26, result.Count());
        Assert.Equal(JsonConvert.SerializeObject(expected, _serializerSettings), JsonConvert.SerializeObject(result, _serializerSettings));
    }

    [Fact]
    public async void WhenAskedForPaychecksForNonExistenEmployee_ShoulThrowAnError()
    {
        var employee = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 75000m)
            .RuleFor(r => r.Dependents, new List<Dependent>());

        var ds = new List<Employee>() { employee.Generate() };

        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        await Assert.ThrowsAsync<NullReferenceException>(() => sut.GetPaychecksForEmployee(0, 2023));
    }
    #endregion

    #region Specific paycheck
    [Fact]
    public async void WhenAskedForPaycheck_ShouldReturnPaycheck_NoDependents_LowSalary()
    {
        var employeeFaker = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 75000m)
            .RuleFor(r => r.Dependents, new List<Dependent>());

        var e = employeeFaker.Generate();
        var ds = new List<Employee>() { e };

        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        var startDate = new DateTime(2023, 1, 1);

        var expected = new Paycheck()
        {
            Benefits = 461.54m,
            Employee = e,
            PaycheckStartDate = startDate,
            PaycheckEndDate = startDate.AddDays(13),
            Salary = 2884.62m,
            YearlyBenefits = 12000.00m,
            YearlySalary = e.Salary
        };

        var result = await sut.GetPaycheckForEmployee(e.Id, 2023, 1);

        Assert.NotNull(result);
        Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void WhenAskedForPaycheck_ShouldReturnPaycheck_NoDependents_HighSalary()
    {
        var employeeFaker = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 100000m)
            .RuleFor(r => r.Dependents, new List<Dependent>());

        var e = employeeFaker.Generate();
        var ds = new List<Employee>() { e };

        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        var startDate = new DateTime(2023, 1, 1);

        var expected = new Paycheck()
        {
            Benefits = 538.46m,
            Employee = e,
            PaycheckStartDate = startDate,
            PaycheckEndDate = startDate.AddDays(13),
            Salary = 3846.15m,
            YearlyBenefits = 14000.00m,
            YearlySalary = e.Salary
        };

        var result = await sut.GetPaycheckForEmployee(e.Id, 2023, 1);

        Assert.NotNull(result);
        Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void WhenAskedForPaycheck_ShouldReturnPaycheck_OneDependent_HighSalary()
    {
        var employeeFaker = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 100000m)
            .RuleFor(r => r.Dependents, new List<Dependent>() { });

        var e = employeeFaker.Generate();

        var dependentsFaker = new Faker<Dependent>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.DateOfBirth, new DateTime(2020, 03, 03))
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.Relationship, Relationship.Spouse)
            .RuleFor(r => r.Employee, e)
            .RuleFor(r => r.EmployeeId, e.Id);

        e.Dependents = dependentsFaker.Generate(1);

        var ds = new List<Employee>() { e };

        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        var startDate = new DateTime(2023, 1, 15);

        var expected = new Paycheck()
        {
            Benefits = 815.38m,
            Employee = e,
            PaycheckStartDate = startDate,
            PaycheckEndDate = startDate.AddDays(13),
            Salary = 3846.15m,
            YearlyBenefits = 21200.00m,
            YearlySalary = e.Salary
        };

        var result = await sut.GetPaycheckForEmployee(e.Id, 2023, 2);

        Assert.NotNull(result);
        Assert.Equal(JsonConvert.SerializeObject(expected, _serializerSettings), JsonConvert.SerializeObject(result, _serializerSettings));
    }

    [Fact]
    public async void WhenAskedForPaycheck_ShouldReturnPaycheck_ThreeDependent_LowSalary()
    {
        var employeeFaker = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 60000m)
            .RuleFor(r => r.Dependents, new List<Dependent>() { });

        var e = employeeFaker.Generate();

        var dependentsFaker = new Faker<Dependent>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.DateOfBirth, new DateTime(2020, 03, 03))
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.Relationship, Relationship.Spouse)
            .RuleFor(r => r.Employee, e)
            .RuleFor(r => r.EmployeeId, e.Id);

        e.Dependents = dependentsFaker.Generate(3);

        var ds = new List<Employee>() { e };

        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        var startDate = new DateTime(2023, 1, 1);

        var expected = new Paycheck()
        {
            Benefits = 1292.31m,
            Employee = e,
            PaycheckStartDate = startDate,
            PaycheckEndDate = startDate.AddDays(13),
            Salary = 2307.69m,
            YearlyBenefits = 33600.00m,
            YearlySalary = e.Salary
        };

        var result = await sut.GetPaycheckForEmployee(e.Id, 2023, 1);

        Assert.NotNull(result);
        Assert.Equal(JsonConvert.SerializeObject(expected, _serializerSettings), JsonConvert.SerializeObject(result, _serializerSettings));
    }

    [Fact]
    public async void WhenAskedForPaycheck_ShouldReturnPaycheck_OneDependentOldPartTime_LowSalary()
    {
        var employeeFaker = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 60000m)
            .RuleFor(r => r.Dependents, new List<Dependent>() { });

        var e = employeeFaker.Generate();

        var dependentsFaker = new Faker<Dependent>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.DateOfBirth, new DateTime(1973, 4, 17))
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.Relationship, Relationship.Spouse)
            .RuleFor(r => r.Employee, e)
            .RuleFor(r => r.EmployeeId, e.Id);

        e.Dependents = dependentsFaker.Generate(1);

        var ds = new List<Employee>() { e };

        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        var startDate = new DateTime(2023, 1, 1);

        var expected = new Paycheck()
        {
            Benefits = 803.59m,
            Employee = e,
            PaycheckStartDate = startDate,
            PaycheckEndDate = startDate.AddDays(13),
            Salary = 2307.69m,
            YearlyBenefits = 20893.33m,
            YearlySalary = e.Salary
        };

        var result = await sut.GetPaycheckForEmployee(e.Id, 2023, 1);

        Assert.NotNull(result);
        Assert.Equal(JsonConvert.SerializeObject(expected, _serializerSettings), JsonConvert.SerializeObject(result, _serializerSettings));
    }

    [Fact]
    public async void WhenAskedForPaycheck_ShouldReturnPaycheck_OneDependentYoungPartTime_HighSalary()
    {
        var employeeFaker = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 100000m)
            .RuleFor(r => r.Dependents, new List<Dependent>() { });

        var e = employeeFaker.Generate();

        var dependentsFaker = new Faker<Dependent>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.DateOfBirth, new DateTime(2023, 6, 14))
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.Relationship, Relationship.Spouse)
            .RuleFor(r => r.Employee, e)
            .RuleFor(r => r.EmployeeId, e.Id);

        e.Dependents = dependentsFaker.Generate(1);

        var ds = new List<Employee>() { e };

        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        var startDate = new DateTime(2023, 5, 7);

        var expected = new Paycheck()
        {
            Benefits = 690.00m,
            Employee = e,
            PaycheckStartDate = startDate,
            PaycheckEndDate = startDate.AddDays(13),
            Salary = 3846.15m,
            YearlyBenefits = 17940.00m,
            YearlySalary = e.Salary
        };

        var result = await sut.GetPaycheckForEmployee(e.Id, 2023, 10);

        Assert.NotNull(result);
        Assert.Equal(JsonConvert.SerializeObject(expected, _serializerSettings), JsonConvert.SerializeObject(result, _serializerSettings));
    }

    [Fact]
    public async void WhenAskedForPaycheckForNonExistenEmployee_ShoulThrowAnError()
    {
        var employee = new Faker<Employee>()
            .RuleFor(r => r.Id, 1)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.DateOfBirth, new DateTime(2000, 10, 15))
            .RuleFor(r => r.Salary, 75000m)
            .RuleFor(r => r.Dependents, new List<Dependent>());

        var ds = new List<Employee>() { employee.Generate() };

        IEmployeesRepository rep = new EmployeesRepository(ds);

        IPaycheckService sut = new PaycheckService(rep);

        await Assert.ThrowsAsync<NullReferenceException>(() => sut.GetPaycheckForEmployee(0, 2023, 1));
    }
    #endregion
}
