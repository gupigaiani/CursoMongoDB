using CursoMongoDB.Models;
using MongoDB.Driver;

namespace CursoMongoDB.Programs;
public static class Program_3_1b
{
    public static void Executar()
    {
        var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("NoticiasDB");
        client.DropDatabase("NoticiasDB");
        Console.WriteLine("Banco de dados 'NoticiasDB' removido com sucesso!");
    }
}