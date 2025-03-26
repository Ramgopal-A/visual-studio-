using MongoDB.Driver;
using MongoDB.Bson;

public class MongoDbService
{
    private readonly IMongoCollection<User> _usersCollection;
    private readonly IMongoCollection<Patient> _patientsCollection;

    public MongoDbService(IMongoDatabase database)
    {
        _usersCollection = database.GetCollection<User>("user");
        _patientsCollection = database.GetCollection<Patient>("patient");

    }

    public async Task AddUserAsync(User model)
    {
        await _usersCollection.InsertOneAsync(model);
    }

    public async Task AddPatientAsync(Patient model)
    {
        await _patientsCollection.InsertOneAsync(model);
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _usersCollection.Find(FilterDefinition<User>.Empty).ToListAsync();

    }

    public async Task<List<Patient>> GetPatientsAsync()
    {
        return await _patientsCollection.Find(FilterDefinition<Patient>.Empty).ToListAsync();

    }

    public async Task<bool> UpdateExitTimeAsync(string visitId, string exitTime)
    {
        if (string.IsNullOrEmpty(exitTime))
        {
            Console.WriteLine("Error: Exit time is empty.");
            return false;
        }

        var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(visitId));
        var update = Builders<User>.Update.Set(p => p.ExitTime, exitTime);


        var result = await _usersCollection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<List<User>> GetPendingVisitsAsync(string fullName)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(u => u.FullName, fullName),
            Builders<User>.Filter.Or(
                Builders<User>.Filter.Eq(u => u.ExitTime, null),
                Builders<User>.Filter.Eq(u => u.ExitTime, "")
            )
        );

        return await _usersCollection.Find(filter).ToListAsync();
    }



}
