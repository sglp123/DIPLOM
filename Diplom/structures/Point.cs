using System.Globalization;

namespace VKR.structures;
public enum Location
{
    Inside,   // 0
    OutSide,  // 1
    BI,       // 2
    NotStated // 3
}

public class Point
{
    // Номер подобласти.
    public int SubElemNum { get; set; }

    // Тип точки
    public Location Type { get; set; }

    // Координата по R
    public double R { get; init; }

    // Координата по Z
    public double Z { get; init; }

    /// <summary>
    /// Метод, возвращающий содержимое точки в формате строки.
    /// </summary>
    /// <returns>Форматированная строка.</returns>
    public override string ToString() => $"{Type} {R.ToString("E15", CultureInfo.InvariantCulture)} {Z.ToString("E15", CultureInfo.InvariantCulture)}";

    /// <summary>
    /// Конструктор класса Point
    /// </summary>
    /// <param name="arr">Координаты точек в виде массива (R; Z)</param>
    public Point(double R, double Z)
    {
        this.R = R;
        this.Z = Z;
        Type = Location.NotStated;
    }

    /// <summary>
    /// Конструктор класса Point
    /// </summary>
    /// <param name="arr">Массив с информацией.</param>
    /// <exception cref="Exception"></exception>
    public Point(List<string> arr)
    {
        R = double.Parse(arr[1]);
        Z = double.Parse(arr[2]);   
        switch (arr[0])
        {
            case "Inside":
                {
                    Type = Location.Inside;
                    break;
                }
            case "OutSide":
                {
                    Type = Location.OutSide;
                    break;
                }
            case "BI":
                {
                    Type = Location.BI;
                    break;
                }
            default:
                {
                    Type = Location.NotStated;
                    break;
                }
        }
    }
}