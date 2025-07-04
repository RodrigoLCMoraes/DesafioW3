using ApiCartao.Helpers;
using ApiCartao.Models;
using ApiCartao.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace ApiCartao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartaoController : ControllerBase
    {
        private readonly CartaoService _cartaoService;

        public CartaoController(CartaoService cartaoService)
        {
            _cartaoService = cartaoService;
        }
        [HttpPost("Solicitar")]
        public IActionResult CriarCartao([FromBody] CartaoModel solicitacao)
        {
            if (solicitacao.Idade < 18)
            {
                return BadRequest(new
                {
                    Aprovado = false,
                    Mensagem = "Erro! É necessário ser maior de idade."
                });
            }
            if (!CpfValidar.ValidarCpf(solicitacao.Cpf))
            {
                return BadRequest(new
                {
                    Aprovado = false,
                    Mensagem = "CPF inválido!"
                });
            }
            if (!solicitacao.SenhaValida)
            {
                return BadRequest(new
                {
                    Aprovado = false,
                    Mensagem = "Senha inválida! Use pelo menos 6 caracteres sem repetições ou sequências."
                });
            }

            //Hash na senha
            solicitacao.Senha = SenhaHash.GerarHash(solicitacao.Senha);

            //Gera número do cartão (string)
            solicitacao.NumeroCartao = new Random().Next(10000000, 99999999).ToString();

            //Seta status inicial
            solicitacao.Status = "Solicitado";

            //----------------------VERIFICAÇÃO DO CARTÃO------------------------------------//
            string bandeira;
            decimal limite = 0m;

            if (solicitacao.DebOuCred?.ToLower() == "crédito")
            {
                if (solicitacao.RendaMensal >= 7000)
                    bandeira = "Visa";
                else if (solicitacao.RendaMensal >= 5000)
                    bandeira = "MasterCard";
                else if (solicitacao.RendaMensal >= 1500)
                    bandeira = "Elo";
                else
                    return BadRequest(new { Mensagem = "Renda insuficiente para cartão de crédito." });

                limite = solicitacao.RendaMensal * 0.3m;
            }
            else if (solicitacao.DebOuCred?.ToLower() == "débito")
            {
                bandeira = solicitacao.RendaMensal >= 7000 ? "Visa" :
                           solicitacao.RendaMensal >= 5000 ? "MasterCard" : "Elo";
                limite = 0; //débito normalmente não tem limite
            }
            else
            {
                return BadRequest(new { Mensagem = "Tipo de cartão inválido." });
            }

            //Atualiza a bandeira no objeto
            solicitacao.Bandeira = bandeira;

            //Salva no MongoDB
            _cartaoService.Create(solicitacao);

            return Ok(new
            {
                Aprovado = true,
                Status = solicitacao.Status,
                Tipo = solicitacao.DebOuCred,
                Mensagem = "Pedido em análise!",
                Limite = limite,
                Bandeira = bandeira,
                NumeroCartao = solicitacao.NumeroCartao
            });
        }

        [HttpPost("Ativar")]
        public IActionResult Ativar([FromBody] AtivaçãoModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Senha) || request.Senha.Length < 6)
            {
                return BadRequest(new { Mensagem = "Senha inválida, mínimo 6 caracteres" });
            }

            string senhaHash = SenhaHash.GerarHash(request.Senha);

            var sucesso = _cartaoService.AtivarCartao(request.NumeroCartao, request.Cpf, senhaHash);

            if (!sucesso)
            {
                return BadRequest(new { Mensagem = "Cartão não encontrado ou não está apto para ativação" });
            }

            return Ok(new { Mensagem = "Cartão ativado com sucesso!" });
        }
        [HttpPost("BloqueioLote")]
        public async Task<IActionResult> BloqueioLote(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest(new { Mensagem = "Arquivo não enviado" });

            var erros = new List<string>();
            int sucesso = 0;
            int falha = 0;

            var dataHoje = DateTime.Now.ToString("yyyyMMdd");
            var nomeErr = $"CARD{dataHoje}001.ERR";
            var caminhoErr = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "erros", nomeErr);

            using var reader = new StreamReader(arquivo.OpenReadStream());
            while (!reader.EndOfStream)
            {
                var linha = await reader.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(linha) || linha.Length < 43)
                {
                    erros.Add($"{linha} ERRO: Linha inválida");
                    falha++;
                    continue;
                }

                var numeroConta = linha.Substring(20, 15).Trim();
                var motivo = int.Parse(linha.Substring(35, 2));
                var idOperador = int.Parse(linha.Substring(45, 1));

                string mensagemErro;
                var ok = _cartaoService.BloquearCartao(numeroConta, motivo, idOperador, out mensagemErro);

                if (!ok)
                {
                    erros.Add($"{linha.Substring(0, 2)} {DateTime.Now:yyyyMMdd} 000001 ERRO: {mensagemErro}");
                    falha++;
                }
                else
                {
                    sucesso++;
                }
            }

            if (erros.Any())
                await System.IO.File.WriteAllLinesAsync(caminhoErr, erros);

            return Ok(new
            {
                Mensagem = $"Processamento concluído: {sucesso} sucesso(s), {falha} erro(s).",
                CaminhoErr = erros.Any() ? $"/erros/{nomeErr}" : null
            });
        }


    }
}


