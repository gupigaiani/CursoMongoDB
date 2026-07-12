using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using CursoMongoDB.Contexts;
using CursoMongoDB.Services;

namespace CursoMongoDB.Programs;

public static class Program_9_3
{
    public static async Task ExecutarAsync()
    {
        var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";

        var contexto = new MongoContext(connectionString, "NoticiasDB");
        var NoticiaService = new NoticiaService(contexto);

        DateTime dataInicio = new DateTime(2024, 01, 01);
        DateTime dataFim = new DateTime(2025, 12, 31);

        try
        {
            var contagem = await NoticiaService.ContarNoticiasPorMesAnoAsync(dataInicio, dataFim);

            if (!contagem.Any())
            {
                Console.WriteLine("Nenhuma notícia encontrada no período especificado.");
            }
            else
            {
                Console.WriteLine("Resultado (ordenado por Ano/Mês):");
                foreach (var item in contagem)
                {
                    Console.WriteLine($"  {item.Key}: {item.Value} notícias");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro ao obter a contagem de notícias: {ex.Message}");
        }

    }
}