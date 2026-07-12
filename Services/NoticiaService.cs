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

        public async Task AtualizarTextoPorUrlAsync(string url, string novoTexto)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("A URL não pode ser vazia.");

            if (string.IsNullOrWhiteSpace(novoTexto))
                throw new ArgumentException("O novo texto não pode ser vazio.");

            var filtro = Builders<BsonDocument>.Filter.Eq("Url", url);
            var atualizacao = Builders<BsonDocument>.Update.Set("Texto", novoTexto);
            var resultado = await _colecao.UpdateOneAsync(filtro, atualizacao);

            if (resultado.MatchedCount == 0)
            {
                Console.WriteLine($"Nenhum documento encontrado com URL: {url}");
            }
            else if (resultado.ModifiedCount > 0)
            {
                Console.WriteLine($"Texto atualizado com sucesso para a URL: {url}");
            }
            else
            {
                Console.WriteLine($"Documento encontrado, mas o texto já estava igual.");
            }
        }

        public async Task AdicionarComentarioAsync(string url, ComentarioClass novoComentario)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("A URL não pode ser vazia.");

            if (novoComentario == null)
                throw new ArgumentNullException(nameof(novoComentario));

            var filtro = Builders<BsonDocument>.Filter.Eq("Url", url);
            var updateComentario = Builders<BsonDocument>.Update.Push("Comentarios", novoComentario.ToBsonDocument());
            var resultado1 = await _colecao.UpdateOneAsync(filtro, updateComentario);

            if (resultado1.MatchedCount == 0)
            {
                Console.WriteLine($"Nenhuma notícia encontrada com a URL: {url}");
                return;
            }

            var noticiaAtualizada = await _colecao.Find(filtro).FirstOrDefaultAsync();
            var comentarios = noticiaAtualizada["Comentarios"].AsBsonArray;

            int totalComentarios = comentarios.Count;
            var updateTotal = Builders<BsonDocument>.Update.Set("TotalComentarios", totalComentarios);

            await _colecao.UpdateOneAsync(filtro, updateTotal);
            Console.WriteLine($"Comentário adicionado com sucesso. Total agora: {totalComentarios}");
        }

        public async Task RemoverComentarioAsync(string url, string textoComentario)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Url", url);
            var update = Builders<BsonDocument>.Update.PullFilter("Comentarios",
                Builders<BsonDocument>.Filter.Eq("Comentario", textoComentario));

            var resultado = await _colecao.UpdateOneAsync(filter, update);

            if (resultado.ModifiedCount > 0)
            {
                var noticiaAtualizada = await _colecao.Find(filter).FirstOrDefaultAsync();
                if (noticiaAtualizada != null)
                {
                    var totalAtual = noticiaAtualizada["Comentarios"].AsBsonArray.Count;
                    var updateTotal = Builders<BsonDocument>.Update.Set("TotalComentarios", totalAtual);
                    await _colecao.UpdateOneAsync(filter, updateTotal);
                    Console.WriteLine($"Comentário removido. Total atualizado: {totalAtual}");
                }
                else
                {
                    Console.WriteLine("Nenhum comentário foi removido. Verifique se o texto está correto.");
                }
            }
        }

        public async Task AtualizarEstatisticasVisualizacaoAsync(string url, int sinalGostou, double tempoLeituraAtual)
        {
            var filtro = Builders<BsonDocument>.Filter.Eq("Url", url);
            var noticia = await _colecao.Find(filtro).FirstOrDefaultAsync();

            if (noticia == null)
            {
                Console.WriteLine($"Notícia com URL '{url}' não encontrada.");
                return;
            }

            int visualizacoes = noticia.GetValue("Visualizacoes", 0).AsInt32;
            int gostei = noticia.GetValue("Gostei", 0).AsInt32;
            int naoGostei = noticia.GetValue("NaoGostei", 0).AsInt32;
            double tempoMedio = noticia.GetValue("TempoMedioLeitura", 0.0).ToDouble();

            int novaQtdVisualizacoes = visualizacoes + 1;
            double novoTempoMedio = ((tempoMedio * visualizacoes) + tempoLeituraAtual) / novaQtdVisualizacoes;

            var atualizacao = Builders<BsonDocument>.Update
                .Inc("Visualizacoes", 1)
                .Set("TempoMedioLeitura", novoTempoMedio);

            if (sinalGostou == 1)
                atualizacao = atualizacao.Inc("Gostei", 1);
            else if (sinalGostou == -1)
                atualizacao = atualizacao.Inc("NaoGostei", 1);

            await _colecao.UpdateOneAsync(filtro, atualizacao);
            Console.WriteLine("Estatísticas atualizadas com sucesso.");
        }

        public async Task<(int totalGostei, int totalNaoGostei)> ObterReacoesNoPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            var filtro = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Gte("DataPublicacao", dataInicio),
                    Builders<BsonDocument>.Filter.Lte("DataPublicacao", dataFim)
                );

            var projeção = Builders<BsonDocument>.Projection.Include("Gostei").Include("NaoGostei");
            var documentos = await _colecao.Find(filtro).Project(projeção).ToListAsync();

            var totalGostei = 0;
            var totalNaoGostei = 0;

            foreach (var doc in documentos)
            {
                totalGostei += doc.GetValue("Gostei", 0).AsInt32;
                totalNaoGostei += doc.GetValue("NaoGostei", 0).AsInt32;
            }

            return (totalGostei, totalNaoGostei);
        }

        public async Task<(int totalGostei, int totalNaoGostei)> ObterReacoesPorFiltroAsync(
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string? nomeJornalista = null,
            string? tag = null)
        {
            var filtros = new List<FilterDefinition<BsonDocument>>();
            filtros.Add(Builders<BsonDocument>.Filter.Gte("DataPublicacao", dataInicio.Value));
            filtros.Add(Builders<BsonDocument>.Filter.Lte("DataPublicacao", dataFim.Value));
            filtros.Add(Builders<BsonDocument>.Filter.AnyEq("Tags", tag));
            filtros.Add(
                Builders<BsonDocument>.Filter.ElemMatch<BsonDocument>(
                    "Jornalistas",
                    Builders<BsonDocument>.Filter.Eq("Nome", nomeJornalista)
                )
            );

            var filtroFinal = filtros.Count > 0
                ? Builders<BsonDocument>.Filter.And(filtros)
                : Builders<BsonDocument>.Filter.Empty;

            var projeção = Builders<BsonDocument>.Projection.Include("Gostei").Include("NaoGostei");
            var documentos = await _colecao.Find(filtroFinal).Project(projeção).ToListAsync();

            var totalGostei = 0;
            var totalNaoGostei = 0;

            foreach (var doc in documentos)
            {
                totalGostei += doc.GetValue("Gostei", 0).AsInt32;
                totalNaoGostei += doc.GetValue("NaoGostei", 0).AsInt32;
            }

            return (totalGostei, totalNaoGostei);
        }
        public async Task<(string? maisAprovada, string? maisRejeitada)> ObterNoticiasMaisRelevantesAsync(DateTime dataInicio, DateTime dataFim)
        {
            var filtro = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Gte("DataPublicacao", dataInicio),
                    Builders<BsonDocument>.Filter.Lte("DataPublicacao", dataFim)
                );
            var projecao = Builders<BsonDocument>.Projection
                .Include("Titulo")
                .Include("Gostei")
                .Include("NaoGostei");

            var ordenacaoGostei = Builders<BsonDocument>.Sort.Descending("Gostei");
            var ordenacaoNaoGostei = Builders<BsonDocument>.Sort.Descending("NaoGostei");

            var maisGosteiDoc = await _colecao
                .Find(filtro)
                .Project(projecao)
                .Sort(ordenacaoGostei)
                .Limit(1)
                .FirstOrDefaultAsync();

            var maisNaoGosteiDoc = await _colecao
                .Find(filtro)
                .Project(projecao)
                .Sort(ordenacaoNaoGostei)
                .Limit(1)
                .FirstOrDefaultAsync();

            string? tituloMaisAprovada = maisGosteiDoc?.GetValue("Titulo", null)?.AsString;
            string? tituloMaisRejeitada = maisNaoGosteiDoc?.GetValue("Titulo", null)?.AsString;

            return (tituloMaisAprovada, tituloMaisRejeitada);
        }
    }
}