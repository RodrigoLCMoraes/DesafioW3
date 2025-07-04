API - Solicitação de cartão:
- Necessário para solicitar: Nome, Data de Nascimento, CPF, Renda Mensal, Descrever se a solicitação é pra Crédito ou Débito e uma Senha;
- O cartão é recusado pelos seguintes motivos: Caso o usuário seja menor de idade, use CPF inválido ou crie uma senha menor de 6 caracteres ou que contenha repetições;
- Caso a renda mensal do usuário seja de 7000 pra cima, ele recebe um cartão com a bandeira VISA com um limite da sua renda multiplicado por 0.30m. A partir de 5000, cartão com bandeira MASTERCARD com o mesmo calculo para o limite. Com renda a partir de 1500, bandeira ELO.
- Com todas as informações sendo válidas, uma numeração para o cartão é gerada e a senha que o usuário cadastrou é gerada em hash.
- Status final: SOLICITADO;

API - Ativação de cartão:
- Dados necessários: Número do cartão, CPF e senha cadastrada;
- Toda validação é contada (Número do cartão existe, CPF válido e senha de acordo com a cadastrada pelo usuário que deseja ativar;
- Status: ATIVO

API - Bloqueio de cartão:
- No Swagger, ele pede que envie um arquivo .IN e, caso tenha erros, retorna um arquivo .ERR;

RODAR A API
- Com o .NET 09 (o que foi usado durante a programação), é necessário que após a inicialização do projeto, quando abrir a conexão local, adicione "/swagger/index.html" (sem aspas).
- Na pasta "Properties", no arquivo "launchSettings.json", é necessário ajustar o host (ou a URL) para corresponder ao endereço local da máquina onde a API será executada.
- Caso não tenha instalado os pacotes do Swagger na máquina, é necessário instalar o Swashbuckle.AspNetCore.SwaggerGen e Swashbuckle.AspNetCore.SwaggerUI;
