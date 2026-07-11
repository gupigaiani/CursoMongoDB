using MongoDB.Bson;
using MongoDB.Driver;

namespace CursoMongoDB.Contexts;

public class MongoContext
{
    private readonly IMongoDatabase _database;

    public IMongoDatabase Database => _database;

    // Construtor recebe a connection string e o nome do banco
    public MongoContext(string connectionString, string dbName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(dbName);
    }

    // Expondo a coleção de notícias já pronta para uso
    public IMongoCollection<BsonDocument> Noticias =>
        _database.GetCollection<BsonDocument>("noticias");
}