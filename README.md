# Algoritmo de Exclusão Mútua Centralizado

Este projeto foi desenvolvido durante a disciplina de Sistemas Distribuídos em 2025/01 da FURB. Ele implementa um algoritmo de exclusão mútua centralizado em C# utilizando threads. O sistema simula um coordenador que gerencia o acesso a recursos compartilhados por vários processos. Cada processo tenta acessar os recursos em intervalos aleatórios, e o coordenador garante que apenas um processo por vez acesse cada recurso.

## Funcionalidades

- **Coordenador Centralizado**: Gerencia o acesso a dois recursos compartilhados.
- **Processos**: São criados dinamicamente a cada 40 segundos e tentam acessar os recursos em intervalos aleatórios entre 10 - 25 segundos.
- **Fila de Processos**: Mantém uma fila de processos que desejam acessar os recursos.
- **Logs Detalhados**: Exibe o estado da fila e dos recursos em tempo real.
- **Simulação de Falhas**: O coordenador "morre" a cada 1 minuto, e um novo coordenador é escolhido aleatoriamente.

## Requisitos

- **.NET SDK**: Certifique-se de ter o .NET SDK instalado. Você pode baixá-lo [aqui](https://dotnet.microsoft.com/download).
- **IDE**: Recomenda-se o uso do Visual Studio ou Visual Studio Code.

## Como Executar
**Clone o Repositório**:   
   git clone https://github.com/mayaracsimoes/Algoritmo_Exclusao_Mutua_Centralizado.git
   cd exclusao-mutua-centralizada
   
## Gerenciador de Processos e Recursos
Este projeto gerencia processos e o acesso a recursos utilizando um coordenador que pode falhar e ser substituído dinamicamente.

## Como Compilar e Executar

### Compilar o Projeto:
```bash
dotnet build
```

### Executar o Projeto:
```bash
dotnet run
```

## Exemplo de Saída no Console

```bash
[21/03/2025 19:19:47] Processo 1 criado.
[21/03/2025 19:20:00] Coordenador morreu. Fila limpa e recursos resetados.
[21/03/2025 19:20:00] Novo coordenador escolhido: Processo 1
[21/03/2025 19:20:09] Processo 1 adicionado à fila.
[21/03/2025 19:20:09] Estado da fila:
- Processo 1 (Criado em: 21/03/2025 19:19:47)
[21/03/2025 19:20:09] Estado dos recursos:
- Recurso 1: Disponível
- Recurso 2: Disponível
[21/03/2025 19:20:10] Processo 1 está acessando o recurso 1.
[21/03/2025 19:20:10] Estado da fila:
Fila vazia.
[21/03/2025 19:20:10] Estado dos recursos:
- Recurso 1: Ocupado
- Recurso 2: Disponível
[21/03/2025 19:20:28] Processo 2 criado.
[21/03/2025 19:20:33] Processo 1 adicionado à fila.
[21/03/2025 19:20:33] Estado da fila:
- Processo 1 (Criado em: 21/03/2025 19:19:47)
[21/03/2025 19:20:33] Estado dos recursos:
- Recurso 1: Ocupado
- Recurso 2: Disponível
[21/03/2025 19:20:33] Processo 1 está acessando o recurso 2.
[21/03/2025 19:20:33] Estado da fila:
Fila vazia.
[21/03/2025 19:20:33] Estado dos recursos:
- Recurso 1: Ocupado
- Recurso 2: Ocupado
```

## Estrutura do Código

- **Processo.cs**: Representa um processo com um ID único e um timestamp de criação.
- **Coordenador.cs**: Gerencia a fila de processos e o acesso aos recursos.
- **Program.cs**: Contém a lógica principal do programa, incluindo a criação de threads para processos e o coordenador.

## Licença

Este projeto está licenciado sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.


