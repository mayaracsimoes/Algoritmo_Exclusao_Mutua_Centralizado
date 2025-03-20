// Especificação:
// ▪ a cada 1 minuto o coordenador morre
// ▪ quando o coordenador morre, a fila também morre (o novo coordenador pode ser escolhido 
// de forma randomizada. Ou seja, não é necessário implementar um algoritmo de eleição)
// ▪ o tempo de processamento de um recurso é de 5 à 15 segundos
// ▪ os processos tentam consumir o(s) recurso(s) num intervalo de 10 à 25 segundos
// ▪ a cada 40 segundos um novo processo deve ser criado (ID randômico)
// ▪ dois processos não podem ter o mesmo ID

// simular o processo, mostrar os consoles, o que acontece com cada processo ao decorrer do tempo
// simular que tal processo está acessando recurso x e coloca na fila

// quando o coordenador morrer ele precisa avisar e gerar um random dos processos que existem
// os processos estarao numa lista de objetos que pertence ao coordenador 
// a lista deve contem o ID do processo e a data  hora que foi criado (eu acho)
// o coordenador pode ser de acesso global 
// o processo tenta a cada 10-25 segundos o recurso, so é adicionado na fila se ainda nao tiver

using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

static void Main(string[] args)
{
    Dictionary<int, Process> processosAbertos = new Dictionary<int, Process>();
    Process coordenador = new Process();
    do
    {
        Process processo = new Process();
        if (coordenador != null)
        {
            coordenador = processo;
            ChooseCoordenador(processosAbertos);
        }
        Thread begin = new Thread(processo.Executando());
        Thread.sleep(40000);
    }
    while (true);

    void ChooseCoordenador(processosAbertos)
    {
        ICollection<int> keys = processosAbertos.Keys();
        int chaveAleatoria = keys[random.Next(keys.Count)];
        processosAbertos[chaveAleatoria].ExecutandoCoordenador();

    }
}

class Process
{
    int id = new Random().NextInt();
    bool isCoordenador = false;

    public static void ExecutandoCoordenador()
    {
        isCoordenador = true;
        Console.WriteLine("Iniciando coordenador");
        Thread.sleep(60000);
        Console.WriteLine("Terminando!");
    }
}