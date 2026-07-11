using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using CursoMongoDB.Contexts;
using CursoMongoDB.Services;

namespace CursoMongoDB.Programs;

public static class Program_5_2a
{
    public static async Task ExecutarAsync()
    {
        var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";
        // var client = new MongoClient(connectionString);
        // var database = client.GetDatabase("NoticiasDB");
        // var collection = database.GetCollection<BsonDocument>("noticias");

        var contexto = new MongoContext(connectionString, "NoticiasDB");
        var NoticiaService = new NoticiaService(contexto);

        string jsonRecebido = @"
        {
            ""Titulo"": ""Brasil bate Equador"",
            ""Texto"": ""No Maracanã, Brasil vende o Equador por 2x0..."",
            ""DataPublicacao"": ""2026-08-10T12:32:00-03:00"",
            ""Tags"": [
                ""esporte"",
                ""brasil""
            ],
            ""Jornalistas"": [
                {
                ""Nome"": ""Maria""
                }
            ],
            ""Comentarios"": [
                {
                ""Comentario"": ""Grande Jogo"",
                ""Curtidas"": 0,
                ""Usuario"": ""Carlos"",
                ""Data"": ""2026-08-10T15:42:00-03:00""
                }
            ],
            ""Anexos"": [
                {
                ""NomeArquivo"": ""foto-jogo.jpg"",
                ""Url"": ""https://meusite.com.br/fotos/foto-jogo.jpg"",
                ""Tamanho"": 204800,
                ""Tipo"": ""imagem/jpg"",
                ""Cliques"": 0
                }
            ],
            ""Visualizacoes"": 0,
            ""TotalComentarios"": 1,
            ""Gostei"": 0,
            ""NaoGostei"": 0,
            ""TempoMedioLeitura"": 0.0
        }";

        var Noticia = JsonConvert.DeserializeObject<NoticiaClass>(jsonRecebido);

        // var noticiaBson = Noticia.ToBson();

        // try
        // {
        //     await collection.InsertOneAsync(noticiaBson);
        //     Console.WriteLine("Notícia incluída com sucesso!");
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine("Ocorreu um erro ao tentar inserir o BSON como string:");
        //     Console.WriteLine(ex.Message);
        // }

        try
        {
            await NoticiaService.InserirNoticiaAsync(Noticia);
            Console.WriteLine("Notícia incluída com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocorreu um erro ao tentar inserir o JSON como string:");
            Console.WriteLine(ex.Message);
        }
    }
}