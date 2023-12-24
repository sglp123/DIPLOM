using VKR.structures;

namespace MathObjects;

// ! Пояснения к векторному МКЭ.
/*
    Принципы построения локальной матрицы
        (Ma1, Ma2, Ma3)
    G = (Ma4, Ma5, Ma6),
        (Ma7, Ma8, Ma9)
    где Mai - подматрица 4х4.
    
        (D, 0, 0)
    M = (0, D, 0),
        (0, D, 0)
    где 
        (4 2 2 1)
    D = (2 4 1 2)
        (2 1 4 2)
        (1 2 2 4)
    Более подробно про Mai в кирпиче на стр. 746.
*/



// ! Работает с горем пополам, лучше перепроверить.
public class LocalMatrix
{
    private readonly double _lambda = 1;

    private readonly double _rk;

    private readonly double _hr;

    private readonly double _gamma = 1;

    private readonly double _hz;

    public double this[int i, int j]
    {
        get
        {
            if (i > 3 || j > 3) throw new IndexOutOfRangeException("Local matrix error.");
            return _lambda * ((_rk / _hr + 0.5) * _G[i % 2, j % 2] * _hz * _M[i / 2, j / 2] +
                   _hr * (_rk * _M[i % 2, j % 2] + _hr * _M2R[i % 2, j % 2]) * _G[i / 2, j / 2] / _hz) +
                   _gamma * (_hz * _M[i / 2, j / 2] * _hr * (_rk * _M[i % 2, j % 2] + _hr * _M2R[i % 2, j % 2]));
        }
    }


    private readonly double[,] _G =  {{ 1.0, -1.0},
                                      {-1.0,  1.0}};

    private readonly double[,] _M2R = {{0.08333333333333333, 0.08333333333333333},
                                       {0.08333333333333333, 0.25}};

    private readonly double[,] _M = {{0.3333333333333333, 0.1666666666666666},
                                     {0.1666666666666666, 0.3333333333333333}};


    public LocalMatrix(double lambda, double gamma, double rk, double hz, double hr)
    {
        _lambda = lambda;
        _gamma = gamma;
        _rk = rk;
        _hr = hr;
        _hz = hz;
    }

    // А лямбда с гаммой ?
    public LocalMatrix(List<int> elem, ArrayOfPoints arrPt)
    {
        _rk = arrPt[elem[0]].R;
        _hr = arrPt[elem[1]].R - arrPt[elem[0]].R;
        _hz = arrPt[elem[2]].Z - arrPt[elem[0]].Z;
    }
}