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
class Processo
{
    public int Id { get; }
    public DateTime CriadoEm { get; }

    public Processo(int id)
    {
        Id = id;
        CriadoEm = DateTime.Now;
    }
}

class Coordenador
{
    private static Coordenador _coordenador;
    private static readonly object _lock = new object();
    private List<Processo> _fila = new List<Processo>();
    private List<bool> _recursosDisponiveis = new List<bool> { true, true }; // Dois recursos
    private Random _random = new Random();

    private Coordenador() { }

    public static Coordenador Instance
    {
        get
        {
            lock (_lock)
            {
                if (_coordenador == null)
                {
                    _coordenador = new Coordenador();
                }
                return _coordenador;
            }
        }
    }

    public void AdicionarProcesso(Processo processo)
    {
        lock (_lock)
        {
            if (!_fila.Contains(processo))
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
            _coordenador = null;
        }
    }

    public Processo EscolherNovoCoordenador(List<Processo> processos)
    {
        lock (_lock)
        {
            if (processos.Count == 0)
                return null;

            int index = _random.Next(processos.Count);
            return processos[index];
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

    static void Main(string[] args)
    {
        Thread coordenadorThread = new Thread(RunCoordenador);
        coordenadorThread.Start();

        Thread criarProcessosThread = new Thread(CriarProcessos);
        criarProcessosThread.Start();

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
            Coordenador.Instance.ProcessarFila();

            // Coordenador morre a cada 1 minuto
            if (DateTime.Now.Second % 60 == 0)
            {
                Coordenador.Instance.Morrer();

                var novoCoordenador = Coordenador.Instance.EscolherNovoCoordenador(_processos);
                if (novoCoordenador != null)
                {
                    Console.WriteLine($"[{DateTime.Now}] Novo coordenador escolhido: Processo {novoCoordenador.Id}");
                }
            }
        }
    }

    static void CriarProcessos()
    {
        while (true)
        {
            Thread.Sleep(40000); // Cria um novo processo a cada 40 segundos
            int id = _nextId++;
            var processo = new Processo(id);
            _processos.Add(processo);
            Console.WriteLine($"[{DateTime.Now}] Processo {processo.Id} criado.");

            Thread processoThread = new Thread(() => RunProcesso(processo));
            processoThread.Start();
        }
    }

    static void RunProcesso(Processo processo)
    {
        while (true)
        {
            Thread.Sleep(_random.Next(10000, 25000)); // Tenta acessar o recurso a cada 10-25 segundos
            Coordenador.Instance.AdicionarProcesso(processo);
        }
    }
}