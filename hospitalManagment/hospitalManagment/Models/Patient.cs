using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Patient
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    

    [BsonElement("fullName")]
    public string FullName { get; set; }

    [BsonElement("gender")]
    public string Gender { get; set; }

    [BsonElement("bloodGroup")]
    public string BloodGroup { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }

    [BsonElement("phoneNumber")]
    public string PhoneNumber { get; set; }

    [BsonElement("houseFlatNo")]
    public string HouseFlatNo { get; set; }

    [BsonElement("city")]
    public string City { get; set; }

    [BsonElement("state")]
    public string State { get; set; }

    [BsonElement("pincode")]
    public string Pincode { get; set; }

    [BsonElement("district")]
    public string District { get; set; }

    [BsonElement("password")]
    public string Password { get; set; }

    public List<LogEntry> Logs { get; set; } = new List<LogEntry>();
}
public class LogEntry
{
    public string Email { get; set; }

    [BsonElement("floor")]
    public string Floor { get; set; }

    [BsonElement("room")]
    public string Room { get; set; }

    [BsonElement("reason")]
    public string Reason { get; set; }

    [BsonElement("entryTime")]
    public string? EntryTime { get; set; }  // Nullable now

    [BsonElement("exitTime")]
    public string? ExitTime { get; set; }  // Nullable now

    [BsonElement("actionType")]
    public string ActionType { get; set; } // "Entry" or "Exit"
}
public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}