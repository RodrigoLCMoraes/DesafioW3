namespace ApiCartao.Helpers
{
    public class CpfValidar
    {
        public static bool ValidarCpf(String cpf)
        {
            cpf = cpf?.Replace(".", "").Replace("-", "");

            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11 || cpf.All(c => c == cpf[0]))
                return false;

            var numeros = cpf.Select(c => int.Parse(c.ToString())).ToArray();

            int soma = 0;
            for (int i = 0; i < 9; ++i)
                soma += numeros[i] * (10 - i);
            int Digito1 = soma % 11 < 2 ? 0 : 11 - (soma % 11);

            soma = 0;
            for (int i = 0; i < 10; ++i)
                soma += numeros[i] * (11 - i);
            int Digito2 = soma % 11 < 2 ? 0 : 11 - (soma % 11);

            return numeros[9] == Digito1 && numeros[10] == Digito2;
        }
    }
}
