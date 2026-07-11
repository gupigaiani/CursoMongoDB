using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CursoMongoDB.Programs;
public static class Program_3_5
{
    public static void Executar()
    {
        var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("NoticiasDB");
        var collection = database.GetCollection<BsonDocument>("noticias");

        var Noticia = new NoticiaClass
        {
            Titulo = "Brasil bate Equador",
            Texto = "No Maracanã, Brasil vende o Equador por 2x0...",
            DataPublicacao = DateTime.Parse("2026-08-10T15:32:00Z"),
            Tags = new List<string> {"esporte", "brasil"},
            Jornalistas = new List<JornalistaClass>
            {
                new JornalistaClass { Nome = "Maria" }
            },
            Comentarios = new List<ComentarioClass>
            {
                new ComentarioClass 
                {
                    Comentario = "Grande Jogo",
                    Curtidas = 0, 
                    Usuario = "Carlos", 
                    Data = DateTime.Parse("2026-08-10T18:42:00Z")
                }
            },
            Anexos = new List<AnexoClass>
            {
                new AnexoClass
                {
                    NomeArquivo = "foto-jogo.jpg",
                    Url = "https://meusite.com.br/fotos/foto-jogo.jpg",
                    Tamanho = 204800,
                    Tipo = "imagem/jpg",
                    Cliques = 0
                }
            },
            Visualizacoes = 0,
            TotalComentarios = 1,
            Gostei = 0,
            NaoGostei = 0,
            TempoMedioLeitura = 0.0
        };

        var noticiaBson = Noticia.ToBson();
        var noticiaJson = Noticia.ToJson();
        var noticiaBsonJson = noticiaBson.ToJson();
        
        Console.WriteLine("\n== NOTÍCIA ==");

        Console.WriteLine("\n--- JSON via ToJson() ---");
        Console.WriteLine(noticiaJson);

        Console.WriteLine("\n--- BSON via ToBsonDocument().ToJson() ---");
        Console.WriteLine(noticiaBsonJson);
    }
}