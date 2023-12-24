using System.Diagnostics;
using System.Drawing;
using VKR.structures;
namespace MathObjects;

public class GlobalMatrix
{
    // Массив индексов по i.
    private readonly int[]? _ig;

    // Массив индексов по j.
    private readonly List<int> _jg;

    // Диагональные элементы.
    private readonly double[]? _diag;

    // Элементы матрицы нижнего треугольника .
    private readonly double[]? _al;

    // Элементы матрицы верхнего треугольника.
    private readonly double[]? _au;

    public int[] IG { get => _ig ?? throw new Exception("IG array is not defined"); }

    public List<int> JG { get => _jg ?? throw new Exception("JG array is not defined"); }

    public double[] DIAG { get => _diag ?? throw new Exception("DIAG array is not defined"); }

    public double[] AL { get => _al ?? throw new Exception("AL array is not defined"); }

    public double[] AU { get => _au ?? throw new Exception("AU array is not defined"); }



    public int Size
    {
        get
        {
            if (_diag is null) throw new Exception("_diag is null");
            return _diag.Length;
        }
    }

    private double ReturanValueAL(int i, int j)
    {
        for (int ii = 0; ii < _ig[i + 1] - _ig[i]; ii++)
            if (_jg[_ig[i] + ii] == j)
                return _al[_ig[i] + ii];
        return 0.0D;
    }

    private double ReturanValueAU(int i, int j)
    {
        for (int ii = 0; ii < _ig[i + 1] - _ig[i]; ii++)
            if (_jg[_ig[i] + ii] == j)
                return _au[_ig[i] + ii];
        return 0.0D;
    }


    public double this[int i, int j]
    {
        get
        {
            if (i > _diag.Length || j > _diag.Length)
                throw new Exception("Index ran out of matrix.");

            switch (i - j)
            {
                case 0: return _diag[i];
                case < 0: return ReturanValueAU(j, i);
                case > 0: return ReturanValueAL(i, j);
            }
        }
    }

    // To delete.
    public GlobalMatrix(int[] ig, List<int> jg, double[] al, double[] diag, double[] au)
    {
        _ig = ig;
        _jg = jg;
        _al = al;
        _diag = diag;
        _au = au;
    }

    public GlobalMatrix(ArrayOfElems _arrOfElms, ArrayOfPoints _arrOfPnt)
    {
        _jg = new();
        _ig = new int[_arrOfPnt.Length + 1];
        _diag = new double[_arrOfPnt.Length];

        // Рабочий массив.
        List<List<int>> arr = new();

        // ! Дерьмодристный момент.
        for (int i = 0; i < _arrOfPnt.Length; i++)
            arr.Add(new List<int>());

        foreach (var _elem in _arrOfElms)
            foreach (var point in _elem)
                foreach (var pnt in _elem)
                    if (pnt < point && Array.IndexOf(arr[point].ToArray(), pnt) == -1)
                    {
                        arr[point].Add(pnt);
                        arr[point].Sort();
                    }

        _ig[0] = 0;
        for (int i = 0; i < _arrOfPnt.Length; i++)
        {
            _ig[i + 1] = _ig[i] + arr[i].Count;
            _jg.AddRange(arr[i]);
        }

        _al = new double[_jg.Count];
        _au = new double[_jg.Count];
        foreach (var _elem in _arrOfElms)
            Add(_elem, _arrOfPnt);

        //ConsiderBoundaryConditions(_arrOfPnt);
    }

    public static GlobalVector operator *(GlobalMatrix _gm, GlobalVector _gv)
    {
        if (_gm.Size != _gv.Size)
            throw new Exception("Невозможно перемножить матрицу на вектор.");

        GlobalVector ans = new(_gv.Size);


        for (int i = 0; i < _gm.Size; i++)
        {
            for (int j = 0; j < _gm.IG[i + 1] - _gm.IG[i]; j++)
                ans[i] += _gm.AL[_gm.IG[i] + j] * _gv[_gm.JG[_gm.IG[i] + j]];
            ans[i] += _gm.DIAG[i] * _gv[i];
            for (int j = 0; j < _gm.IG[i + 1] - _gm.IG[i]; j++)
                ans[_gm.JG[_gm.IG[i] + j]] += _gm.AU[_gm.IG[i] + j] * _gv[i];
        }

        //for (int i = 0; i < _gm.Size; i++)
        //    for (int j = 0; j < _gv.Size; j++)
        //        ans[i] += _gm[i, j] * _gv[j];
        return ans;
    }

    public GlobalVector Multiply(GlobalVector _gv)
    {
        if (Size != _gv.Size)
            throw new Exception("Невозможно перемножить матрицу на вектор.");

        GlobalVector ans = new(_gv.Size);

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < IG[i + 1] - IG[i]; j++)
                ans[i] += AL[IG[i] + j] * _gv[JG[IG[i] + j]];
            ans[i] += DIAG[i] * _gv[i];
            for (int j = 0; j < IG[i + 1] - IG[i]; j++)
                ans[JG[IG[i] + j]] += AU[IG[i] + j] * _gv[i];
        }

        return ans;
    }

    //private void ConsiderBoundaryConditions(ArrayOfPoints arrPt)
    //{
    //    if (_ig is null) throw new Exception("_ig is null.");
    //    if (_al is null) throw new Exception("_al is null.");
    //    if (_diag is null) throw new Exception("_diag is null.");
    //    if (_au is null) throw new Exception("_au is null.");

    //    foreach (var border in arrBrd)
    //    {
    //        switch (border[0])
    //        {
    //            case 1:
    //                {
    //                    for (int i = 2; i < 4; i++)
    //                    {
    //                        for (int j = _ig[border[i]]; j < _ig[border[i] + 1]; j++)
    //                            _al[j] = 0;
    //                        _diag[border[i]] = 1;
    //                        for (int j = 0; j < _jg.Count; j++)
    //                            if (_jg[j] == border[i])
    //                                _au[j] = 0;
    //                    }
    //                    break;
    //                }
    //            case 2: break; // du / dn = 0
    //            case 3: throw new ArgumentException("Пока нет возможности учитывать КУ 3-го рода");

    //        }
    //    }
    //}

    private void Add(List<int> elem, ArrayOfPoints arrPt)
    {
        LocalMatrix lm = new(elem, arrPt);

        if (_diag is null) throw new Exception("_diag isn't initialized");
        if (_ig is null) throw new Exception("_ig isn't initialized");
        if (_au is null) throw new Exception("_au isn't initialized");
        if (_al is null) throw new Exception("_au isn't initialized");


        int ii = 0;
        foreach (var i in elem)
        {
            int jj = 0;
            foreach (var j in elem)
            {
                int ind = 0;
                double val = 0.0D;
                switch (i - j)
                {
                    case 0:
                        val = lm[ii, jj];
                        _diag[i] += lm[ii, jj];
                        break;
                    case < 0:
                        ind = _ig[j];
                        for (; ind <= _ig[j + 1] - 1; ind++)
                            if (_jg[ind] == i) break;
                        val = lm[ii, jj];
                        _au[ind] += lm[ii, jj];
                        break;
                    case > 0:
                        ind = _ig[i];
                        for (; ind <= _ig[i + 1] - 1; ind++)
                            if (_jg[ind] == j) break;
                        val = lm[ii, jj];
                        _al[ind] += lm[ii, jj];
                        break;
                }
                jj++;
            }
            ii++;
        }
    }

}
