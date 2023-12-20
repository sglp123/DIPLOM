using System.Collections.Immutable;
namespace Diplom;

public class Mesh
{
    // ? Надо было бы закинуть это в относительный путь, причем сделать это галантно
    // Относительный путь до файла с перечнем точек расчетной области.
    private const string _POINTSPATH = @"C:\Users\ЛаринМаркВячеславови\Desktop\Diplom\Diplom\elems_points\Points.txt";

    // Относительный путь до файла с перечнем элементов расчетной области.
    private const string _ELEMSPATH = @"C:\Users\ЛаринМаркВячеславови\Desktop\Diplom\Diplom\elems_points\Elems.txt";

    // Количество узлов в области.
    public int NodesAmountTotal
    {
        get => NodesAmountR * NodesAmountZ;
    }

    // Количество элементов в фигуре.
    public int ElemsAmount { get; set; }

    // Количество узлов по оси R.
    public int NodesAmountR
    {
        get => _nodesR.Count;
    }

    List<int> _nodesR_Refs;

    // TODO: Постараться избавиться от этих массивов и изменить массивы ссылок на элементы.
    // Границы по оси X без разбиений.
    private ImmutableArray<double> _nodesRWithoutFragmentation { get; set; }

    // Массив границ по оси X.
    private List<double> _nodesR;

    // Массив количества разбиений по элементам для oX.
    private string? _infoAboutR;




    // Количество разбиений по оси Z.
    public int NodesAmountZ
    {
        get => _nodesZ.Count;
    }

    List<int> _nodesZRefs;

    // TODO: Постараться избавиться от этих массивов и изменить массивы ссылок на элементы
    // Границы по оси Z без разбиений.
    private ImmutableArray<double> _nodesZWithoutFragmentation { get; set; }

    // Массив границ по оси Z.
    private List<double> _nodesZ;

    private string? _infoAboutZ;


    // Перечень границ всех элементов.
    public List<List<int>> Elems;


    // Количество границ.
    private int _bordersAmount;

    // Массив всех границ.
    private List<List<int>> _borders;

    private void RemakeBorders()
    {
        foreach (var border in _borders)
        {
            for (int i = 2; i < border.Count; i++)
            {
                switch (i)
                {
                    case 2:
                    case 3:
                        border[i] = _nodesR_Refs[border[i]];
                        break;
                    case 4:
                    case 5:
                        border[i] = _nodesZRefs[border[i]];
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Метод, создающий множество точек в необходимых границах.
    /// </summary>
    public void GenerateGrid()
    {
        int currentPosition = 0;
        double[] php = _infoAboutR.Split().Select(double.Parse).ToArray();
        for (int i = 0; i < _nodesRWithoutFragmentation.Length - 1; i++)
        {
            double h = _nodesR[1 + currentPosition] - _nodesR[currentPosition];
            double denominator = 0.0;

            int perem = Convert.ToInt32(php[2 * i]);
            for (int j = 0; j < perem; j++)
                denominator += Math.Pow(php[2 * i + 1], j);

            double x0 = h / denominator;

            for (int j = 0; j < perem - 1; j++)
            {
                _nodesR.Insert(currentPosition + 1, _nodesR[currentPosition] + x0 * Math.Pow(php[2 * i + 1], j));
                currentPosition++;
            }
            currentPosition++;
            _nodesR_Refs[i + 1] = currentPosition;
        }

        currentPosition = 0;
        php = _infoAboutZ.Split().Select(double.Parse).ToArray();
        for (int i = 0; i < _nodesZWithoutFragmentation.Length - 1; i++)
        {
            double h = _nodesZ[1 + currentPosition] - _nodesZ[currentPosition];
            double denominator = 0.0;

            int perem = Convert.ToInt32(php[2 * i]);
            for (int j = 0; j < perem; j++)
                denominator += Math.Pow(php[2 * i + 1], j);
            double x0 = h / denominator;

            for (int j = 0; j < perem - 1; j++)
            {
                _nodesZ.Insert(currentPosition + 1, _nodesZ[currentPosition] + x0 * Math.Pow(php[2 * i + 1], j));
                currentPosition++;
            }
            currentPosition++;
            _nodesZRefs[i + 1] = currentPosition;

        }
        RemakeBorders();
    }

    /// <summary>
    /// Метод, выводящий координаты всех точек в расчетной области в файл Points.dat.
    /// </summary>
    public void OutPutPoints()
    {
        using var sw = new StreamWriter(_POINTSPATH);
        sw.WriteLine(NodesAmountTotal);
        int i = 0;
        foreach (var Z in _nodesZ)
            foreach (var R in _nodesR)
            {
                sw.WriteLine($"{SetPointType(new Point(R, Z))}");
                i++;
            }
    }

    // ? Убедиться в правильности порядка учета краевых условий.
    /// <summary>
    /// Метод, устанавливающий тип точки.
    /// </summary>
    /// <param name="pnt">Точка</param>
    private Point SetPointType(Point pnt)
    {
        for (int i = 0; i < Elems.Count; i++)
        {
            if (_nodesRWithoutFragmentation[Elems[i][1]] <= pnt.R && pnt.R <= _nodesRWithoutFragmentation[Elems[i][2]] &&
                _nodesZWithoutFragmentation[Elems[i][3]] >= pnt.Z && pnt.Z >= _nodesZWithoutFragmentation[Elems[i][4]])
            {
                int minValue = 4;
                foreach (var arr in _borders)
                {
                    if (_nodesR[arr[2]] <= pnt.R && pnt.R <= _nodesR[arr[3]] &&
                        _nodesZ[arr[4]] >= pnt.Z && pnt.Z >= _nodesZ[arr[5]] &&
                        arr[0] < minValue)
                    {
                        minValue = arr[0];
                        break;
                    }
                }
                switch (minValue)
                {
                    case 1: { pnt.Type = Location.BI; break; }
                    default: { pnt.Type = Location.Inside; pnt.SubElemNum = i; break; }
                }
            }
            else if (pnt.Type == Location.NotStated)
                pnt.Type = Location.OutSide;
        }
        return pnt;
    }

    // TODO: сделать генерацию, которая сможет склеить множество элементов
    /// <summary>
    /// Метод, генерирующий массив элементов.
    /// </summary>
    public void GenerateElemsArray()
    {
        using var sw = new StreamWriter(_ELEMSPATH);
        sw.WriteLine((_nodesR.Count - 1) * (_nodesZ.Count - 1));
        for (int k = 0; k < _nodesZ.Count - 1; k++)
            for (int i = 0; i < _nodesR.Count - 1; i++)
                sw.WriteLine($"{k * _nodesR.Count + i} {k * _nodesR.Count + i + 1} " +
                             $"{(k + 1) * _nodesR.Count + i} {(k + 1) * _nodesR.Count + i + 1}");
    }

    public void ReadFrom(string path1, string path2)
    {
        string _currPath = path1;
        try
        {
            // Считывание параметров для расчетной области.
            using (var sr = new StreamReader(_currPath))
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


                ElemsAmount = int.Parse(sr.ReadLine() ?? "0");
                for (int i = 0; i < ElemsAmount; i++)
                    Elems.Add(sr.ReadLine().Split().Select(int.Parse).ToList());
            }

            // Считывание данных для границ.
            _currPath = path2;
            using (var sr = new StreamReader(_currPath))
            {
                _bordersAmount = int.Parse(sr.ReadLine() ?? "0");
                for (int i = 0; i < _bordersAmount; i++)
                    _borders.Add(sr.ReadLine().Split().Select(int.Parse).ToList());

            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error during reading {_currPath} file: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Конструктор класса 3D-сетки.
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
