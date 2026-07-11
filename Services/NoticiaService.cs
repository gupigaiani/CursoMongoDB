using MongoDB.Bson;
using MongoDB.Driver;
using CursoMongoDB.Contexts;
using CursoMongoDB.Models;

namespace CursoMongoDB.Services
{
    public class NoticiaService
    {
        private readonly IMongoCollection<BsonDocument> _colecao;

        // O construtor recebe o MongoContext (Injeção de Dependência)
        public NoticiaService(MongoContext contexto)
        {
            _colecao = contexto.Noticias;
        }

        // Método para inserir UMA notícia
        public async Task InserirNoticiaAsync(NoticiaClass noticia)
        {
            var bsonDoc = noticia.ToBsonDocument();
            await _colecao.InsertOneAsync(bsonDoc);
        }

        // Método para inserir VÁRIAS notícias
        public async Task InserirNoticiasAsync(IEnumerable<NoticiaClass> noticias)
        {
            var listaBson = new List<BsonDocument>();
            foreach (var noticia in noticias)
            {
                listaBson.Add(noticia.ToBsonDocument());
            }
            await _colecao.InsertManyAsync(listaBson);
        }
    }
}