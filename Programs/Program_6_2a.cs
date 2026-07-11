using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using CursoMongoDB.Contexts;
using CursoMongoDB.Services;

namespace CursoMongoDB.Programs;

public static class Program_6_2a
{
    public static async Task ExecutarAsync()
    {
        var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";

        var contexto = new MongoContext(connectionString, "NoticiasDB");
        var NoticiaService = new NoticiaService(contexto);

        string jsonRecebido = @"{
            ""Titulo"": ""Arqueólogos Encontram Templo Perdido de Civilização Pré-Inca"",
            ""Texto"": ""Durante escavações na região andina do Peru, pesquisadores encontraram vestígios de um templo cerimonial que antecede o Império Inca, com artefatos preservados e inscrições desconhecidas."",
            ""DataPublicacao"": ""2025-07-10T14:20:00Z"",
            ""Tags"": [""arqueologia"", ""américa do sul"", ""história antiga""],
            ""Jornalistas"": [
                { ""Nome"": ""Sofia Ramires"" }
            ],
            ""Comentarios"": [
                {
                    ""Comentario"": ""Incrível avanço para o entendimento das culturas pré-colombianas!"",
                    ""Curtidas"": 31,
                    ""Usuario"": ""ExploradorAndino"",
                    ""Data"": ""2025-07-10T16:10:00Z""
                }
            ],
            ""Anexos"": [
                {
                    ""NomeArquivo"": ""templo-perdido.jpg"",
                    ""Url"": ""https://meusite.com/img/templo-perdido.jpg"",
                    ""Tamanho"": 102400,
                    ""Tipo"": ""image/jpeg"",
                    ""Cliques"": 48
                }
            ],
            ""Visualizacoes"": 880,
            ""TotalComentarios"": 1,
            ""Gostei"": 289,
            ""NaoGostei"": 1,
            ""TempoMedioLeitura"": 3.8
        }";

        var Noticia = JsonConvert.DeserializeObject<NoticiaClass>(jsonRecebido);

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