# GamePadAPI.SeleniumTests

Suíte de **10 testes funcionais automatizados (CT11–CT20)** do GamePad, escrita em C# com
xUnit + Selenium WebDriver, dirigindo o navegador **Microsoft Edge** sobre o front-end (Vite)
integrado ao back-end local.

Os testes são propositalmente **lentos e visíveis** (janela 1920×1080, pausas e destaque cor-de-rosa
nos elementos) para facilitar a **gravação em vídeo**.

## Pré-requisitos

1. **Back-end no ar** — pelo Visual Studio (IIS Express, `https://localhost:44391`) ou
   `dotnet run`. O `appsettings.Development.json` precisa de um `Igdb:ClientSecret` válido
   (os testes CT12/CT13/CT14 dependem da IGDB).
2. **Front-end no ar** — na pasta `src/front`: `npm run dev` (sobe em `http://localhost:5173`).
3. **`src/front/.env`** com `VITE_API_URL` apontando para o back-end (ex.: `https://localhost:44391`).
4. **Microsoft Edge** instalado (o driver é baixado automaticamente pelo Selenium Manager).

## Como executar (modo gravação, navegador visível)

Na pasta `src/back/GamePadAPI`:

```bash
dotnet test GamePadAPI.SeleniumTests/GamePadAPI.SeleniumTests.csproj
```

Os testes rodam **em ordem** (CT11 → CT20), um por vez, com o Edge visível. Basta gravar a tela.

## Variáveis de ambiente (opcionais)

| Variável        | Padrão                  | Para que serve                                   |
|-----------------|-------------------------|--------------------------------------------------|
| `BASE_URL`      | `http://localhost:5173` | URL do front-end                                 |
| `PAUSE_MS`      | `1000`                  | Pausa entre as ações                             |
| `TYPE_DELAY_MS` | `70`                    | Atraso entre cada caractere digitado             |
| `HEADLESS`      | (desligado)             | `HEADLESS=1` roda sem janela (validação rápida)  |
| `SHOT_DIR`      | (desligado)             | Pasta para salvar screenshot/HTML ao fim de cada teste (diagnóstico) |

Exemplo de validação rápida (sem janela, sem pausas):

```bash
HEADLESS=1 PAUSE_MS=0 TYPE_DELAY_MS=0 dotnet test GamePadAPI.SeleniumTests/GamePadAPI.SeleniumTests.csproj
```

## Os 10 casos

| CT  | Fluxo                                                        | Depende de   |
|-----|-------------------------------------------------------------|--------------|
| CT11 | Home carrega com logo e navbar                              | front        |
| CT12 | Navegar para "Jogos" pela navbar lista jogos               | front + IGDB |
| CT13 | Buscar um jogo retorna resultados                          | front + IGDB |
| CT14 | Abrir o detalhe de um jogo                                 | front + IGDB |
| CT15 | Abrir o modal de login                                     | front        |
| CT16 | Login com credenciais inválidas exibe erro                 | back         |
| CT17 | Validação do cadastro (campo obrigatório)                  | front        |
| CT18 | Cadastrar um novo usuário                                  | back         |
| CT19 | Login com o usuário cadastrado exibe "Bem-vindo!"          | back         |
| CT20 | Usuário logado cria uma lista de jogos                     | back         |
