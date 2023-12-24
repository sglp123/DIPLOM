using System.Collections.Immutable; //для неизменяемых массивов
using VKR.structures;

namespace VKR;

public class Mesh
{
    private const string _POINTSPATH = @"C:\Users\ЛаринМаркВячеславови\Desktop\Diplom\Diplom\elems_points\Points.txt";
    private const string _ELEMSPATH = @"C:\Users\ЛаринМаркВячеславови\Desktop\Diplom\Diplom\elems_points\Elems.txt";

    public int NodesAmountTotal // Количество узлов в области.
    {
        get => NodesAmountR * NodesAmountZ;
    }
    public int NodesAmountR // Количество узлов по оси R.
    {
        get => _nodesR.Count;
    }

    List<int> _nodesR_Refs;

    private ImmutableArray<double> _nodesRWithoutFragmentation { get; set; } // Узлы по R без разбиений.
    private List<double> _nodesR; // Массив узлов по R со всеми разбиениями.
    private string? _infoAboutR;  // [Считывается] Информация по количеству разбиений и кэфу разрядки по R.

    public int NodesAmountZ // Количество узлов по оси Z.
    {
        get => _nodesZ.Count;
    }

    List<int> _nodesZRefs;

    int ElemsAmount;

    private ImmutableArray<double> _nodesZWithoutFragmentation { get; set; } // Узлы по Z без разбиений.
    private List<double> _nodesZ; // Массив узлов по Z со всеми разбиениями.
    private string? _infoAboutZ; // [Считывается] Информация по количеству разбиений по Z.
    public List<int> Elems; // Перечень всех элементов по 4-ём точкам.
    private int _bordersAmount; // Количество границ.
    private List<List<int>> _borders; // Массив всех границ.

    private void RemakeBorders()
    {
        foreach (var border in _borders)
        {
            for (int i = 2; i < border.Count; i++)
            {
                switch (i)
                {
                    case 2 or 3:
                        border[i] = _nodesR_Refs[border[i]];
                        break;
                    case 4 or 5:
                        border[i] = _nodesZRefs[border[i]];
                        break;
                }
            }
        }
    }

    public void GenerateGrid()
    {
        int currentPosition = 0;

        int addNodesR = int.Parse(_infoAboutR.Split()[0]); // кол-во разбиений по R
        double kR = double.Parse(_infoAboutR.Split()[1]); // разрядка по R

        double h = _nodesR[1 + currentPosition] - _nodesR[currentPosition]; // числитель
        double denominator = 0.0;

        for (int i = 0; i < addNodesR; i++)
            denominator += Math.Pow(kR, i);

        double h0 = h / denominator; // начальный шаг

        for (int i = 0; i < addNodesR - 1; i++) // генерируем все узлы по R
        {
            _nodesR.Insert(currentPosition + 1, _nodesR[currentPosition] + h0 * Math.Pow(kR, i));
            currentPosition++;
        }
        currentPosition++;
        _nodesR_Refs[^1] = currentPosition;

        /*--- то же самое по Z ---*/

        currentPosition = 0;
        int addNodesZ = int.Parse(_infoAboutZ.Split()[0]);
        double kZ = double.Parse(_infoAboutZ.Split()[1]);

        h = _nodesZ[1 + currentPosition] - _nodesZ[currentPosition];
        denominator = 0.0;

        for (int j = 0; j < addNodesZ; j++)
            denominator += Math.Pow(kZ, j);
        h0 = h / denominator;

        for (int j = 0; j < addNodesZ - 1; j++)
        {
            _nodesZ.Insert(currentPosition + 1, _nodesZ[currentPosition] + h0 * Math.Pow(kZ, j));
            currentPosition++;
        }
        currentPosition++;
        _nodesZRefs[^1] = currentPosition;

        RemakeBorders();
    }

