namespace HashLab;

//Один шаг пробирования: номер позиции и ключ, который там лежит (null = пусто)
public record ProbeStep(int Index, string? Key);

//Результат операции занесения/поиска.
public record OpResult(bool Ok, string Message, int Primary, List<ProbeStep> Steps, string? Info = null);

public record Collision(string Key, int Primary);

// Вариант 11: хеш-функция Л, открытое перемешивание, квадратичные пробы.
// Позиции вектора нумеруются 1..N.
public class OpenHashTable
{
    public const int N = 100; // длина вектора
    public const int M = 97;  // простое число, ближайшее к N (p < n), для операции mod в ХФ

    private readonly string?[] _keys = new string?[N + 1];
    private readonly string?[] _info = new string?[N + 1];
    private readonly List<Collision> _collisions = new();

    public int Count { get; private set; }
    public IReadOnlyList<Collision> Collisions => _collisions;
    public string? KeyAt(int i) => _keys[i];
    public string? InfoAt(int i) => _info[i];

    public static int HashL(string key)
    {
        int len = key.Length;
        return ((key[1] + key[len - 1]) * len) % M + 1;
    }

    ///Вторичный индекс: i_вт = (i_перв + j^2) mod n, j = 1,2,... (в нумерации 1..n).
    public static int Probe(int primary, int j) => (primary - 1 + j * j) % N + 1;

    private static int Index(int primary, int j) => j == 0 ? primary : Probe(primary, j);

    public OpResult Add(string key, string info)
    {
        if (key.Length < 2)
            return new(false, "Ключ должен содержать не менее 2 символов.", 0, new());
        int p = HashL(key);
        var steps = new List<ProbeStep>();
        if (Count >= N)
            return new(false, "Таблица переполнена.", p, steps);

        for (int j = 0; j < N; j++) // j < n
        {
            int idx = Index(p, j);
            steps.Add(new(idx, _keys[idx]));
            if (_keys[idx] == key)
                return new(false, $"Ключ '{key}' уже есть в таблице (позиция {idx}); ключи должны быть уникальны.", p, steps);
            if (_keys[idx] == null)
            {
                _keys[idx] = key;
                _info[idx] = info;
                Count++;
                if (j > 0) _collisions.Add(new(key, p));
                string msg = j == 0
                    ? $"Запись помещена в позицию {idx}."
                    : $"Коллизия! Запись помещена в позицию {idx} после {j} проб(ы).";
                return new(true, msg, p, steps);
            }
        }
        return new(false, "Свободная позиция не найдена среди проб (j < n), хотя в таблице могут быть свободные места.", p, steps);
    }

    public OpResult Find(string key)
    {
        if (key.Length < 2)
            return new(false, "Ключ должен содержать не менее 2 символов.", 0, new());
        int p = HashL(key);
        var steps = new List<ProbeStep>();
        for (int j = 0; j < N; j++)
        {
            int idx = Index(p, j);
            steps.Add(new(idx, _keys[idx]));
            if (_keys[idx] == key)
                return new(true, $"Запись найдена в позиции {idx}.", p, steps, _info[idx]);
            if (_keys[idx] == null)
                return new(false, $"Позиция {idx} пуста - записи с ключом '{key}' нет.", p, steps);
        }
        return new(false, "Запись не найдена (просмотрены все пробы).", p, steps);
    }

    public void Clear()
    {
        Array.Clear(_keys); Array.Clear(_info);
        _collisions.Clear(); Count = 0;
    }
}
