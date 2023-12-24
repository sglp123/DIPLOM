namespace VKR.structures;

public class ArrayOfElems
{
    // Путь к файлу из которого берем локальные элементы.
    const string _ELEMPATH = @"C:\Users\ЛаринМаркВячеславови\Desktop\Diplom\Diplom\elems_points\Elems.txt";

    public List<List<int>> Arr = new(); // Массив всех локальных элементов.
    public int Length; // Длина массива ArrayOfElems.

    public MyEnumerator GetEnumerator() => new(this);

    /// <summary>
    /// Метод, возвращающий i-ый массив.
    /// </summary>
    /// <param name="i">Итератор.</param>
    /// <returns>Локальный элемент.</returns>
    public List<int> this[int i] => Arr[i];

    public class MyEnumerator
    {
        int nIndex;
        ArrayOfElems collection;
        public MyEnumerator(ArrayOfElems coll)
        {
            collection = coll;
            nIndex = -1;
        }

        public bool MoveNext()
        {
            nIndex++;
            return nIndex < collection.Arr.Count;
        }

        public List<int> Current => collection.Arr[nIndex];
    }

    /// <summary>
    /// Конструктор класса ArrayOfElems.
    /// </summary>
    public ArrayOfElems()
    {
        using var sr = new StreamReader(_ELEMPATH);
        Length = int.Parse(sr.ReadLine() ?? "0"); // если строчка NULL, то 0
        for (int i = 0; i < Length; i++)
            Arr.Add(sr.ReadLine().Split().Select(int.Parse).ToList());
    }
}