using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiCartao.Models
{
    public class CartaoModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        private string Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNasc { get; set; }
        public string Cpf { get; set; }
        public decimal RendaMensal { get; set; }
        public string DebOuCred { get; set; }
        public string Senha { get; set; }

        public string Status;
        public string NumeroCartao;
        public string Bandeira;

        public int Idade => CalcularIdade();
        public bool MaiorDeIdade => Idade >= 18;
        public bool SenhaValida => ValidarSenha(Senha);

        private int CalcularIdade()
        {
            var hoje = DateTime.Today;
            var idade = hoje.Year - DataNasc.Year;
            if (DataNasc.Date > hoje.AddYears(-idade)) --idade;
            return idade;
        }

        private bool ValidarSenha(string Senha)
        {
            if (string.IsNullOrEmpty(Senha) || Senha.Length < 6) return false;

            for (int i = 0; i < Senha.Length - 2; i++)
            {
                if (Senha[i + 1] == Senha[i] + 1 && Senha[i + 2] == Senha[i] + 2)
                    return false;

                if (Senha[i + 1] == Senha[i] - 1 && Senha[i + 2] == Senha[i] - 2)
                    return false;

                if (Senha[i] == Senha[i + 1] && Senha[i] == Senha[i + 2])
                    return false;
            }
            return true;
        }
    }
}
