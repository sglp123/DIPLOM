// Расчет нестационарного электромагнитного поля, кругового электрического диполя в осесимметричной задаче

using System.Globalization; // для CultureInfo
using VKR;
using VKR.structures;
using MathObjects;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture; // вывод значений с точками

Mesh MyMesh = new();
MyMesh.ReadFrom("C:\\Users\\ЛаринМаркВячеславови\\Desktop\\Diplom\\Diplom\\input\\Grid.txt", "C:\\Users\\ЛаринМаркВячеславови\\Desktop\\Diplom\\Diplom\\input\\b1.txt");
MyMesh.GenerateGrid();
MyMesh.OutPutPoints();
MyMesh.GenerateElemsArray();

ArrayOfElems myArrayOfElems = new();
ArrayOfPoints myArrayOfPoints = new();
GlobalMatrix myGlobalMatrix = new(myArrayOfElems, myArrayOfPoints);
GlobalVector myGlobalVector = new(myArrayOfElems, myArrayOfPoints);