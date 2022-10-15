using Microsoft.VisualBasic;
using System.Collections.Concurrent;

namespace ClassLibrary1
{
    public class Class1
    {
        private
        Dictionary<string, int> Parse(string[] fContent)
        {
            // в мапу (словарь) запишем слова и их количество
            Dictionary<string, int> dict = new Dictionary<string, int>();
            //в массив запишем все символы, которые не могут войти в состав слова (возможно, сюда надо что-то еще добавить)
            char[] delimeters = { ')', '(', ']', '[', '{', '}', ' ', ',', '.', '!', '?', ':', ';', '-', '—' };

            foreach (var str in fContent)
            {
                // в цикле проходим по всем листам, сплитим строки файла по указанным символам.
                // Если слова нет в словаре, то добавляем его туда. Если есть, то +1 к счетчику этого слова
                var words = str.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    if (dict.TryAdd(word, 1) == false)
                    {
                        dict[word]++;
                    }
                }
            }
            return dict;
        }

        public
        ConcurrentDictionary<string, int> ParallelParse(string[] fContent)
        {
            /*int concurrencyLvl = Environment.ProcessorCount;
            int startCapacity = 1000;*/
            ConcurrentDictionary<string, int> dict = new ConcurrentDictionary<string, int>();
            // Насколько я понял, нет смысла в использовании объекта "Таск",
            // так как "Parallel.ForEach" сам подрубает нужное кол-во потоков
            /*Task kek = Task.Run(() =>
            {*/
                char[] delimeters = { ')', '(', ']', '[', '{', '}', ' ', ',', '.', '!', '?', ':', ';', '-', '—' };
                Parallel.ForEach(fContent, delegate (string str)
                {
                    var words = str.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in words)
                    {
                        // если value++, то в функцию залетит только value!
                        // Насколько я понял, это конкуррентно безопасное комбо методов "TryAdd" & "TryUpdate"
                        dict.AddOrUpdate(word, 1, (key, value) => ++value);
                    }
                });
            /*});
            kek.Wait();*/

            return dict;
        }
    }
}