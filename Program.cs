// Программа для решения транспортной задачи и задачи линейного программирования
// файл с ответами ищи в bin/Debug/netX.0/result.txt
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class SolverApp
{
    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("--- Решатель задач ---");
            Console.WriteLine("1. Транспортная задача");
            Console.WriteLine("2. Задача линейного программирования");
            Console.Write("Выберите тип задачи: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    TransportSolver.Solve();
                    break;
                case "2":
                    LinearProgramSolver.Solve();
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Нажмите любую клавишу для продолжения.");
                    Console.ReadKey();
                    break;
            }

            Console.Write("\nРешить новую задачу? (y/n): ");
            if (Console.ReadLine().ToLower() != "y") break;
        }
    }
}

public class TransportProblem
{
    public static TransportProblem Normalize(int rows, int cols, int[] supply, int[] demand, int[,] costs)
    {
        int sumSupply = supply.Sum();
        int sumDemand = demand.Sum();

        if (sumSupply < sumDemand)
        {
            rows++;
            int[,] newCosts = new int[rows, cols];
            for (int i = 0; i < rows - 1; i++)
                for (int j = 0; j < cols; j++)
                    newCosts[i, j] = costs[i, j];
            for (int j = 0; j < cols; j++) newCosts[rows - 1, j] = 0;
            costs = newCosts;

            supply = supply.Concat(new[] { sumDemand - sumSupply }).ToArray();
        }
        else if (sumSupply > sumDemand)
        {
            cols++;
            int[,] newCosts = new int[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols - 1; j++)
                    newCosts[i, j] = costs[i, j];
            for (int i = 0; i < rows; i++) newCosts[i, cols - 1] = 0;
            costs = newCosts;

            demand = demand.Concat(new[] { sumSupply - sumDemand }).ToArray();
        }

        return new TransportProblem
        {
            Rows = rows,
            Cols = cols,
            Supply = supply,
            Demand = demand,
            Costs = costs
        };
    }

    public int Rows, Cols;
    public int[] Supply;
    public int[] Demand;
    public int[,] Costs;

    public static TransportProblem FromFile(string path)
    {
        string[] lines = File.ReadAllLines(path).Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#")).ToArray();

        int rows = int.Parse(lines[0].Split(':')[1].Trim());
        int cols = int.Parse(lines[1].Split(':')[1].Trim());

        int[] supply = lines[2].Split(':')[1].Trim().Split().Select(int.Parse).ToArray();
        int[] demand = lines[3].Split(':')[1].Trim().Split().Select(int.Parse).ToArray();

        int[,] costs = new int[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            var values = lines[5 + i].Trim().Split().Select(int.Parse).ToArray();
            if (values.Length != cols) throw new Exception($"Неверное количество значений в строке стоимости {i + 1}");
            for (int j = 0; j < cols; j++)
                costs[i, j] = values[j];
        }

        return Normalize(rows, cols, supply, demand, costs);
    }

    public void PrintInfo()
    {
        Console.WriteLine("Матрица стоимости:");
        PrintMatrix(Costs);
        Console.WriteLine("Поставки: " + string.Join(" ", Supply));
        Console.WriteLine("Потребности: " + string.Join(" ", Demand));
    }

    public static void PrintMatrix(int[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
                Console.Write(matrix[i, j].ToString().PadLeft(5));
            Console.WriteLine();
        }
    }

    public static void SaveResultToFile(int[,] result, string filename)
    {
        using (StreamWriter sw = new StreamWriter(filename))
        {
            for (int i = 0; i < result.GetLength(0); i++)
                sw.WriteLine(string.Join(" ", Enumerable.Range(0, result.GetLength(1)).Select(j => result[i, j])));
        }
    }
}

