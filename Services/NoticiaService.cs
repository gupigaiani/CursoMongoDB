using MongoDB.Bson;
using MongoDB.Driver;
using CursoMongoDB.Contexts;
using CursoMongoDB.Models;
using MongoDB.Bson.Serialization;

namespace CursoMongoDB.Services
{
    public class NoticiaService
    {
        private readonly IMongoCollection<BsonDocument> _colecao;
        private readonly MongoContext _contexto;

        // O construtor recebe o MongoContext (Injeção de Dependência)
        public NoticiaService(MongoContext contexto)
        {
            _colecao = contexto.Noticias;
            _contexto = contexto;
            CriarOuAtualizarSchemaDaColecao();
            CriarIndiceUnicoParaUrl();
        }

        private void CriarOuAtualizarSchemaDaColecao()
        {
            var db = _contexto.Database;
            var nomeColecao = "noticias";
            var schema = GetMongoJsonSchema();

            var colecoes = db.ListCollectionNames().ToList();

            if (!colecoes.Contains(nomeColecao))
            {
                // Cria a coleção com o schema, se ainda não existir
                var options = new CreateCollectionOptions<BsonDocument>
                {
                    Validator = new BsonDocumentFilterDefinition<BsonDocument>(schema),
                    ValidationAction = DocumentValidationAction.Error,
                    ValidationLevel = DocumentValidationLevel.Strict
                };
                db.CreateCollection(nomeColecao, options);
            }
            else
            {
                // Atualiza a validação da coleção existente usando collMod
                var command = new BsonDocument
                {
                    { "collMod", nomeColecao },
                    { "validator", schema },
                    { "validationLevel", "strict" },
                    { "validationAction", "error" }
                };

                db.RunCommand<BsonDocument>(command);
            }
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
        public static BsonDocument GetMongoJsonSchema()
        {
            var schemaJson = @"
            {
                bsonType: 'object',
                required: [
                    'Titulo', 'Url', 'Texto', 'DataPublicacao', 'Tags', 'Jornalistas',
                    'Comentarios', 'Anexos', 'Visualizacoes', 'TotalComentarios', 'Gostei',
                    'NaoGostei', 'TempoMedioLeitura'
                ],
                properties: {
                    Titulo: { bsonType: 'string' },
                    Url: { bsonType: 'string' },
                    Texto: { bsonType: 'string' },
                    DataPublicacao: { bsonType: 'date' },
                    Tags: {
                    bsonType: 'array',
                    items: { bsonType: 'string' }
                    },
                    Jornalistas: {
                    bsonType: 'array',
                    items: {
                        bsonType: 'object',
                        required: ['Nome'],
                        properties: {
                        Nome: { bsonType: 'string' }
                        }
                    }
                    },
                    Comentarios: {
                    bsonType: 'array',
                    items: {
                        bsonType: 'object',
                        required: ['Comentario', 'Curtidas', 'Usuario', 'Data'],
                        properties: {
                        Comentario: { bsonType: 'string' },
                        Curtidas: { bsonType: 'int' },
                        Usuario: { bsonType: 'string' },
                        Data: { bsonType: 'date' }
                        }
                    }
                    },
                    Anexos: {
                    bsonType: 'array',
                    items: {
                        bsonType: 'object',
                        required: ['NomeArquivo', 'Url', 'Tamanho', 'Tipo', 'Cliques'],
                        properties: {
                        NomeArquivo: { bsonType: 'string' },
                        Url: { bsonType: 'string' },
                        Tamanho: { bsonType: 'int' },
                        Tipo: { bsonType: 'string' },
                        Cliques: { bsonType: 'int' }
                        }
                    }
                    },
                    Visualizacoes: { bsonType: 'int' },
                    TotalComentarios: { bsonType: 'int' },
                    Gostei: { bsonType: 'int' },
                    NaoGostei: { bsonType: 'int' },
                    TempoMedioLeitura: { bsonType: 'double' }
                }
            }";

            return BsonDocument.Parse(@"{ $jsonSchema: " + schemaJson + " }");
        }
        public static void TratarErroValidacaoMongo(MongoWriteException mwx)
        {
            Console.WriteLine("Ocorreu um erro de validação ao tentar inserir a notícia.");

            try
            {
                var detalhes = mwx.WriteError?.Details?.AsBsonDocument;

                if (detalhes != null)
                {
                    Console.WriteLine("Detalhes da violação de schema:");
                    Console.WriteLine(detalhes.ToJson(new MongoDB.Bson.IO.JsonWriterSettings
                    {
                        Indent = true,
                        OutputMode = MongoDB.Bson.IO.JsonOutputMode.RelaxedExtendedJson
                    }));
                }
                else
                {
                    Console.WriteLine("Não foi possível acessar os detalhes da falha de validação.");
                }
            }
            catch (Exception e2)
            {
                Console.WriteLine("Erro ao tentar extrair informações do erro de validação:");
                Console.WriteLine(e2.Message);
            }

            Console.WriteLine("Dica: Verifique os tipos e campos exigidos pelo schema JSON definido no MongoDB.");
        }

        public async Task AtualizarUrlsFaltantesAsync()
        {
            var documentos = await _colecao.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync();
            foreach (var doc in documentos)
            {
                if (!doc.Contains("Url") || string.IsNullOrWhiteSpace(doc["Url"].AsString))
                {
                    var noticia = BsonSerializer.Deserialize<NoticiaClass>(doc);
                    noticia.Titulo = noticia.Titulo;
                    var filtro = Builders<BsonDocument>.Filter.Eq("_id", doc["_id"]);
                    var bsonAtualizado = noticia.ToBsonDocument();

                    await _colecao.ReplaceOneAsync(filtro, bsonAtualizado);
                    Console.WriteLine($"URL atualizada para o documento com _id: {doc["_id"]}");
                }
            }
        }

        public async Task RemoverNoticiasComUrlDuplicadaAsync()
        {
            var documentos = await _colecao.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync();
            var urlsVistas = new HashSet<string>();
            int removidos = 0;
            foreach (var doc in documentos)
            {
                if (!doc.Contains("Url") || doc["Url"].IsBsonNull || doc["Url"].BsonType != BsonType.String)
                    continue;

                string url = doc["Url"].AsString;

                if (urlsVistas.Contains(url))
                {
                    var filtro = Builders<BsonDocument>.Filter.Eq("_id", doc["_id"]);
                    await _colecao.DeleteOneAsync(filtro);
                    Console.WriteLine($"Documento com URL duplicada removido: {url}");
                    removidos++;
                }
                else
                {
                    urlsVistas.Add(url);
                }
            }
        }

        private void CriarIndiceUnicoParaUrl()
        {
            var indexKeys = Builders<BsonDocument>.IndexKeys.Ascending("Url");
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<BsonDocument>(indexKeys, indexOptions);
            _colecao.Indexes.CreateOne(indexModel);
        }
    }
}