using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using CursoMongoDB.Contexts;
using CursoMongoDB.Services;

namespace CursoMongoDB.Programs;

public static class Program_9_1
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
            var (total, mediaGostei, maxVis, minVis, aprovRel) = await NoticiaService.ObterEstatisticasNoticiasAsync(dataInicio, dataFim);
            Console.WriteLine($"Total de visualizações: {total}");
            Console.WriteLine($"Média de 'gostei' por notícia: {mediaGostei:N2}");
            Console.WriteLine($"Máximo de visualizações: {maxVis}");
            Console.WriteLine($"Mínimo de visualizações: {minVis}");
            Console.WriteLine($"Média da aprovação relativa: {aprovRel:P2}"); // Exibe como porcentagem
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocorreu um erro obter estatísticas das notícias:");
            Console.WriteLine(ex.Message);
        }
    }
}