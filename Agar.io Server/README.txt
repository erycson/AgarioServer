Créditos:
	ExtremsX: Crackear cliente e Emulador do servidor
	Agar.io: Cliente

Fontes: 
	https://github.com/DnAp/Agar.io-Reverse-Engineering/blob/master/main_out.js
	https://github.com/Snowl/Petri/tree/master/src

Versão:
	main_out.js: 502
	quadtree.js: 3
	server: 5 (2015/05/21)

Erros:
	As vezes a celula consegue sair do reino
	Algumas comidas são criadas fora do reino
	De vez enquando está havendo uma travada no cliente

Implementar:
	A movimentação de quando a celula está dividida está ruim
	A celula não se divide quando bate num virus
	Quando a pessoa expeli uma massa, essa massa pode sair da arena
	O virus não cresce
	Mode Team
	Varios Packets ainda não foram criadas

Change log

Versão 5
	O Virus pode ser alimentado e se dividir
	Corrigido erro ao atualizar a quantidade de jogadores ficar negativo

Versão 4
	Cliente versão atualizado para 502
	Pequeno arquivo de configuração para o site
	Sistema de configuração do servidor alterado para arquivo .xml
	Otimização na movimentação das celulas, agora as celulas são agrupadas pelo dono, e os dados só de quem tiver perto que é enviado
	Correção na quantidade de jogadores jogando
	A taxa de atualização da movimentação foi alterada para 23 atualizações por segundo

Versão 3
	Uso da biblioteca Fleck (https://github.com/statianzo/Fleck) para aumento da compatibilidade

Versão 2
	Criado um sistema simples de LOG
	Agora uma celula pode comer a outra
	As skins foram adicionadas

Versão 1
	Movimentação de uma celula
	Expelir massa
	Dividir celula
	Comida 
	Comer comida
	Virus
	Ranking
