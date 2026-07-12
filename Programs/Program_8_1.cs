using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using CursoMongoDB.Contexts;
using CursoMongoDB.Services;

namespace CursoMongoDB.Programs;

public static class Program_8_1
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
            var (totalGostei, totalNaoGostei) = await NoticiaService.ObterReacoesNoPeriodoAsync(dataInicio, dataFim);
            Console.WriteLine($"Total de 'Gostei': {totalGostei}");
            Console.WriteLine($"Total de 'Não Gostei': {totalNaoGostei}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocorreu um erro obter os valores de Gostei e Não Gostei:");
            Console.WriteLine(ex.Message);
        }
    }
}