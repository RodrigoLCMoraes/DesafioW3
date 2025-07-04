using ApiCartao.Models;
using ApiCartao.Settings;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace ApiCartao.Services
{
    public class CartaoService
    {
        private readonly IMongoCollection<CartaoModel> _cartoes;

        public CartaoService(IOptions<MongoDBSettings> settings)
        {
            var cliente = new MongoClient(settings.Value.ConnectionString);
            var database = cliente.GetDatabase(settings.Value.DatabaseName);
            _cartoes = database.GetCollection<CartaoModel>("Cartoes");
        }

        public void Create(CartaoModel cartao)
        {
            _cartoes.InsertOne(cartao);
        }

        public bool AtivarCartao(string numeroCartao, string cpf, string senhaHash)
        {
            var filtro = Builders<CartaoModel>.Filter.And(
                Builders<CartaoModel>.Filter.Eq(c => c.NumeroCartao, numeroCartao),
                Builders<CartaoModel>.Filter.Eq(c => c.Cpf, cpf)
            );

            var cartao = _cartoes.Find(filtro).FirstOrDefault();

            if (cartao == null)
                return false;

            if (cartao.Status != "APROVADO" && cartao.Status != "ENTREGUE" && cartao.Status != "Solicitado")
                return false;

            var atualizacao = Builders<CartaoModel>.Update
                .Set(c => c.Senha, senhaHash)
                .Set(c => c.Status, "ATIVO");

            _cartoes.UpdateOne(filtro, atualizacao);

            return true;
        }
        public bool BloquearCartao(string numeroConta, int motivo, int idOperador, out string mensagemErro)
        {
            mensagemErro = "";

            var cartao = _cartoes.Find(c => c.NumeroCartao == numeroConta).FirstOrDefault();

            if (cartao == null)
            {
                mensagemErro = "Conta não encontrada";
                return false;
            }

            if (cartao.Status != "ATIVO")
            {
                mensagemErro = "Conta não pode ser bloqueada";
                return false;
            }

            var filtro = Builders<CartaoModel>.Filter.Eq(c => c.NumeroCartao, numeroConta);
            var atualizacao = Builders<CartaoModel>.Update.Set(c => c.Status, "BLOQUEADO");

            _cartoes.UpdateOne(filtro, atualizacao);

            return true;
        }

    }

}
