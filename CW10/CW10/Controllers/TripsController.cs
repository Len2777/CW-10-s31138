using CW10.DTOs;
using CW10.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW10.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController(IDbService dbService) : ControllerBase
{
   

    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await dbService.GetTrips(page, pageSize));
    }


    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AssignClient(int idTrip, [FromBody] AssignClientDto dto)
    {
        return await dbService.AssignClientToTrip(idTrip, dto);
    }
    
}