public static class TransportSolver
{
    public static void Solve()
    {
        Console.Clear();
        Console.WriteLine("--- Транспортная задача ---");
        Console.WriteLine("1. Ввести данные вручную");
        Console.WriteLine("2. Загрузить данные из файла");
        Console.Write("Выберите способ ввода: ");
        string inputChoice = Console.ReadLine();

        TransportProblem problem = null;

        if (inputChoice == "1")
        {
            Console.Write("Введите количество строк (складов): ");
            int rows = int.Parse(Console.ReadLine());
            Console.Write("Введите количество столбцов (магазинов): ");
            int cols = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите поставки (через пробел): ");
            int[] supply = Console.ReadLine().Trim().Split().Select(int.Parse).ToArray();

            Console.WriteLine("Введите потребности (через пробел): ");
            int[] demand = Console.ReadLine().Trim().Split().Select(int.Parse).ToArray();

            int[,] costs = new int[rows, cols];
            Console.WriteLine("Введите матрицу стоимости по строкам:");
            for (int i = 0; i < rows; i++)
            {
                var row = Console.ReadLine().Trim().Split().Select(int.Parse).ToArray();
                for (int j = 0; j < cols; j++)
                    costs[i, j] = row[j];
            }

            problem = TransportProblem.Normalize(rows, cols, supply, demand, costs);
        }
        else if (inputChoice == "2")
        {
            Console.Write("Введите путь к входному файлу (.txt): ");
            string path = Console.ReadLine();

            if (!File.Exists(path))
            {
                Console.WriteLine("Файл не найден. Нажмите любую клавишу для повтора.");
                Console.ReadKey();
                return;
            }

            problem = TransportProblem.FromFile(path);
        }
        else
        {
            Console.WriteLine("Неверный выбор ввода. Нажмите любую клавишу для повтора.");
            Console.ReadKey();
            return;
        }

        problem.PrintInfo();
        Console.WriteLine("Выберите метод решения:");
        Console.WriteLine("1. Северо-Западный угол");
        Console.WriteLine("2. Метод минимальных элементов");
        Console.Write("Ваш выбор: ");
        int method = int.Parse(Console.ReadLine());

        int[,] result = null;
        switch (method)
        {
            case 1:
                result = NorthWestCorner(problem);
                break;
            case 2:
                result = MinimumCost(problem);
                break;
            default:
                Console.WriteLine("Неверный выбор метода.");
                return;
        }

        Console.WriteLine("Результат:");
        TransportProblem.PrintMatrix(result);

        Console.Write("Сохранить результат в файл? (y/n): ");
        if (Console.ReadLine().ToLower() == "y")
        {
            Console.Write("Введите имя файла: ");
            string savePath = Console.ReadLine();
            TransportProblem.SaveResultToFile(result, savePath);
            Console.WriteLine($"Результат сохранён в {savePath}");
        }

        Console.WriteLine("Нажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }

    public static int[,] NorthWestCorner(TransportProblem prob)
    {
        int[,] result = new int[prob.Rows, prob.Cols];
        int[] supply = (int[])prob.Supply.Clone();
        int[] demand = (int[])prob.Demand.Clone();

        int i = 0, j = 0;
        while (i < prob.Rows && j < prob.Cols)
        {
            int val = Math.Min(supply[i], demand[j]);
            result[i, j] = val;
            supply[i] -= val;
            demand[j] -= val;

            if (supply[i] == 0) i++;
            else if (demand[j] == 0) j++;
        }

        return result;
    }

    public static int[,] MinimumCost(TransportProblem prob)
    {
        int[,] result = new int[prob.Rows, prob.Cols];
        int[] supply = (int[])prob.Supply.Clone();
        int[] demand = (int[])prob.Demand.Clone();
        bool[,] used = new bool[prob.Rows, prob.Cols];

        while (true)
        {
            int minCost = int.MaxValue;
            int minI = -1, minJ = -1;
            for (int i = 0; i < prob.Rows; i++)
            {
                for (int j = 0; j < prob.Cols; j++)
                {
                    if (!used[i, j] && prob.Costs[i, j] < minCost && supply[i] > 0 && demand[j] > 0)
                    {
                        minCost = prob.Costs[i, j];
                        minI = i;
                        minJ = j;
                    }
                }
            }
            if (minI == -1) break;

            int val = Math.Min(supply[minI], demand[minJ]);
            result[minI, minJ] = val;
            supply[minI] -= val;
            demand[minJ] -= val;
            used[minI, minJ] = true;
        }

        return result;
    }
}
public static class LinearProgramSolver
{
    public static void Solve()
    {
        Console.Clear();
        Console.WriteLine("--- Задача линейного программирования (симплекс метод) ---");
        Console.WriteLine("1. Ввести данные вручную");
        Console.WriteLine("2. Загрузить данные из файла");
        Console.Write("Выберите способ ввода: ");
        string inputChoice = Console.ReadLine();

        var A = new List<List<double>>();
        var b = new List<double>();
        var c = new List<double>();

        try
        {
            if (inputChoice == "1")
            {
                Console.Write("Введите количество переменных: ");
                int vars = int.Parse(Console.ReadLine());
                Console.Write("Введите количество ограничений: ");
                int cons = int.Parse(Console.ReadLine());

                Console.WriteLine("Введите коэффициенты ограничений и правую часть через | (пример: 1 2 3 | 30):");
                for (int i = 0; i < cons; i++)
                {
                    var parts = Console.ReadLine().Split('|');
                    A.Add(parts[0].Trim().Split().Select(double.Parse).ToList());
                    b.Add(double.Parse(parts[1].Trim()));
                }

                Console.WriteLine("Введите коэффициенты целевой функции (через пробел):");
                c = Console.ReadLine().Trim().Split().Select(double.Parse).ToList();
            }
            else if (inputChoice == "2")
            {
                Console.Write("Введите путь к входному файлу (.txt): ");
                string path = Console.ReadLine();

                if (!File.Exists(path))
                {
                    Console.WriteLine("Файл не найден. Нажмите любую клавишу для повтора.");
                    Console.ReadKey();
                    return;
                }

                string[] lines = File.ReadAllLines(path);
                bool parsingConstraints = true;

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                    if (line.StartsWith("obj:"))
                    {
                        parsingConstraints = false;
                        c = line.Substring(4).Trim().Split().Select(double.Parse).ToList();
                    }
                    else if (parsingConstraints)
                    {
                        var parts = line.Split('|');
                        A.Add(parts[0].Trim().Split().Select(double.Parse).ToList());
                        b.Add(double.Parse(parts[1]));
                    }
                }
            }
            else
            {
                Console.WriteLine("Неверный выбор. Нажмите любую клавишу для повтора.");
                Console.ReadKey();
                return;
            }

            var simplex = new Simplex(A, b, c);
            var solution = simplex.Solve();

            Console.WriteLine("\nОптимальное значение целевой функции: " + solution.Item1);
            Console.WriteLine("Решение: " + string.Join(" ", solution.Item2));

            Console.Write("\nСохранить результат в файл? (y/n): ");
            if (Console.ReadLine().ToLower() == "y")
            {
                Console.Write("Введите имя файла: ");
                string savePath = Console.ReadLine();
                File.WriteAllLines(savePath, new[]
                {
                    "Целевая функция: " + solution.Item1,
                    "Решение: " + string.Join(" ", solution.Item2)
                });
                Console.WriteLine($"Результат сохранён в {savePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        Console.WriteLine("Нажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }
}

public class Simplex
{
    private List<List<double>> A;
    private List<double> b;
    private List<double> c;

    public Simplex(List<List<double>> A, List<double> b, List<double> c)
    {
        this.A = A;
        this.b = b;
        this.c = c;
    }

    public Tuple<double, List<double>> Solve()
    {
        int m = A.Count;
        int n = A[0].Count;
        var tableau = new double[m + 1, n + m + 1];

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
                tableau[i, j] = A[i][j];
            tableau[i, n + i] = 1;
            tableau[i, n + m] = b[i];
        }

        for (int j = 0; j < n; j++)
            tableau[m, j] = -c[j];

        while (true)
        {
            int pivotCol = -1;
            for (int j = 0; j < n + m; j++)
            {
                if (tableau[m, j] < 0)
                {
                    pivotCol = j;
                    break;
                }
            }
            if (pivotCol == -1) break;

            int pivotRow = -1;
            double minRatio = double.MaxValue;
            for (int i = 0; i < m; i++)
            {
                if (tableau[i, pivotCol] > 0)
                {
                    double ratio = tableau[i, n + m] / tableau[i, pivotCol];
                    if (ratio < minRatio)
                    {
                        minRatio = ratio;
                        pivotRow = i;
                    }
                }
            }
            if (pivotRow == -1) throw new Exception("Решение не существует (неограниченное).");

            double pivot = tableau[pivotRow, pivotCol];
            for (int j = 0; j <= n + m; j++)
                tableau[pivotRow, j] /= pivot;

            for (int i = 0; i <= m; i++)
            {
                if (i == pivotRow) continue;
                double factor = tableau[i, pivotCol];
                for (int j = 0; j <= n + m; j++)
                    tableau[i, j] -= factor * tableau[pivotRow, j];
            }
        }

        var result = new List<double>(new double[n]);
        for (int j = 0; j < n; j++)
        {
            for (int i = 0; i < m; i++)
            {
                if (Math.Abs(tableau[i, j] - 1) < 1e-6)
                {
                    bool isBasic = true;
                    for (int k = 0; k < m; k++)
                    {
                        if (k != i && Math.Abs(tableau[k, j]) > 1e-6)
                        {
                            isBasic = false;
                            break;
                        }
                    }
                    if (isBasic)
                    {
                        result[j] = tableau[i, n + m];
                        break;
                    }
                }
            }
        }

        return Tuple.Create(tableau[m, n + m], result);
    }
}
