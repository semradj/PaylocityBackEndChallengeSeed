using Api.Dtos;
using Api.Dtos.Dependent;
using Api.Models;
using Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DependentsController : ControllerBase
{
    private readonly IDependentsRepository _dependentsRepository;

    public DependentsController(IDependentsRepository dependentsRepository)
    {
        _dependentsRepository = dependentsRepository ?? throw new ArgumentNullException(nameof(dependentsRepository));
    }

    [SwaggerOperation(Summary = "Get dependent by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetDependentDto>>> Get(int id)
    {
        ApiResponse<GetDependentDto> result = new() { Success = false };

        try
        {
            var data = await _dependentsRepository.GetDependentById(id);
            result = new()
            {
                Data = data.ToGetDependentDto(),
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

    [SwaggerOperation(Summary = "Get all dependents")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> GetAll()
    {
        ApiResponse<List<GetDependentDto>> result = new() { Success = false };

        try
        {
            var data = await _dependentsRepository.GetAllDependents();
            result = new()
            {
                Data = data.Select(x => x.ToGetDependentDto()).ToList(),
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