    /// <summary>
    /// Метод, выводящий координаты всех точек в расчетной области в файл Points.dat.
    /// </summary>
    public void OutPutPoints()
    {
        using var sw = new StreamWriter(_POINTSPATH);
        sw.WriteLine(NodesAmountTotal);
        foreach (var Z in _nodesZ)
            foreach (var R in _nodesR)
                sw.WriteLine($"{SetPointType(new Point(R, Z))}");
    }

    /// <summary>
    /// Метод, устанавливающий тип точки.
    /// </summary>
    /// <param name="point">Точка</param>
    private Point SetPointType(Point point)
    {
        if (_nodesRWithoutFragmentation[0] <= point.R && point.R <= _nodesRWithoutFragmentation[1] && // проверка на граничную или
            _nodesZWithoutFragmentation[0] >= point.Z && point.Z >= _nodesZWithoutFragmentation[1])   // внешнюю точку (можно будет убрать)
        {
            int minValue = 4;
            foreach (var arr in _borders)
            {
                if (_nodesR[arr[2]] <= point.R && point.R <= _nodesR[arr[3]] && 
                    _nodesZ[arr[4]] >= point.Z && point.Z >= _nodesZ[arr[5]] &&
                    arr[0] < minValue)
                {
                    minValue = arr[0];
                    break;
                }
            }
            switch (minValue)
            {
                case 1: { point.Type = Location.BI; break; }
                default: { point.Type = Location.Inside; point.SubElemNum = 0; break; }
            }
        }
        else point.Type = Location.OutSide;
        return point;
    }

    // TODO: сделать генерацию, которая сможет склеить множество элементов
    /// <summary>
    /// Метод, генерирующий массив элементов.
    /// </summary>
    public void GenerateElemsArray()
    {
        using var sw = new StreamWriter(_ELEMSPATH);
        sw.WriteLine((_nodesR.Count - 1) * (_nodesZ.Count - 1));
        for (int i = 0; i < _nodesZ.Count - 1; i++)
            for (int j = 0; j < _nodesR.Count - 1; j++)
                sw.WriteLine($"{i * _nodesR.Count + j} {i * _nodesR.Count + j + 1} {(i + 1) * _nodesR.Count + j} {(i + 1) * _nodesR.Count + j + 1}");
    }

    public void ReadFrom(string path1, string path2)
    {
        string currPath = path1;
        try
        {
            using (var sr = new StreamReader(currPath)) // Считывание параметров для расчетной области.
            {
                _nodesR = sr.ReadLine().Split().Select(double.Parse).ToList();
                for (int i = 0; i < _nodesR.Count; i++)
                    _nodesR_Refs.Add(i);
                _nodesRWithoutFragmentation = _nodesR.ToImmutableArray();
                _infoAboutR = sr.ReadLine() ?? "";

                _nodesZ = sr.ReadLine().Split().Select(double.Parse).ToList();
                for (int i = 0; i < _nodesZ.Count; i++)
                    _nodesZRefs.Add(i);
                _nodesZWithoutFragmentation = _nodesZ.ToImmutableArray();
                _infoAboutZ = sr.ReadLine() ?? "";

                ElemsAmount = 1;
                Elems = [1, 0, 1, 0, 1];
            }

            // Считывание данных для границ.
            currPath = path2;
            using (var sr = new StreamReader(currPath))
            {
                _bordersAmount = int.Parse(sr.ReadLine() ?? "0");
                for (int i = 0; i < _bordersAmount; i++)
                    _borders.Add(sr.ReadLine().Split().Select(int.Parse).ToList());

            }
        }
        catch (IOException ex)  // Ошибка при считывание данных для границ.
        {
            Console.WriteLine($"Error during reading {currPath} file: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Конструктор класса 2D-сетки.
    /// </summary>
    public Mesh()
    {
        _borders = new();
        Elems = new();
        _nodesZ = new();
        _nodesR = new();
        _nodesR_Refs = new();
        _nodesZRefs = new();
    }
}
