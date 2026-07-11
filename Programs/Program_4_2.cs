using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace CursoMongoDB.Programs;

public static class Program_4_2
{
    public static void Executar()
    {
        var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("NoticiasDB");
        var collection = database.GetCollection<BsonDocument>("noticias");

        string caminhoArquivo = "C:\\temp\\4905 - Vídeo 4.2 - Lista.js";
        if (!File.Exists(caminhoArquivo))
        {
            Console.WriteLine("Arquivo noticias.json não encontrado!");
            return;
        }

        string conteudoJson = File.ReadAllText(caminhoArquivo);
        var listaNoticias = JsonConvert.DeserializeObject<List<NoticiaClass>>(conteudoJson);
        int contador = 1;
        foreach (var Noticia in listaNoticias)
        {
            var noticiaBson = Noticia.ToBson();
            try
            {
                collection.InsertOne(noticiaBson);
                Console.WriteLine("Notícia incluída com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao tentar inserir o BSON como string:");
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine($"Notícia {contador} inserida no MongoDB: {Noticia.Titulo}");
            contador++;
        }
    }
}