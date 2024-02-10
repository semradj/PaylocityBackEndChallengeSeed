using Api.Dtos;
using Api.Dtos.Paycheck;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PaycheckController : ControllerBase
{
    private readonly IPaycheckService _paycheckService;

    public PaycheckController(IPaycheckService paycheckService)
    {
        _paycheckService = paycheckService ?? throw new ArgumentNullException(nameof(paycheckService));
    }

    [SwaggerOperation(Summary = "Get paycheck for employee id and year by paycheck number 1 to 26")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetPaycheckDto>>>> Get(int employeeId, int year, int paycheckNumber)
    {
        ApiResponse<GetPaycheckDto> result = new() { Success = false };

        try
        {
            var data = await _paycheckService.GetPaycheckForEmployee(employeeId, year, paycheckNumber);
            result = new()
            {
                Data = data.ToGetPaycheckDto(),
                Success = true
            };

            return Ok(result);
        }
        catch (NullReferenceException)
        {
            // question: ApiResponse Message and Error properties, what are the policies about providing details to the consumer
            // is it used internally, so it is ok to provide Error details or is it better to hide these values as they may contain stuff we don't want to expose?
            return NotFound(result);
        }
        catch
        {
            return BadRequest(result);
        }
    }

    [SwaggerOperation(Summary = "Get all paycheck for employee and year")]
    [HttpGet("All")]
    public async Task<ActionResult<ApiResponse<List<GetPaycheckDto>>>> Get(int employeeId, int year)
    {
        ApiResponse<List<GetPaycheckDto>> result = new() { Success = false };

        try
        {
            var data = await _paycheckService.GetPaychecksForEmployee(employeeId, year);
            result = new()
            {
                Data = data.Select(x => x.ToGetPaycheckDto()).ToList(),
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
}