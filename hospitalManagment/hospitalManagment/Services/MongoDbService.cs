using MongoDB.Driver;

public class MongoDbService
{
    private readonly IMongoCollection<Patient> _patientsCollection;

    public MongoDbService(IMongoDatabase database)
    {
        _patientsCollection = database.GetCollection<Patient>("user");
    }

    public async Task AddPatientAsync(Patient patient)
    {
        await _patientsCollection.InsertOneAsync(patient);
    }
    //To verify the email id while login
    public async Task<Patient?> GetPatientByEmailAsync(string email)
    {
        return await _patientsCollection.Find(p => p.Email == email).FirstOrDefaultAsync();
    }
     public async Task UpdatePatientAsync(Patient patient)
    {
        var filter = Builders<Patient>.Filter.Eq(p => p.Email, patient.Email);
        await _patientsCollection.ReplaceOneAsync(filter, patient);
    }

    public async Task AddLogEntryAsync(string email, LogEntry logEntry)
    {
        var filter = Builders<Patient>.Filter.Eq(p => p.Email, email);
        var update = Builders<Patient>.Update.Push(p => p.Logs, logEntry);
        await _patientsCollection.UpdateOneAsync(filter, update);
    }

    public async Task<List<LogEntry>> GetLogsByEmailAsync(string email)
    {
        var patient = await _patientsCollection.Find(p => p.Email == email).FirstOrDefaultAsync();
        return patient?.Logs ?? new List<LogEntry>();
    }
}
