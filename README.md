# COMBATE ONLINE

![GitHub release (latest by date)](https://img.shields.io/badge/release-v1.2.2-31795E)

Recria��o online do jogo Combate (original [Stratego](https://en.wikipedia.org/wiki/Stratego))
distribuido pela [Estrela](https://pt.wikipedia.org/wiki/Estrela_(empresa))
em parceria com a [Hasbro](https://pt.wikipedia.org/wiki/Hasbro).
O jogo surgiu em 1942 e ao longo dos anos teve v�rios temas.
O tema escolhido aqui foi o Combate da Estrela por mem�rias afetivas.

## Tabela de conte�do

- [COMBATE ONLINE](#combate-online)
- [Tabela de conte�do](#tabela-de-conteudo)
- [Como baixar](#como-baixar)
- [Como jogar](#como-jogar)
	- [Necessidades](#necessidades)
	- [Como abrir](#como-abrir)
	- [Como conectar com um amigo](#como-conectar-com-um-amigo)
- [Regras](#regras)
    - [Objetivo](#objetivo)
	- [Ex�rcito](#exercito)
	- [Turnos](#turno)
	- [Movimenta��o](#movimentacao)
	- [Ataque](#ataque)
	- [Como vencer](#como-vencer)

## Como baixar

Para baixar o jogo, ou verificar se est� dispon�vel para seu sistema operacional, voc� deve seguir o seguintes passos:

1. Ir para a tela de [releases](https://github.com/allanjales/Combate/releases/),
clicando [aqui](https://github.com/allanjales/Combate/releases/);
1. No release mais recente, verificar se tem a vers�o para o seu sistema operacional;
1. Baixar o arquivo correspondente.

## Como jogar

### Necessidades

O jogo n�o tem um sistema de matchmaking aleat�rio, ent�o � imprescind�vel a exist�ncia de um amigo para jogar com voc�.
Tamb�m � necess�rio conex�o com a internet. N�o temos jogo local.

Em resumo, s�o necess�rios:

- 2 jogadores;
- conex�o com a internet.

Note que n�o � poss�vel e permitido jogar com mais ou menos jogadores.

### Como abrir

Uma vez com o arquivo `.zip` correspondente ao seu sistema operacional baixado (ver [Como baixar](#como-baixar)),
voc� deve extrai-lo em alguma pasta vazia e abrir o arquivo execut�vel dentro:

- `Combate.exe`

### Como conectar com um amigo

Para conectar com um amigo, basta que os dois, ao abrirem o jogo,
digitem o mesmo nome de sala no campo correspondente e digitem algum apelido para si mesmos durante a partida.

## Regras

O Combate � jogado em turnos e todos seus movimentos ser�o feitos com o mouse.

### Objetivo

Seu objetivo no jogo � capturar a bandeira inimiga.

### Ex�rcito

Seu ex�rcito ser� escolhido aleat�riamente no in�cio de cada partida.
Como regra, o ex�rcito vermelho sempre ter� o primeiro movimento da partida.

### Turnos

Existem tr�s turnos no jogo:

- **Turno de posicionamento** das pe�as s� existe no in�cio da partida.
Voc� deve trocar suas pe�as de posi��o para proteger sua bandeira e avan�ar sobre os inimigos.
Este turno s� acaba quando os dois jogadores d�o pronto;
- **Turno de movimenta��o**: O jogador da vez movimenta uma pe�a ou ataca;
- **Turno de ataque**: Quando a pe�a que o jogador movimentou pode atacar alguma pe�a,
o jogador tem o direito de escolha de ataca-la ou n�o.

### Movimenta��o

Todas as pe�as de valores 1 � 10 podem se movimentar.
As pe�as que se movimentam s� podem andar para quadrados adjacentes.

O `(2) Soldado` que pode escolher uma dire��o e andar quantos quadrados quiser at� um obst�culo.
Contudo, ao fazer isso ele perde a oportunidade de atacar em seguida e deve passar o turno pro outro jogador.

### Ataque

Para atacar, o advers�rio deve, com uma pe�a selecionada, selecionar uma pe�a advers�ria num quadrado adjacente.

Ganha a pe�a que tiver maior n�mero, salvo algumas exe��es:

- A pe�a atacada � a `(!) Bandeira` -> quem ataca ganha;
- A pe�a atacada � uma `(-) Bomba` -> ambas pe�as saem da partida
    - Se for um `(3) Cabo-armeiro`, ele desarma a bomba e apenas a bomba sai.
- Se um `(1) Espi�o` ataca o `(10) Marechal`, o Marechal sai.

### Como vencer

Existem 3 formas da partida terminar:

- Jogador capturou a `(!) Bandeira` (quem capturou ganha);
- Jogador ficou sem movimentos dispon�veis (o outro ganha);
- Jogador saiu (o outro ganha)

H� a possibilidade tamb�m de haver um empate caso os dois jogadores fiquem sem movimentos.