using CursoMongoDB.Models;

namespace CursoMongoDB.Programs;
public static class Program_2_4
{
    public static void Executar()
    {
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
    }
}