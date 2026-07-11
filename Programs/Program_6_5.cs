using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using CursoMongoDB.Contexts;
using CursoMongoDB.Services;

namespace CursoMongoDB.Programs;

public static class Program_6_5
{
    public static async Task ExecutarAsync()
    {
        var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";

        var contexto = new MongoContext(connectionString, "NoticiasDB");
        var NoticiaService = new NoticiaService(contexto);

        try
        {
            await NoticiaService.RemoverNoticiasComUrlDuplicadaAsync();
            Console.WriteLine("URLs repetidas removidas com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocorreu um erro ao tentar remover as URLs repetidas:");
            Console.WriteLine(ex.Message);
        }
    }
}