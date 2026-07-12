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
    public static class Program_10_1
    {
        public static async Task ExecutarAsync()
        {

            var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";
            var contexto = new MongoContext(connectionString, "NoticiasDB");
            var NoticiaService = new NoticiaService(contexto);

            try
            {
                await NoticiaService.CriarIndiceDataPublicacaoAsync();
                Console.WriteLine("--- Operação de criação de índice finalizada ---");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro ao criao o indice: {ex.Message}");
            }
        }
    }
}