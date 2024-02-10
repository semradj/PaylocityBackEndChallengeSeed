using Api.Dtos;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Api.Repository;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeesRepository _employeesRepository;

    public EmployeesController(IEmployeesRepository employeesRepository)
    {
        _employeesRepository = employeesRepository ?? throw new ArgumentNullException(nameof(employeesRepository));
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
    {
        ApiResponse<GetEmployeeDto> result = new() { Success = false };

        try
        {
            var data = await _employeesRepository.GetEmployeeById(id);
            result = new()
            {
                Data = data.ToGetEmployeeDto(),
                Success = true
            };

            return Ok(result);
        }
        catch (NullReferenceException)
        {
            return NotFound(result);
        }
        catch
        {
            return BadRequest(result);
        }
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
    {
        ApiResponse<List<GetEmployeeDto>> result = new() { Success = false };

        try
        {
            var data = await _employeesRepository.GetAllEmployees();

            result = new()
            {
                Data = data.Select(x => x.ToGetEmployeeDto()).ToList(),
                Success = true
            };

            return Ok(result);
        }
        catch
        {
            return BadRequest(result);
        }
    }
}
