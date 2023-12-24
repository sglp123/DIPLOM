using System.Globalization;
using System.Numerics;
using VKR.structures;
namespace MathObjects;

public class GlobalVector
{
    private readonly double[]? _values;

    public static GlobalVector operator -(GlobalVector gv1, GlobalVector gv2)
    {
        if (gv1.Size != gv2.Size)
            throw new Exception("Найти разность векторов не возможно, т.к. они имеют разные размеры.");

        GlobalVector gv = new(gv1.Size);
        for (int i = 0; i < gv.Size; i++)
            gv[i] = gv1[i] - gv2[i];

        return gv;
    }

    public static GlobalVector operator +(GlobalVector gv1, GlobalVector gv2)
    {
        if (gv1.Size != gv2.Size)
            throw new Exception("Найти разность векторов не возможно, т.к. они имеют разные размеры.");

        GlobalVector gv = new(gv1.Size);
        for (int i = 0; i < gv.Size; i++)
            gv[i] = gv1[i] + gv2[i];

        return gv;
    }

    public static GlobalVector operator *(double a, GlobalVector gv)
    {
        GlobalVector _gv = new(gv.Size);
        for (int i = 0; i < gv.Size; i++)
            _gv[i] = a * gv[i];
        return _gv;
    }

    public static double operator *(GlobalVector gv1, GlobalVector gv2)
    {
        if (gv1.Size != gv2.Size) throw new Exception("Невозможно найти скалярное умножение векторов, из-за разности в размерах.");
        double ans = 0.0D;
        for (int i = 0; i < gv1.Size; i++)
            ans += gv1[i] * gv2[i];
        return ans;
    }

    public double Norma()
    {
        if (_values == null) throw new Exception("_values is null!");
        double ans = 0.0D;
        foreach (var v in _values)
            ans += v * v;
        return Math.Sqrt(ans);
    }

    // ? Нужен ли public?
    public GlobalVector(int size)
    {
        _values = new double[size];
    }

    public int Size => _values.Length;

    public double this[int index]
    {
        get => _values[index];
        set => _values[index] = value;
    }

    // ? Удалить после.
    public GlobalVector()
    {

    }

    public GlobalVector(double[] arr)
    {
        _values = arr;
    }

    public override string ToString()
    {
        if (_values is null) throw new Exception("_values is null");
        string result = "";
        foreach (var item in _values)
            result += $"{item.ToString("E15", CultureInfo.InvariantCulture)}\n";
        return result;
    }

    private static double U1(Point p) => p.Z * p.Z;

    //public void ConsiderBoundaryConditions(ArrayOfBorders arrBrd, ArrayOfPoints arrp)
    //{
    //    if (_values is null) throw new Exception("_values is null");
    //    foreach (var border in arrBrd)
    //        switch (border[0])
    //        {
    //            case 1:
    //                //for (int i = 2; i < 4; i++)
    //                //    _values[border[i]] = 0;
    //                switch (border[1])
    //                {
    //                    case 1:
    //                        for (int i = 2; i < 4; i++)
    //                            _values[border[i]] = 0.0D;
    //                        break;

    //                    case 2:
    //                    case 4:
    //                        for (int i = 2; i < 4; i++)
    //                            _values[border[i]] = U1(arrp[border[i]]);
    //                        break;

    //                    case 3:
    //                        for (int i = 2; i < 4; i++)
    //                            _values[border[i]] = 10000.0D;
    //                        break;

    //                    default:
    //                        throw new Exception("No such bord");
    //                }
    //                break;
    //            case 2:
    //                for (int i = 2; i < 4; i++)
    //                    _values[border[i]] += 0.0D;
    //                break;
    //            case 3: throw new ArgumentException("Пока нет возможности учитывать КУ 3-го рода");
    //        }
    //}

    public GlobalVector(ArrayOfElems arrEl, ArrayOfPoints arrPt)
    {
        _values = new double[arrPt.Length];

        foreach (var elem in arrEl)
        {
            var lv = new LocalVector(elem, arrPt);
            for (int i = 0; i < 4; i++)
            {
                _values[elem[i]] += lv[i];
            }
        }
        //ConsiderBoundaryConditions(arrBr, arrPt);
    }
}
