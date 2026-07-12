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
    public static class Program_10_3
    {
        public static async Task ExecutarAsync()
        {
            var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";
            var contexto = new MongoContext(connectionString, "NoticiasDB");
            var NoticiaService = new NoticiaService(contexto);

            Console.WriteLine("--- Garantindo a existência dos índices otimizados... ---");
            // Primeiro, garantimos que todos os índices necessários para as consultas abaixo existam.
            await NoticiaService.CriarIndicesListagensOrdenadasAsync();
            Console.WriteLine("-------------------------------------------------------\n");

            // a) Testando busca por tag, ordenado por data
            var noticiasPorTag = await NoticiaService.ListarNoticiasPorTagRecentes("tecnologia", new DateTime(2025, 1, 1), new DateTime(2025, 12, 31));
            ImprimirResultados("a) Notícias Recentes da Tag 'tecnologia'", noticiasPorTag, "DataPublicacao");

            // b) Testando ranking por visualizações
            var rankingViews = await NoticiaService.ListarNoticiasPorRelevancia(new DateTime(2025, 1, 1), new DateTime(2025, 12, 31));
            ImprimirResultados("b) Ranking por Visualizações no Período", rankingViews, "Visualizacoes");

            // c) Testando busca por palavra-chave
            var buscaTexto = await NoticiaService.BuscarNoticiasPorPalavraChave("investimento");
            ImprimirResultados("c) Busca por Palavra-Chave 'investimento'", buscaTexto, "DataPublicacao");

            // d) Testando busca por jornalista
            var noticiasJornalista = await NoticiaService.ListarNoticiasPorJornalista("Paula Castro");
            ImprimirResultados("d) Notícias Recentes da Jornalista 'Paula Castro'", noticiasJornalista, "DataPublicacao");

            // e) Testando busca por anexos populares
            var anexosPopulares = await NoticiaService.ListarNoticiasComAnexosPopulares("application/pdf");
            ImprimirResultados("e) Notícias com Anexos 'pdf' Mais Populares", anexosPopulares, "Anexos.Cliques");
        }
        private static void ImprimirResultados(string titulo, List<BsonDocument> documentos, string campoOrdenado)
        {
            Console.WriteLine($"--- {titulo} ---");
            if (documentos == null || documentos.Count == 0)
            {
                Console.WriteLine("Nenhum documento encontrado.");
                Console.WriteLine("\n-------------------------------------------------------\n");
                return;
            }

            foreach (var doc in documentos)
            {
                var valorOrdenado = "N/A";
                try
                {
                    // Lógica para extrair o valor do campo ordenado, mesmo que seja aninhado ou não exista
                    if (doc.Contains(campoOrdenado) && !doc[campoOrdenado].IsBsonNull)
                    {
                        valorOrdenado = doc[campoOrdenado].ToString();
                    }
                    else if (campoOrdenado.Contains(".")) // Para campos aninhados como "Anexos.Cliques"
                    {
                        var partes = campoOrdenado.Split('.');
                        if (doc.Contains(partes[0]) && doc[partes[0]].IsBsonArray)
                        {
                            // Pega o valor do primeiro anexo que corresponda (simplificação para exibição)
                            valorOrdenado = doc[partes[0]].AsBsonArray.FirstOrDefault()?[partes[1]].ToString() ?? "N/A";
                        }
                    }
                }
                catch { /* Ignora erros se o campo não existir no documento específico */ }

                Console.WriteLine($"  - Título: {doc["Titulo"]}, ({campoOrdenado}: {valorOrdenado})");
            }
            Console.WriteLine($"Total de resultados: {documentos.Count}");
            Console.WriteLine("\n-------------------------------------------------------\n");
        }
    }
}