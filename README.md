# GamePad — Trabalho Final de Teste e Manutenção de Software

<br><img src="./docs/images/gamepadHeader.png" width="200px">

<br>

`DISCIPLINA: Teste e Manutenção de Software`

`Profa. Luciana Mara · PUC Minas · 1º semestre/2026`

Este repositório contém a **atividade final da disciplina de Teste e Manutenção de Software**,
aplicada sobre o projeto **GamePad** (plataforma de avaliação e compartilhamento de jogos,
desenvolvida no Trabalho Interdisciplinar). O foco aqui **não é o desenvolvimento da aplicação**,
e sim a **qualidade**: testes unitários, testes funcionais automatizados, testes manuais,
refatoração e relatório de defeitos.

> A documentação e o código originais da aplicação GamePad continuam disponíveis ao final deste
> documento, em **[Sobre a aplicação GamePad](#sobre-a-aplicação-gamepad)**.

## O que foi entregue

| Entrega | Onde está | Status |
|---|---|---|
| **10 testes unitários** (CT01–CT10) | [`src/back/GamePadAPI/GamePadAPI.Tests`](src/back/GamePadAPI/GamePadAPI.Tests) | ✅ 10/10 aprovados |
| **10 testes funcionais Selenium** (CT11–CT20) | [`src/back/GamePadAPI/GamePadAPI.SeleniumTests`](src/back/GamePadAPI/GamePadAPI.SeleniumTests) | ✅ |
| **10 testes funcionais manuais** (CT21–CT30) | [`docs/Plano_de_Testes_GamePad.docx`](docs/Plano_de_Testes_GamePad.docx) — seção 3 | ✅ |
| **Refatoração de 2 métodos** (antes/depois) | [`docs/refatoracao/`](docs/refatoracao) + .docx seção 4 | ✅ |
| **Relatório de defeitos** (D01–D05 + soluções) | `.docx` seção 5 | ✅ |
| **Vídeo narrativo** (execução dos testes) | [`docs/Roteiro_Video_GamePad.md`](docs/Roteiro_Video_GamePad.md) | 🎬 |

Os 30 casos de teste estão documentados no template oficial em
[`docs/Plano_de_Testes_GamePad.docx`](docs/Plano_de_Testes_GamePad.docx), com os campos
Caso de Teste, Pré-Condição, Procedimento, Dados de entrada, Saída esperada, Saída encontrada,
Prioridade e Técnica.

## Estrutura dos testes

```
src/back/GamePadAPI/
├── GamePadAPI/                  # Aplicação (ASP.NET Core 8 + EF Core 8)
├── GamePadAPI.Tests/           # 10 testes unitários (xUnit + EF InMemory)  — CT01–CT10
└── GamePadAPI.SeleniumTests/   # 10 testes funcionais (xUnit + Selenium)    — CT11–CT20
```

### Testes unitários (CT01–CT10)

xUnit com banco em memória (EF Core InMemory) — não exigem SQL Server nem rede.

```bash
cd src/back/GamePadAPI
dotnet test GamePadAPI.Tests/GamePadAPI.Tests.csproj
```

### Testes funcionais Selenium (CT11–CT20)

Dirigem o **Microsoft Edge** sobre o front-end (Vite) + back-end locais. São propositalmente
**lentos e visíveis** (janela 1920×1080, pausas e destaque dos elementos) para facilitar a
gravação do vídeo. Pré-requisitos e detalhes de cada caso no
**[README da suíte Selenium](src/back/GamePadAPI/GamePadAPI.SeleniumTests/README.md)**.

```bash
# com front (npm run dev) e back no ar:
cd src/back/GamePadAPI
dotnet test GamePadAPI.SeleniumTests/GamePadAPI.SeleniumTests.csproj
```

### Testes manuais (CT21–CT30)

Casos de caixa-preta documentados com prints na seção 3 do
[`Plano_de_Testes_GamePad.docx`](docs/Plano_de_Testes_GamePad.docx).

## Refatorações realizadas

Dois métodos refatorados preservando o comportamento (os 10 testes unitários continuam passando):

| Método | Técnica | Arquivo |
|---|---|---|
| `PostUsuario` | Extração de Método (`EmailJaCadastrado` / `NomeJaCadastrado`, reusados em `PutUsuario`) | `GamePadAPI/Controllers/UsuariosController.cs` |
| `PostAvaliacao` | Decompor Condicional (`ComentarioSemNota`) | `GamePadAPI/Controllers/AvaliacoesApiController.cs` |

Prints antes/depois em [`docs/refatoracao/`](docs/refatoracao).

## Stack

- **Back-end:** ASP.NET Core 8, EF Core 8 (SQL Server), BCrypt, JWT
- **Front-end:** React + Vite
- **Testes:** xUnit, EF Core InMemory, Selenium WebDriver (Edge)

## Integrantes

* Alex Mendes dos Santos
* Andry Marques Pereira da Silveira
* Isaac Souza Fernandes
* Pablo Marques Cordeiro
* Ramon Pereira de Souza
* Yalle Ramos Ferrari de Magalhaes

---

# Sobre a aplicação GamePad

`CURSO: Análise e Desenvolvimento de Sistemas`

`DISCIPLINA: Trabalho Interdisciplinar Desenvolvimento de Aplicação Interativa`

`3º semestre/2025` · Professor: Kleber Jacques Ferreira de Souza

Plataforma voltada para lazer e entretenimento, onde os usuários podem avaliar e compartilhar
suas opiniões sobre jogos. Através de notas e comentários, a plataforma facilita a descoberta de
jogos bem avaliados, ajudando os usuários a tomarem decisões mais rápidas e assertivas sobre o
que jogar. O objetivo é criar um espaço interativo e colaborativo, onde a troca de experiências
enriquece a comunidade e melhora a experiência de entretenimento de todos.

## Como executar a aplicação

```bash
# 1. Back-end
cd src/back/GamePadAPI/GamePadAPI
dotnet restore
dotnet ef database update   # requer SQL Server e connection string em appsettings.json
dotnet run

# 2. Front-end (em outro terminal)
cd src/front
npm install
npm run dev
```

Acesse: http://localhost:5173

## Documentação do projeto

<ol>
<li><a href="docs/01-Contexto.md"> Documentação de contexto</a></li>
<li><a href="docs/02-Especificacao.md"> Especificação do projeto</a></li>
<li><a href="docs/03-Metodologia.md"> Metodologia</a></li>
<li><a href="docs/04-Projeto-interface.md"> Projeto de interface</a></li>
<li><a href="docs/05-Template-padrao.md"> Template padrão da aplicação</a></li>
<li><a href="docs/06-Arquitetura-solucao.md"> Arquitetura da solução</a></li>
<li><a href="docs/07-Plano-testes-software.md"> Plano de testes de software</a></li>
<li><a href="docs/08-Registro-testes-software.md"> Registro de testes de software</a></li>
<li><a href="docs/09-Plano-testes-usabilidade.md"> Plano de testes de usabilidade</a></li>
<li><a href="docs/10-Registro-testes-usabilidade.md"> Registro de testes de usabilidade</a></li>
<li><a href="docs/11-Referencias.md"> Referências</a></li>
</ol>

* <a href="src/README.md">Código</a>
* <a href="presentation/README.md">Apresentação do projeto</a>
</content>
</invoke>
