
using System.Diagnostics;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        // принимаем только 1 аргумент на вход - имя файла
        if (args.Length != 1)
        {
            Console.WriteLine("Please, enter just one input file");
            return;
        }
        // проверка на существование файла по указанному пути
        string input = args[0];
        if (File.Exists(input) == false)
        {
            Console.WriteLine("Your file doesnt exist! :(");
            return;
        }
        // объекты для измерения эффективности (скорости выполнения) методов
        Stopwatch norm = new Stopwatch();
        Stopwatch parallel = new Stopwatch();

        // в пердыдущей домашке я считывал строки в листы, а тут, я сдуру решил
        // представить данные в виде массива строк. Эффективность обработки данных увеличилась!
        var fContent = File.ReadAllLines(input);
        var obj = new ClassLibrary1.Class1();

        // манипуляции для вызова приватного метода
        Object[] arguments = { fContent };
        var type = typeof(ClassLibrary1.Class1);
        var m_info = type.GetMethod("Parse", BindingFlags.NonPublic | BindingFlags.Instance);

        //Обрабатываем без параллельности
        norm.Start();
        var dict = (Dictionary<string, int>)m_info.Invoke(obj, arguments);
        norm.Stop();

        //Обрабатываем с параллельностью
        parallel.Start();
        var dictConcurrent = obj.ParallelParse(fContent)/*.ToDictionary(dict=>dict.Key, dict=>dict.Value)*/;
        parallel.Stop();

        var sort_words = dictConcurrent.OrderByDescending(_ => _.Value);
        using (StreamWriter file = new StreamWriter("output.txt"))
        {
            foreach (var row in sort_words)
            {
                file.WriteLine($"{row.Key} {row.Value}");
            }
        }

        Console.WriteLine("Complited!");
        Console.WriteLine("Parallel: {0}", parallel.ElapsedMilliseconds);
        Console.WriteLine("NORMAL: {0}", norm.ElapsedMilliseconds);
    }

    /*
     * P.S
     * Файл, размером 2,7 мб, содержащий томик "Войны и мира", параллельным методом обрабатывается чуть быстрее.
     * А Файл, размером 100 мб обычным способом обрабатывался быстрее, но я думаю, это потому
     * что он состоял из одних нулей!
     */
}