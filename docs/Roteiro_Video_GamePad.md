# Roteiro de Narração — Vídeo do Trabalho Final (GamePad)

**Duração-alvo:** até 10 minutos · **Idioma:** português · **Narração:** 1ª pessoa, tom calmo.

> Dica geral: fale enquanto a ação acontece na tela. Os testes automatizados (unitários e
> Selenium) rodam sozinhos — você só narra por cima. Os manuais você demonstra ao vivo.

---

## 0. Pré-gravação (NÃO grave isto — deixe pronto antes)

**Ambiente:**
1. Subir o **back-end** (Visual Studio → IIS Express, ou `dotnet run`) com `Igdb:ClientSecret` válido no `appsettings.Development.json`.
2. Subir o **front-end**: em `src/front` → `npm run dev` (sobe em `http://localhost:5173`).
3. Conferir o `src/front/.env` com `VITE_API_URL` apontando para o back.
4. Para os testes Selenium ficarem **visíveis e lentos** (bons para vídeo), use as variáveis:
   ```bash
   # PowerShell, na pasta src/back/GamePadAPI
   $env:PAUSE_MS=1200; $env:TYPE_DELAY_MS=80
   ```
   (NÃO use `HEADLESS` — o navegador precisa aparecer.)

**Tela / gravação:**
- Fechar notificações do Windows, abas e janelas desnecessárias.
- Deixar abertos e prontos: (a) terminal na pasta `src/back/GamePadAPI`, (b) navegador na home
  `http://localhost:5173`, (c) o documento `Plano_de_Testes_GamePad.docx` (para mostrar refatoração e defeitos).
- Maximizar as janelas. Resolução 1920×1080 se possível.
- Teste o microfone antes.

---

## 1. Abertura — 0:00 a 0:25

**[Mostrar: rosto/slide ou a home do GamePad]**

> "Olá, professora. Neste vídeo apresento o trabalho final de Teste e Manutenção de Software,
> aplicado no projeto GamePad. Vou demonstrar os 10 testes unitários, os 10 testes funcionais
> automatizados em Selenium e os 10 testes manuais, além da refatoração de dois métodos e do
> relatório de defeitos encontrados. O front-end e o back-end já estão rodando localmente."

---

## 2. Testes Unitários — 0:25 a 2:00

**[Mostrar: terminal na pasta `src/back/GamePadAPI`]**

> "Começando pelos testes unitários. Eles foram feitos em C# com xUnit e Entity Framework Core
> InMemory, seguindo o padrão Arrange–Act–Assert. Vou executar a suíte com `dotnet test`."

**[Rodar:]**
```bash
dotnet test GamePadAPI.Tests/GamePadAPI.Tests.csproj
```

**[Enquanto compila/roda, narrar:]**

> "São dez casos que cobrem as principais regras do back-end: cadastro de usuário com e-mail e
> nome duplicados, cadastro válido com a senha protegida por BCrypt e imagem padrão, autenticação
> com geração de token JWT e com senha incorreta, as regras de avaliação — comentário exige nota,
> nota sem comentário é válida —, o cálculo da média por jogo com filtro mínimo, a regra de não
> curtir o próprio comentário e o gerenciamento de listas."

**[Quando aparecer o resultado:]**

> "E aqui o resultado: dez de dez casos aprovados, zero falhas."

---

## 3. Testes Funcionais Automatizados (Selenium) — 2:00 a 5:30

**[Mostrar: terminal]**

> "Agora os testes funcionais automatizados em Selenium, também em C#, controlando o navegador
> Microsoft Edge sobre a aplicação real. Eles rodam em ordem, do CT11 ao CT20, com pausas para
> ficarem acompanháveis."

**[Rodar:]**
```bash
dotnet test GamePadAPI.SeleniumTests/GamePadAPI.SeleniumTests.csproj
```

**[O Edge abre e executa. Narrar cada caso conforme ele acontece:]**

