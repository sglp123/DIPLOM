// Расчет нестационарного электромагнитного поля, кругового электрического диполя в осесимметричной задаче

using System.Globalization;
using Diplom;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
Mesh MyMesh = new();
MyMesh.ReadFrom("C:\\Users\\ЛаринМаркВячеславови\\Desktop\\Diplom\\Diplom\\input\\Grid.txt", "C:\\Users\\ЛаринМаркВячеславови\\Desktop\\Diplom\\Diplom\\input\\b1.txt");
MyMesh.GenerateGrid();
MyMesh.OutPutPoints();
MyMesh.GenerateElemsArray();