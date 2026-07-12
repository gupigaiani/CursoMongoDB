using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using CursoMongoDB.Contexts;
using CursoMongoDB.Services;

namespace CursoMongoDB.Programs;

public static class Program_9_2
{
    public static async Task ExecutarAsync()
    {
        var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";

        var contexto = new MongoContext(connectionString, "NoticiasDB");
        var NoticiaService = new NoticiaService(contexto);

        DateTime dataInicio = new DateTime(2025, 01, 01);
        DateTime dataFim = new DateTime(2025, 12, 31);

        try
        {
            var noticiasPorDia = await NoticiaService.NoticiasPorDiaSemanaAsync(dataInicio, dataFim);
            Console.WriteLine("Notícias publicadas por dia da semana:");
            foreach (var dia in noticiasPorDia)
            {
                Console.WriteLine($"{dia.Key}: {dia.Value}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocorreu um erro obter as notícias por dia da semana:");
            Console.WriteLine(ex.Message);
        }
    }
}