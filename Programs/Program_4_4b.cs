using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace CursoMongoDB.Programs;

public static class Program_4_4b
{
    public static async Task ExecutarAsync()
    {
        Console.WriteLine("Processo iniciado (ASSÍNCRONO), esperando por 10 segundos...");
        await Task.Delay(10000);
        Console.WriteLine("Processo assíncrono finalizado.");
    }
}