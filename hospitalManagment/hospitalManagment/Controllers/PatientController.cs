using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/patient")]
public class PatientController : ControllerBase
{
    private readonly MongoDbService _mongoDbService;

    public PatientController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddPatient([FromBody] Patient Patient)
    {
        if (Patient == null)
        {
            return BadRequest(new { message = "Invalid patient data" });
        }
        await _mongoDbService.AddPatientAsync(Patient);
        return Ok(new { message = "Patient added successfully" });
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetPatients()
    {
        try
        {

            var patients = await _mongoDbService.GetPatientsAsync();
        
            return Ok(patients); 
            
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

}
