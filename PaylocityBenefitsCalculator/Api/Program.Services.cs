using Api.Models;
using Api.Repository;
using Api.Services;

public partial class Program
{
    private static void RegisterRepositories(WebApplicationBuilder builder)
    {
        List<Employee> _employees = new()
        {
            new()
            {
                Id = 1,
                FirstName = "LeBron",
                LastName = "James",
                Salary = 75420.99m,
                DateOfBirth = new DateTime(1984, 12, 30)
            },
            new()
            {
                Id = 2,
                FirstName = "Ja",
                LastName = "Morant",
                Salary = 92365.22m,
                DateOfBirth = new DateTime(1999, 8, 10),
                Dependents = new List<Dependent>
                {
                    new()
                    {
                        Id = 1,
                        FirstName = "Spouse",
                        LastName = "Morant",
                        Relationship = Relationship.Spouse,
                        DateOfBirth = new DateTime(1998, 3, 3)
                    },
                    new()
                    {
                        Id = 2,
                        FirstName = "Child1",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2020, 6, 23)
                    },
                    new()
                    {
                        Id = 3,
                        FirstName = "Child2",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2021, 5, 18)
                    }
                }
            },
            new()
            {
                Id = 3,
                FirstName = "Michael",
                LastName = "Jordan",
                Salary = 143211.12m,
                DateOfBirth = new DateTime(1963, 2, 17),
                Dependents = new List<Dependent>
                {
                    new()
                    {
                        Id = 4,
                        FirstName = "DP",
                        LastName = "Jordan",
                        Relationship = Relationship.DomesticPartner,
                        DateOfBirth = new DateTime(1974, 1, 2)
                    }
                }
            },
            //new()
            //{
            //    Id = 4,
            //    FirstName = "a",
            //    LastName = "b",
            //    Salary = 80000m,
            //    DateOfBirth = new DateTime(1963, 2, 17),
            //    Dependents = new List<Dependent>
            //    {
            //        new()
            //        {
            //            Id = 5,
            //            FirstName = "c",
            //            LastName = "b",
            //            Relationship = Relationship.DomesticPartner,
            //            DateOfBirth = new DateTime(1973, 6, 15)
            //        }
            //    }
            //}
        };

        // use singleton for our repositories to keep same data source for all requests
        // for real database configure repositories connections to be pooled
        // if api requests were used for data retrieval, configure http client factory, and retry policies
        builder.Services.AddSingleton<IEmployeesRepository>(x => new EmployeesRepository(_employees));
        builder.Services.AddSingleton<IDependentsRepository>(x => new DependentsRepository(_employees.SelectMany(x => x.Dependents).ToList()));
    }

    private static void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IPaycheckService, PaycheckService>();
    }
}