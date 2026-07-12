using CursoMongoDB.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using CursoMongoDB.Contexts;
using CursoMongoDB.Services;

namespace CursoMongoDB.Programs;

public static class Program_7_1
{
    public static async Task ExecutarAsync()
    {
        var connectionString = "mongodb+srv://root:root@cluster0.s37qu9z.mongodb.net/?appName=Cluster0";

        var contexto = new MongoContext(connectionString, "NoticiasDB");
        var NoticiaService = new NoticiaService(contexto);

        var Url = "arqueologos_encontram_templo_perdido_de_civilizacao_preinca";
        var NovoTexto = "Pesquisadores comprovam que o templo cerimonial foi criado realmente na época Inca.";

        try
        {
            await NoticiaService.AtualizarTextoPorUrlAsync(Url, NovoTexto);
            Console.WriteLine("Texto da Notícia alterada com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocorreu um erro ao tentar alterar o texto da Notícia:");
            Console.WriteLine(ex.Message);
        }
    }
}