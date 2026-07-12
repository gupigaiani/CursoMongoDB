using System;
using System.Collections.Generic;
using MongoDB.Bson;
using CursoMongoDB.Contexts;
using CursoMongoDB.Services;

namespace CursoMongoDB.Programs
{
    public static class Program_10_5
    {
        public static async Task ExecutarAsync()
        {
            var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";

            var contexto = new MongoContext(connectionString, "NoticiasDB");
            var noticiaService = new NoticiaService(contexto);

            Console.WriteLine("--- 5 Notícias Mais Lidas da Tag 'brasil' ---\n");

            var noticias = await noticiaService.ListarNoticiasMaisLidasPorTagAsync("brasil");

            ImprimirResultados(noticias);
        }

        private static void ImprimirResultados(List<BsonDocument> documentos)
        {
            if (documentos == null || documentos.Count == 0)
            {
                Console.WriteLine("Nenhuma notícia encontrada.");
                return;
            }

            foreach (var doc in documentos)
            {
                string titulo = doc.GetValue("Titulo", "Sem título").AsString;
                int visualizacoes = doc.GetValue("Visualizacoes", 0).AsInt32;

                Console.WriteLine($"Título: {titulo}");
                Console.WriteLine($"Visualizações: {visualizacoes}");
                Console.WriteLine(new string('-', 50));
            }

            Console.WriteLine($"Total de notícias encontradas: {documentos.Count}");
        }
    }
}