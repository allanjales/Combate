# COMBATE ONLINE

![GitHub release (latest by date)](https://img.shields.io/badge/release-v1.2.2-31795E)

Recriação online do jogo Combate (original [Stratego](https://en.wikipedia.org/wiki/Stratego))
distribuido pela [Estrela](https://pt.wikipedia.org/wiki/Estrela_(empresa))
em parceria com a [Hasbro](https://pt.wikipedia.org/wiki/Hasbro).
O jogo surgiu em 1942 e ao longo dos anos teve vários temas.
O tema escolhido aqui foi o Combate da Estrela por memórias afetivas.

## Tabela de conteúdo

- [COMBATE ONLINE](#combate-online)
- [Tabela de conteúdo](#tabela-de-conteudo)
- [Como baixar](#como-baixar)
- [Como jogar](#como-jogar)
	- [Necessidades](#necessidades)
	- [Como abrir](#como-abrir)
	- [Como conectar com um amigo](#como-conectar-com-um-amigo)
- [Regras](#regras)
    - [Objetivo](#objetivo)
	- [Exército](#exercito)
	- [Turnos](#turno)
	- [Movimentação](#movimentacao)
	- [Ataque](#ataque)
	- [Como vencer](#como-vencer)

## Como baixar

Para baixar o jogo, ou verificar se está disponível para seu sistema operacional, você deve seguir o seguintes passos:

1. Ir para a tela de [releases](https://github.com/allanjales/Combate/releases/),
clicando [aqui](https://github.com/allanjales/Combate/releases/);
1. No release mais recente, verificar se tem a versão para o seu sistema operacional;
1. Baixar o arquivo correspondente.

## Como jogar

### Necessidades

O jogo não tem um sistema de matchmaking aleatório, então é imprescindível a existência de um amigo para jogar com você.
Também é necessário conexão com a internet. Não temos jogo local.

Em resumo, são necessários:

- 2 jogadores;
- conexão com a internet.

Note que não é possível e permitido jogar com mais ou menos jogadores.

### Como abrir

Uma vez com o arquivo `.zip` correspondente ao seu sistema operacional baixado (ver [Como baixar](#como-baixar)),
você deve extrai-lo em alguma pasta vazia e abrir o arquivo executável dentro:

- `Combate.exe`

### Como conectar com um amigo

Para conectar com um amigo, basta que os dois, ao abrirem o jogo,
digitem o mesmo nome de sala no campo correspondente e digitem algum apelido para si mesmos durante a partida.

## Regras

O Combate é jogado em turnos e todos seus movimentos serão feitos com o mouse.

### Objetivo

Seu objetivo no jogo é capturar a bandeira inimiga.

### Exército

Seu exército será escolhido aleatóriamente no início de cada partida.
Como regra, o exército vermelho sempre terá o primeiro movimento da partida.

### Turnos

Existem três turnos no jogo:

- **Turno de posicionamento** das peças só existe no início da partida.
Você deve trocar suas peças de posição para proteger sua bandeira e avançar sobre os inimigos.
Este turno só acaba quando os dois jogadores dão pronto;
- **Turno de movimentação**: O jogador da vez movimenta uma peça ou ataca;
- **Turno de ataque**: Quando a peça que o jogador movimentou pode atacar alguma peça,
o jogador tem o direito de escolha de ataca-la ou não.

### Movimentação

Todas as peças de valores 1 à 10 podem se movimentar.
As peças que se movimentam só podem andar para quadrados adjacentes.

O `(2) Soldado` que pode escolher uma direção e andar quantos quadrados quiser até um obstáculo.
Contudo, ao fazer isso ele perde a oportunidade de atacar em seguida e deve passar o turno pro outro jogador.

### Ataque

Para atacar, o adversário deve, com uma peça selecionada, selecionar uma peça adversária num quadrado adjacente.

Ganha a peça que tiver maior número, salvo algumas exeções:

- A peça atacada é a `(!) Bandeira` -> quem ataca ganha;
- A peça atacada é uma `(-) Bomba` -> ambas peças saem da partida
    - Se for um `(3) Cabo-armeiro`, ele desarma a bomba e apenas a bomba sai.
- Se um `(1) Espião` ataca o `(10) Marechal`, o Marechal sai.

### Como vencer

Existem 3 formas da partida terminar:

- Jogador capturou a `(!) Bandeira` (quem capturou ganha);
- Jogador ficou sem movimentos disponíveis (o outro ganha);
- Jogador saiu (o outro ganha)

Há a possibilidade também de haver um empate caso os dois jogadores fiquem sem movimentos.