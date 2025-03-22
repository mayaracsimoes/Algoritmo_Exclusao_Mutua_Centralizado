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
using System.Threading;

class Processo
{
    public int Id { get; }
    public DateTime CriadoEm { get; }

    public bool EhCoordenador { get; set; }

    public Processo(int id)
    {
        Id = id;
        CriadoEm = DateTime.Now;
        EhCoordenador = false;
    }
}

class Coordenador
{
    private static Processo _processoCoordenador;
    private static readonly object _lock = new object();
    private List<Processo> _fila = new List<Processo>();
    private List<bool> _recursosDisponiveis = new List<bool> { true, true }; // Dois recursos
    private Random _random = new Random();
   
    public Coordenador() { }

    public bool TemCoordenador()
    {
        return _processoCoordenador != null;
    }
    public void AdicionarProcesso(Processo processo)
    {
        lock (_lock)
        {
            if (!_fila.Contains(processo) && !processo.EhCoordenador)
            {
                _fila.Add(processo);
                Console.WriteLine($"[{DateTime.Now}] Processo {processo.Id} adicionado à fila.");
                MostrarFila();
            }
        }
    }

    public void ProcessarFila()
    {
        lock (_lock)
        {
            for (int i = 0; i < _recursosDisponiveis.Count; i++)
            {
                if (_recursosDisponiveis[i] && _fila.Count > 0)
                {
                    var processo = _fila[0];
                    _fila.RemoveAt(0);
                    _recursosDisponiveis[i] = false; // Recurso ocupado
                    Console.WriteLine($"[{DateTime.Now}] Processo {processo.Id} está acessando o recurso {i + 1}.");
                    MostrarFila();

                    // Inicia uma thread para liberar o recurso após o processamento
                    int recursoIndex = i; // Captura o índice correto
                    new Thread(() => LiberarRecurso(processo, recursoIndex)).Start();
                }
            }
        }
    }

    private void LiberarRecurso(Processo processo, int recursoIndex)
    {
        Thread.Sleep(_random.Next(5000, 15000)); // Simula o tempo de processamento
        lock (_lock)
        {
            // Verifica se o índice é válido
            if (recursoIndex >= 0 && recursoIndex < _recursosDisponiveis.Count)
            {
                _recursosDisponiveis[recursoIndex] = true; // Recurso liberado
                Console.WriteLine($"[{DateTime.Now}] Processo {processo.Id} liberou o recurso {recursoIndex + 1}.");
                MostrarFila();
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}] Erro: Índice de recurso inválido ({recursoIndex}).");
            }
        }
    }

    public void Morrer()
    {
        lock (_lock)
        {
            _fila.Clear();
            _recursosDisponiveis = new List<bool> { true, true }; // Reseta os recursos
            Console.WriteLine($"[{DateTime.Now}] Coordenador morreu. Fila limpa e recursos resetados.");
            _processoCoordenador = null;
        }
    }

    public void EscolherNovoCoordenador(List<Processo> processos, List<Thread> processosThreads)
    {
        lock (_lock)
        {
            if (processos.Count == 0)
                return;

            int index = _random.Next(processos.Count);
            Processo novoCoordenador = processos[index];

            _processoCoordenador = novoCoordenador;
            processos[index].EhCoordenador = true;
            // Encontra e interrompe a thread do novo coordenador
            if (index != -1 && index < processosThreads.Count)
            {
                processosThreads[index].Interrupt();
                Console.WriteLine($"[{DateTime.Now}] Thread do Processo {novoCoordenador.Id} interrompida (agora é coordenador).");
            }

            processos.RemoveAt(index);

            Console.WriteLine($"[{DateTime.Now}] Novo coordenador escolhido: Processo {novoCoordenador.Id}");
        }
    }

    public void MostrarFila()
    {
        lock (_lock)
        {
            Console.WriteLine($"[{DateTime.Now}] Estado da fila:");
            if (_fila.Count == 0)
            {
                Console.WriteLine("Fila vazia.");
            }
            else
            {
                foreach (var processo in _fila)
                {
                    Console.WriteLine($"- Processo {processo.Id} (Criado em: {processo.CriadoEm})");
                }
            }

            Console.WriteLine($"[{DateTime.Now}] Estado dos recursos:");
            for (int i = 0; i < _recursosDisponiveis.Count; i++)
            {
                Console.WriteLine($"- Recurso {i + 1}: {(_recursosDisponiveis[i] ? "Disponível" : "Ocupado")}");
            }
        }
    }
}

class Program
{
    private static List<Processo> _processos = new List<Processo>();
    private static Random _random = new Random();
    private static int _nextId = 1;
    static DateTime _ultimaMorteCoordenador = DateTime.Now; // Armazena o momento da última morte

    public static Coordenador _coordenador = new Coordenador();
    public static List<Thread> _processoThreads = new List<Thread>();

    static void Main(string[] args)
    {

        Thread criarProcessosThread = new Thread(() => CriarProcessos(true));
        criarProcessosThread.Start();

        Thread coordenadorThread = new Thread(RunCoordenador);
        coordenadorThread.Start();

        while (true)
        {
            Thread.Sleep(1000);
        }
    }

    static void RunCoordenador()
    {
        while (true)
        {
            Thread.Sleep(1000); // Verifica a fila a cada 1 segundo
            _coordenador.ProcessarFila();

            // Verifica se 1 minuto se passou desde a última morte
            if ((DateTime.Now - _ultimaMorteCoordenador).TotalMinutes >= 1)
            {
                _coordenador.Morrer();
                _ultimaMorteCoordenador = DateTime.Now; // Atualiza o momento da última morte
            }

            if (!_coordenador.TemCoordenador())
            {
                _coordenador.EscolherNovoCoordenador(_processos, _processoThreads);

            }
        }
    }

    static void CriarProcessos(bool PrimeiraExecucao)
    {
        while (true)
        {
            if (!PrimeiraExecucao)
            {

                Thread.Sleep(40000); // Cria um novo processo a cada 40 segundos
            }

            int id = _nextId++;
            var processo = new Processo(id);

            _processos.Add(processo);
            Thread processoThread = new Thread(() => RunProcesso(processo));
            _processoThreads.Add(processoThread); // Armazena a thread na lista
            processoThread.Start();
            PrimeiraExecucao = false;

            Console.WriteLine($"[{DateTime.Now}] Processo {processo.Id} criado.");

        }
    }

    static void RunProcesso(Processo processo)
    {
        while (true)
        {
            try
            {
                Thread.Sleep(_random.Next(10000, 25000)); // Tenta acessar o recurso a cada 10-25 segundos
                _coordenador.AdicionarProcesso(processo);
            }
            catch (ThreadInterruptedException)
            {
                // Captura a interrupção da thread
                //Console.WriteLine($"[{DateTime.Now}] Processo {processo.Id} foi interrompido ao se tornar coordenador.");
                break; // Sai do loop para encerrar a thread
            }

        }
    }
}