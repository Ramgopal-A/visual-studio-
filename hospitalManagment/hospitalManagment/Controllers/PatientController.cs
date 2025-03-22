using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson.IO;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;



[Route("api/user")]
[ApiController]
public class PatientController : ControllerBase
{
    private readonly MongoDbService _mongoDbService;
    private readonly IConfiguration _configuration;

    public PatientController(MongoDbService mongoDbService, IConfiguration configuration)
    {
        _mongoDbService = mongoDbService;
        _configuration = configuration;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddPatient([FromBody] Patient patient)
    {
        try
        {
            if (patient == null)
                return BadRequest(new { message = "Invalid patient data." });

            await _mongoDbService.AddPatientAsync(patient);

            // Send password email
            bool emailSent = SendEmail(patient.Email, patient.Password);
            if (!emailSent)
            {
                return StatusCode(500, new { message = "User registered, but failed to send email." });
            }

            return Ok(new { message = "Password sent to your email." });
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error in AddPatient: " + ex.Message);
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }

    private bool SendEmail(string toEmail, string password)
    {
        try
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];

            var client = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, "No Reply"),
                Subject = "Your Registration Password",
                Body = $"Dear User,\n\nYour account has been registered successfully.\n\nYour password: {password}\n\nPlease do not reply to this email.\n\nBest regards,\nHospital Management System",
                IsBodyHtml = false
            };

            mailMessage.To.Add(toEmail);
            client.Send(mailMessage);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error sending email: " + ex.Message);
            return false;
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        try
        {
            if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new { message = "Email and Password are required." });
            }

            var patient = await _mongoDbService.GetPatientByEmailAsync(loginRequest.Email);

            if (patient == null || patient.Password != loginRequest.Password)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(new { /*message = "Login successful!"*/
                email = patient.Email,
                fullName = patient.FullName,
                gender = patient.Gender,
                bloodGroup = patient.BloodGroup,
                phoneNumber = patient.PhoneNumber,
                houseFlatNo = patient.HouseFlatNo,
                city = patient.City,
                state = patient.State,
                district = patient.District,
                pincode = patient.Pincode
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in Login: " + ex.Message);
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }

    /*[HttpPost("addLog")]
    public async Task<IActionResult> AddLog([FromBody] LogEntry logEntry)
    {
        try
        {
            var patient = await _mongoDbService.GetPatientByEmailAsync(logEntry.Email);
            if (patient == null)
                return NotFound(new { message = "User not found" });

            if (logEntry.ActionType == "Entry")
            {
                logEntry.EntryTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                logEntry.ExitTime = "--"; // Placeholder for exit time
            }
            else if (logEntry.ActionType == "Exit")
            {
                var lastLog = patient.Logs.FindLast(l => l.ExitTime == "--");
                if (lastLog != null)
                    lastLog.ExitTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            await _mongoDbService.AddLogEntryAsync(logEntry.Email, logEntry);
            return Ok(new { message = "Log updated successfully!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in AddLog: " + ex.Message);
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }*/

    [HttpPost("addLog")]
    public async Task<IActionResult> AddLog([FromBody] LogEntry logEntry)
    {
        
        try
        {
            if (logEntry == null || string.IsNullOrEmpty(logEntry.Email))
            {
                Console.WriteLine("Invalid log data received.");
                return BadRequest(new { message = "Invalid log data" });
            }

            Console.WriteLine($"Received log entry for email: {logEntry.Email}");

            var patient = await _mongoDbService.GetPatientByEmailAsync(logEntry.Email);
            if (patient == null)
            {
                return NotFound(new { message = "User not found" });
            }

            await _mongoDbService.AddLogEntryAsync(logEntry.Email, logEntry);
            return Ok(new { message = "Log updated successfully!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }


    [HttpGet("getLogs")]
    public async Task<IActionResult> GetLogsByEmail(string email)
    {
        try
        {
            var logs = await _mongoDbService.GetLogsByEmailAsync(email);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in GetLogs: " + ex.Message);
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }


   /* public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }*/
}

