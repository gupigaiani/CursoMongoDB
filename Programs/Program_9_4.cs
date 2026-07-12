using System;
using System.Collections.Generic;
using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using CursoMongoDB.Contexts;
using CursoMongoDB.Services;

namespace CursoMongoDB.Programs
{
    public static class Program_9_4
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
                var dashboard = await NoticiaService.GerarDashboardAnaliseTagsAsync(dataInicio, dataFim);
                if (dashboard == null || !dashboard.Elements.Any())
                {
                    Console.WriteLine("\nNenhuma notícia encontrada no período para gerar o dashboard.");
                    return;
                }

                ImprimirResultadosFacet("--- Top 5 Tags Mais Utilizadas no Período ---", dashboard["TopTagsPorUso"].AsBsonArray);
                ImprimirResultadosFacet("--- Top 5 Tags com Maior Média de 'Gostei' no Período ---", dashboard["TopTagsPorGostei"].AsBsonArray);
                ImprimirResultadosFacet("--- Top 5 Tags com Maior Média de Comentários no Período ---", dashboard["TopTagsPorComentarios"].AsBsonArray);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro ao gerar o dashboard: {ex.Message}");
            }
        }

        private static void ImprimirResultadosFacet(string titulo, BsonArray resultados)
        {
            Console.WriteLine($"\n{titulo}");
            if (!resultados.Any()) { /* ... */ } // Código de impressão já definido

            foreach (var item in resultados.Select(r => r.AsBsonDocument))
            {
                Console.Write($"Tag: {item["Tag"],-20}");
                foreach (var elemento in item.Elements)
                {
                    if (elemento.Name != "Tag")
                    {
                        string valorFormatado = elemento.Value.IsDouble ? $"{elemento.Value.AsDouble:F2}" : elemento.Value.ToString();
                        Console.Write($" | {elemento.Name}: {valorFormatado}");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}