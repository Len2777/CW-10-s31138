using CW10.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW10.Controllers;

 
[ApiController]
[Route("api/clients")]
public class ClientsController(IDbService dbService) : ControllerBase
{

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {   
        
        return await dbService.DeleteClient(idClient);
    }
}