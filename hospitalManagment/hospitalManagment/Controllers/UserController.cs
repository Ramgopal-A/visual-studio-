using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;



[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly MongoDbService _mongoDbService;


    public UserController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddUser([FromBody] User User)
    {
        try
        {
            if (User == null)
                return BadRequest(new { message = "Invalid patient data." });

            await _mongoDbService.AddUserAsync(User);
            return Ok(new { message = "USer added successfully." });

        }
        catch (Exception ex)
        {

            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _mongoDbService.GetUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    // ✅ Update Exit Time API
    [HttpPut("updateExitTime")]
    public async Task<IActionResult> UpdateExitTime([FromBody] ExitTimeUpdateModel model)
    {
        try
        {
            if (model == null || string.IsNullOrEmpty(model.VisitId) || string.IsNullOrEmpty(model.ExitTime))
            {
                return BadRequest(new { message = "Invalid request data." });
            }
            await _mongoDbService.UpdateExitTimeAsync(model.VisitId, model.ExitTime);
            return Ok(new { message = "Exit time updated successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }

    [HttpGet("getPending")]
    public async Task<IActionResult> GetPendingVisits([FromQuery] string fullName)
    {
        try
        {
            if (string.IsNullOrEmpty(fullName))
                return BadRequest(new { message = "User name is required." });

            var pendingVisits = await _mongoDbService.GetPendingVisitsAsync(fullName);
            return Ok(pendingVisits);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    

    }















