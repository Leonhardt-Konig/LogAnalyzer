# LogAnalyzer

Um programa para ler e analizar arquivos de texto, com mais ênfase em arquivos .log.

# Principais Funcionalidades:
1-Abrir arquivos de texto razoavelmente grandes com pouco lag;
2-Função de pesquisa arbitrária para encontrar palavras ou termos específicos;
3-Renderização de texto com demanda;
4-Uso de WPF para mostrar o conteúdo em uma UI simples;

#Detalhes extras:

Arquivos de texto serão lidos e um offset de bytes para cada linha é criado. 
Usando esse offset de bytes, cada linha que será renderizada no começo é inserida num buffer que contém 100 items inicialmente.
Rolar para baixo para revelar mais items acontece com a demanda após calcular o número de itens necessários para cobrir o número de linhas visíveis.
Isso permite uma experiência com poucas instâncias de lag.