- **CT11** — "Primeiro, a home carrega com o logo e a barra de navegação visíveis."
- **CT12** — "Aqui o teste navega para a página de Jogos pela navbar e confirma que a lista de jogos aparece."
- **CT13** — "Agora ele digita 'Zelda' na busca e verifica que vêm resultados."
- **CT14** — "Este abre o detalhe de um jogo a partir da listagem."
- **CT15** — "Aqui ele abre o modal de login pela navbar."
- **CT16** — "Login com credenciais inválidas: o teste confirma que aparece a mensagem de erro."
- **CT17** — "Validação do cadastro: ao enviar o formulário vazio, surge o erro de campo obrigatório."
- **CT18** — "Agora um cadastro de usuário novo, com sucesso."
- **CT19** — "Login com esse usuário recém-criado, confirmando a saudação de boas-vindas."
- **CT20** — "E, por fim, o usuário logado cria uma nova lista de jogos."

**[Quando terminar:]**

> "Novamente, dez de dez casos aprovados."

---

## 4. Testes Funcionais Manuais — 5:30 a 8:00

> "Os dez testes manuais, do CT21 ao CT30, estão documentados no plano de testes, com pré-condição,
> procedimento, dados, saída esperada e prioridade. Vou demonstrar os principais ao vivo na aplicação."

**[Mostrar: navegador logado na aplicação]**

**CT21 — Avaliar um jogo (Fazer Review)**
> "No detalhe de um jogo, clico em 'Fazer Review', dou 5 estrelas, escrevo um comentário e salvo.
> O comentário aparece na lista com as estrelas, e o botão muda para 'Editar Review'."

**CT24 — Curtir um jogo**
> "Aqui clico em 'Curtir' — o botão fica destacado como 'Curtido'."

**CT26 — Tentar curtir o próprio comentário**
> "Neste caso, no meu próprio comentário, o botão de curtir aparece desabilitado, com o aviso de
> que não posso curtir meu próprio comentário."

**CT27 — Adicionar um jogo a uma lista**
> "Clico em 'Lista', escolho uma lista existente e adiciono o jogo."

**CT30 — Logout**
> "E para encerrar, abro o menu do usuário e clico em 'Sair' — a navbar volta a mostrar 'Entrar' e
> 'Registrar'."

> "Os demais casos manuais — navegação na galeria, status do jogo, edição de perfil, entre outros —
> seguem o mesmo procedimento documentado no plano de testes."

---

## 5. Refatoração de Métodos — 8:00 a 8:50

**[Mostrar: documento, seção 4 — imagens antes/depois]**

> "Foram refatorados dois métodos do back-end, preservando o comportamento. O primeiro, PostUsuario,
> tinha as validações de e-mail e nome duplicados repetidas em outro método; apliquei Extração de
> Método, criando helpers reutilizáveis e eliminando a duplicação. O segundo, PostAvaliacao, tinha
> uma condição booleana complexa; apliquei Decompor Condicional, dando a ela um nome claro:
> 'ComentarioSemNota'. Como mostrei no início, a suíte unitária continua dez de dez aprovada após as
> mudanças, comprovando que o comportamento foi mantido."

---

## 6. Relatório de Defeitos — 8:50 a 9:40

**[Mostrar: documento, seção 5 — quadro-resumo]**

> "Durante os testes encontrei cinco defeitos, documentados com severidade e proposta de solução.
> Destaco dois: os contadores de estatística do jogo usam códigos de status errados, então o número
> de 'Curtiram' nunca aumenta; e o botão 'Ler mais' da descrição nunca aparece, porque o código
> verifica um campo que não existe nos dados retornados pela API. Para cada defeito há uma proposta
> de correção no relatório."

---

## 7. Encerramento — 9:40 a 10:00

> "Com isso encerro a demonstração: trinta casos de teste, sendo vinte automatizados e dez manuais,
> a refatoração de dois métodos e o relatório de defeitos. Obrigado(a)!"

---

## Resumo de tempo

| Bloco | Início | Fim |
|------|--------|-----|
| Abertura | 0:00 | 0:25 |
| Unitários | 0:25 | 2:00 |
| Selenium | 2:00 | 5:30 |
| Manuais | 5:30 | 8:00 |
| Refatoração | 8:00 | 8:50 |
| Defeitos | 8:50 | 9:40 |
| Encerramento | 9:40 | 10:00 |

> Se faltar tempo: reduza a demonstração manual para 3 casos (CT21, CT26, CT30) e acelere a fala nos
> blocos de refatoração/defeitos. Se sobrar tempo: mostre mais um caso manual ou abra uma tabela de
> caso de teste no documento.
