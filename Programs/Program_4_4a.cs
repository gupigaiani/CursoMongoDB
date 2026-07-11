using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace CursoMongoDB.Programs;

public static class Program_4_4a
{
    public static void Executar()
    {
        Console.WriteLine("Processo iniciado (SÍNCRONO), esperando por 10 segundos...");
        Thread.Sleep(10000);
        Console.WriteLine("Processo síncrono finalizado.");
    }
}