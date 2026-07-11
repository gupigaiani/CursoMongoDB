using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using CursoMongoDB.Contexts;
using CursoMongoDB.Services;

namespace CursoMongoDB.Programs;

public static class Program_5_2b
{
    public static async Task ExecutarAsync()
    {
        var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";
        // var client = new MongoClient(connectionString);
        // var database = client.GetDatabase("NoticiasDB");
        // var collection = database.GetCollection<BsonDocument>("noticias");

        var contexto = new MongoContext(connectionString, "NoticiasDB");
        var NoticiaService = new NoticiaService(contexto);

        string caminhoArquivo = "C:\\temp\\4905 - Vídeo 5.2 - Lista.js";
        if (!File.Exists(caminhoArquivo))
        {
            Console.WriteLine("Arquivo noticias.json não encontrado!");
            return;
        }

        string conteudoJson = File.ReadAllText(caminhoArquivo);
        var listaNoticias = JsonConvert.DeserializeObject<List<NoticiaClass>>(conteudoJson);

        // var listaBson = new List<BsonDocument>();
        // foreach (var Noticia in listaNoticias)
        // {
        //     listaBson.Add(Noticia.ToBson());
        // }

        // try
        // {
        //     await collection.InsertManyAsync(listaBson);
        //     Console.WriteLine("Notícias incluídas com sucesso!");
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine("Ocorreu um erro ao tentar inserir a lista de notícias:");
        //     Console.WriteLine(ex.Message);
        // }

        try
        {
            await NoticiaService.InserirNoticiasAsync(listaNoticias);
            Console.WriteLine("Notícias incluída com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocorreu um erro ao tentar inserir a lista de notícias:");
            Console.WriteLine(ex.Message);
        }
    }
}