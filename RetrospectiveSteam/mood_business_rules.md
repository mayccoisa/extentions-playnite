# Regras de Negócio: Mood do Ano (Personas)

Este documento define as regras para classificar o "Mood do Ano" do jogador com base no tempo de jogo investido em categorias específicas.

## Algoritmo de Classificação

1.  **Gatilhos**: Cada Persona possui uma lista de "Tags" ou "Gêneros" associados.
2.  **Pontuação**: Para cada jogo jogado no ano, se ele possuir PELO MENOS UM dos gatilhos de uma Persona, o tempo total de jogo desse título naquele ano é somado à pontuação da Persona.
3.  **Unicidade por Jogo**: O tempo do jogo é somado apenas uma vez por Persona, independente de quantos gatilhos ele atinja para aquela Persona específica.
4.  **Vencedor**: A Persona com o maior tempo acumulado é selecionada.
5.  **Fallback**: Se nenhum gatilho for atingido (tempo = 0), a Persona selecionada é "O GAMER ECLÉTICO".

## Tabela de Personas

| Título | Gatilhos (Tags/Gêneros) | Subtítulo |
| :--- | :--- | :--- |
| **O HERÓI DE ÉPICOS** | RPG, Fantasia, Mundo Aberto, JRPG | Sua jornada foi marcada por espadas, magia e mundos vastos. |
| **O SOBREVIVENTE IMPLACÁVEL** | Roguelike, Roguelite, Souls-like, Permadeath | Você riu na cara do 'Game Over' e tentou só mais uma vez. |
| **O MESTRE DO CHILL** | Cozy, Simulação Agrícola, Sandbox, Relaxante, Farming | Seu ano foi puro relaxamento, construindo e cultivando em paz. |
| **O ATIRADOR DE ELITE** | FPS, Shooter, Battle Royale, Tático, Tiros em Primeira Pessoa | Reflexos rápidos, mira precisa e muita munição gasta. |
| **O ARQUITETO DE IMPÉRIOS** | Estratégia, RTS, 4X, Construção de Cidades, Card Battler | Planos complexos, economia e vitórias calculadas friamente. |
| **O CAÇADOR DE PESADELOS** | Terror, Survival Horror, Zumbis, Psicológico | Você escolheu passar o ano com a luz acesa e o coração acelerado. |
| **A ALMA DA FESTA** | Co-op, Multiplayer Local, Party Game, Luta | Sessões caóticas, risadas e amizades (quase) destruídas. |
| **O REI DAS PISTAS** | Corrida, Esportes, Simulação de Voo, Arcade | Sua paixão foi a velocidade, a precisão e a quebra de recordes. |
| **O CARTÓGRAFO DE PIXELS** | Metroidvania, Plataforma 2D, Pixel Art, Retro | Você explorou cada canto do mapa em busca de segredos escondidos. |
| **O INVESTIGADOR NARRATIVO** | Rico em História, Visual Novel, Detetive, Noir, Mistério | Você viveu pelo enredo, desvendando mistérios e tomando decisões difíceis. |
| **O INFILTRADO DISTÓPICO** | Cyberpunk, Sci-Fi, Furtivo, Stealth, Hack & Slash | Luzes de neon, tecnologia avançada e futuros não tão distantes. |
| **O GAMER ECLÉTICO** | *Padrão (Fallback)* | Você jogou de tudo um pouco! Um verdadeiro sommelier de videogames. |
