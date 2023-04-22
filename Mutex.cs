public class Mutex
{
    private readonly System.Threading.Mutex mutex = new System.Threading.Mutex();
    private int[] array;

    public void Run()
    {
        array = new int[10];

        ThreadPool.QueueUserWorkItem(GenerateArray);
        ThreadPool.QueueUserWorkItem(ModifyArray);
        ThreadPool.QueueUserWorkItem(FindMax);

        Console.ReadKey();
    }

    public void GenerateArray(object state)
    {
        Random random = new Random();
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = random.Next(10);
        }
        Console.WriteLine("Сгенерированные элементы: " + string.Join(", ", array));
    }

    public void ModifyArray(object state)
    {
        Random random = new Random();
        for (int i = 0; i < array.Length; i++)
        {
            mutex.WaitOne();

            int oldValue = array[i];
            int newValue = oldValue + random.Next(10);
            array[i] = newValue;

            Console.WriteLine($"Элемент {i} изменен на {newValue}");
            mutex.ReleaseMutex();
        }
    }

    public void FindMax(object state)
    {
        while (true)
        {
            mutex.WaitOne();
            bool modified = false;
            int max = int.MinValue;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > max)
                {
                    max = array[i];
                }
                if (array[i] >= 100)
                {
                    modified = true;
                }
            }
            mutex.ReleaseMutex();

            if (modified)
            {
                Console.WriteLine("Максимальное значение: " + max);
                break;
            }
        }
    }       
}

class ProgramMain
{
    public static void Main()
    {
        Mutex program = new Mutex();
        program.Run();
    }
}