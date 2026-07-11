using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using CursoMongoDB.Contexts;
using CursoMongoDB.Services;

namespace CursoMongoDB.Programs;

public static class Program_5_3a
{
    public static async Task ExecutarAsync()
    {
        var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";
        // var client = new MongoClient(connectionString);
        // var database = client.GetDatabase("NoticiasDB");
        // var collection = database.GetCollection<BsonDocument>("noticias");

        var contexto = new MongoContext(connectionString, "NoticiasDB");
        var NoticiaService = new NoticiaService(contexto);

        string jsonRecebido = @"{
            ""Titulo"": ""Festival de Música agita a cidade do Rio de Janeiro"",
            ""Texto"": ""O festival reuniu artistas nacionais e internacionais..."",
            ""DataPublicacao"": ""2024-11-22T20:30:00Z"",
            ""Tags"": [""cultura"", ""musica"", ""brasil""],
            ""Jornalistas"": [
                { ""Nome"": ""João Almeida"" }
            ],
            ""Comentarios"": [
                {
                    ""Comentario"": ""Evento sensacional!"",
                    ""Curtidas"": 19,
                    ""Usuario"": ""LarissaSantos"",
                    ""Data"": ""2024-11-22T22:10:00Z""
                }
            ],
            ""Anexos"": [
                {
                    ""NomeArquivo"": ""festival-musica.jpg"",
                    ""Url"": ""https://meusite.com/img/festival-musica.jpg"",
                    ""Tamanho"": 98000,
                    ""Tipo"": ""image/jpeg"",
                    ""Cliques"": 31
                }
            ],
            ""Visualizacoes"": 950,
            ""TotalComentarios"": 1,
            ""Gostei"": 76,
            ""NaoGostei"": 2,
            ""TempoMedioLeitura"": 3.1
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