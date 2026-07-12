using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using CursoMongoDB.Contexts;
using CursoMongoDB.Services;

namespace CursoMongoDB.Programs;

public static class Program_8_3
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
            var (maisAprovada, maisRejeitada) = await NoticiaService.ObterNoticiasMaisRelevantesAsync(dataInicio, dataFim);

            Console.WriteLine("Notícia com mais GOSTEI:");
            Console.WriteLine(maisAprovada ?? "Nenhuma encontrada.");

            Console.WriteLine("\nNotícia com mais NÃO GOSTEI:");
            Console.WriteLine(maisRejeitada ?? "Nenhuma encontrada.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao obter as notícias mais relevantes:");
            Console.WriteLine(ex.Message);
        }
    }
}