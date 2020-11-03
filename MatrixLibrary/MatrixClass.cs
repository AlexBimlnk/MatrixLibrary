using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace MatrixLibrary
{
    public class MatrixClass
    {
        public enum ValueName
        {
            Matrix,
            Solution,
            MatrixT,
            MatrixInverse
        }

        private short eps = 10;

        private double[,] table;
        private double[] lineSolution;
        private double[,] transpositionTable;
        private double[,] inverseTable;

        //---------Gaus-----------
        private double[,] gausTable;
        private double[] gausSolution;
        private string answerGaus = "<GausMethod>\n\tAnswer is:";
        //---------Gaus-----------

        //--------Kramer----------
        private string answerKramer = "<KramerMethod>\n\tAnswer is:";
        //--------Kramer----------

        //--------Matrix----------
        private string answerMatrix = "<MatrixMethod>\n\tAnswer is:";
        //--------Matrix----------

        private double det;
        private int rowCount;
        private int columnCount;
        private string size;

        #region Статические функции
        public static MatrixClass operator + (MatrixClass matrix1, MatrixClass matrix2)
        {
            if (matrix1.GetColumnCount == matrix2.GetColumnCount &&
                matrix1.GetRowCount == matrix2.GetRowCount)
            {
                int row = matrix1.GetRowCount;
                int column = matrix1.GetColumnCount;

                double[,] _matrix1 = matrix1.GetMatrix;
                double[,] _matrix2 = matrix2.GetMatrix;
                double[,] tb = new double[row, column];
                for (int i = 0; i < row; i++)
                    for (int j = 0; j < column; j++)
                        tb[i, j] = _matrix1[i, j] + _matrix2[i, j];
                return new MatrixClass(row, column, tb);
            }
            else
                throw new Exception("Число строк и столбцов первой матрицы " +
                    "не соответствуют числу строк и столбцов второй матрицы");
        }

        public static MatrixClass operator - (MatrixClass matrix1, MatrixClass matrix2)
        {
            if (matrix1.GetColumnCount == matrix2.GetColumnCount &&
                matrix1.GetRowCount == matrix2.GetRowCount)
            {
                int row = matrix1.GetRowCount;
                int column = matrix1.GetColumnCount;

                double[,] _matrix1 = matrix1.GetMatrix;
                double[,] _matrix2 = matrix2.GetMatrix;
                double[,] tb = new double[row, column];
                for (int i = 0; i < row; i++)
                    for (int j = 0; j < column; j++)
                        tb[i, j] = _matrix1[i, j] - _matrix2[i, j];
                return new MatrixClass(row, column, tb);
            }
            else
                throw new Exception("Число строк и столбцов первой матрицы " +
                    "не соответствуют числу строк и столбцов второй матрицы");
        }

        public static MatrixClass operator * (MatrixClass matrix, double k)
        {
            int row = matrix.GetRowCount;
            int column = matrix.GetColumnCount;
            double[,] _m1 = matrix.GetMatrix;
            double[] _s1 = matrix.GetSolution;
            double[,] tb = new double[row, column];
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                    tb[i, j] = _m1[i, j] * k;
            return new MatrixClass(row, column, tb);
        }

        public static MatrixClass operator * (MatrixClass matrix1, MatrixClass matrix2)
        {
            int column1 = matrix1.GetColumnCount;
            int row2 = matrix2.GetRowCount;
            if (column1 == row2)
            {
                int row1 = matrix1.GetRowCount;
                int column2 = matrix2.GetColumnCount;

                double[,] _tableM1 = matrix1.GetMatrix;
                double[,] _tableM2 = matrix2.GetMatrix;

                double[,] tb = new double[row1, column2];

                for (int i = 0; i < row1; i++)
                    for (int j = 0; j < column2; j++)
                        for (int k = 0; k < column1; k++)
                            tb[i, j] += _tableM1[i, k] * _tableM2[k, j];

                return new MatrixClass(row1, column2, tb);
            }
            else
                throw new Exception("Количество столбцов первой матрицы " +
                    "не равно количеству строк второй матрицы.");
        }
        #endregion

        #region Конструкторы
        //Конструктор для квадратных матриц
        public MatrixClass(int _size, double[,] value, double[] _lineSolution = null)
        {
            rowCount = _size;
            columnCount = _size;
            size = $"{_size}x{_size}";

            if (_lineSolution != null)
                lineSolution = (double[])_lineSolution.Clone();

            table = (double[,])value.Clone();
            TranspositionMatrix();

            Determinant.CreateDetTable(table);
            det = Determinant.GetDeterminant;
            if(det!=0)
                InverseMatrix();
        }

        //Конструктор произвольных матриц
        public MatrixClass(int _row, int _column, double[,] value)
        {
            rowCount = _row;
            columnCount = _column;
            size = $"{rowCount}x{columnCount}";

            table = (double[,])value.Clone();
            TranspositionMatrix();
        }
        #endregion

        #region Print value function
        public void PrintValue(ValueName valueName, string name = "")
        {
            if (valueName == ValueName.Matrix)
            {
                Console.WriteLine($"GetMatrix {name} is: ");
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                        Console.Write($"\t{table[i, j]}");
                    Console.WriteLine();
                }
            }
            if (valueName == ValueName.Solution)
            {
                Console.WriteLine($"GetSolution {name} is: ");
                for (int i = 0; i < lineSolution.Length; i++)
                    Console.Write($"\t{lineSolution[i]}");
                Console.WriteLine();
            }
            if (valueName == ValueName.MatrixT)
            {
                Console.WriteLine($"Transposition matrix {name} is: ");
                for (int i = 0; i < columnCount; i++)
                {
                    for (int j = 0; j < rowCount; j++)
                        Console.Write($"\t{transpositionTable[i, j]}");
                    Console.WriteLine();
                }
            }
            if (valueName == ValueName.MatrixInverse)
            {
                Console.WriteLine($"Inverse matrix {name} is: ");
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                        Console.Write($"\t{inverseTable[i,j]}");
                    Console.WriteLine();
                }
            }
        }
        #endregion

        public void Solve()
        {
            if (rowCount == columnCount)
            {
                if (lineSolution != null)
                {
                    GausMethod();
                    Console.WriteLine(answerGaus);
                    KramerMethod();
                    Console.WriteLine(answerKramer);
                    MatrixMethod();
                    Console.WriteLine(answerMatrix);
                }
                else
                    throw new Exception("Не заданы равенства линейных уравнений.");
            }
            else
                throw new Exception("Матрица не является квадратной.");
        }

        private void InverseMatrix()
        {
            double[,] tb = Determinant.GetTable;
            double[,] E = new double[rowCount, columnCount];
            for (int i = 0; i < rowCount; i++)
                E[i, i] = 1;
            HistoryParsing(Determinant.GetHistoryChangeTable, true, E);
            inverseTable = (double[,])E.Clone();
            for (int i = 0; i < rowCount; i++)
                for (int j = 0; j < rowCount; j++)
                    inverseTable[i, j] = Math.Round(inverseTable[i,j]/tb[i, i],eps);
        }

        private void TranspositionMatrix()
        {
            transpositionTable = new double[columnCount, rowCount];
            for (int i = 0; i < columnCount; i++)
                for (int j = 0; j < rowCount; j++)
                    transpositionTable[i, j] = table[j, i];
        }

        private void KramerMethod()
        {
            if (det != 0)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    double[,] tb = (double[,])table.Clone();
                    for (int j = 0; j < rowCount; j++)
                        tb[j, i] = lineSolution[j];
                    Determinant.CreateDetTable(tb);
                    answerKramer += $"\n\t\t{i + 1} = {Math.Round(Determinant.GetDeterminant / det, eps)}; ";
                }
            }
            else
                throw new Exception("<KramerMethod>\n\tDeterminant is 0");
        }

        private void GausMethod()
        {
            Determinant.CreateDetTable(table);
            gausTable = Determinant.GetTable;
            gausSolution = new double[rowCount];
            lineSolution.CopyTo(gausSolution, 0);
            HistoryParsing(Determinant.GetHistoryChangeTable);
            string ans = "";
            for (int i = rowCount - 1; i >= 0; i--)
                ans = ans.Insert(0, $"\n\t\t{i+1} = {Math.Round(gausSolution[i]/gausTable[i,i])};");
            answerGaus += ans;
            det = Determinant.GetDeterminant;
        }

        private void MatrixMethod()
        {
            if (det != 0)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < rowCount; j++)
                        sum += inverseTable[i, j] * lineSolution[j];
                    answerMatrix += $"\n\t\t{i + 1} = {Math.Round(sum, eps)};";
                }
            }
            else
                throw new Exception("<MatrixMethod>\n\tDeterminant is 0");
        }

        private void HistoryParsing(List<string> _history, bool forInverse = false, double[,] tb = null)
        {
            foreach(string i in _history)
            {
                string[] parsArr = i.Split('_');

                int indexLine = Int32.Parse(parsArr[1]);
                int indexNextLine = Int32.Parse(parsArr[2]);
                //Если свапаем линиии
                if (parsArr[0] == "line")
                {
                    //Если парсим для обратной матрицы
                    if (forInverse)
                    {
                        for (int j = 0; j < columnCount; j++)
                        {
                            double num = tb[indexLine, j];
                            tb[indexLine, j] = -tb[indexNextLine, j];
                            tb[indexNextLine, j] = num;
                        }
                    }
                    else
                    {
                        double num = gausSolution[indexLine];
                        gausSolution[indexLine] = -gausSolution[indexNextLine];
                        gausSolution[indexNextLine] = num;
                    }
                }
                else
                {
                    double kof = double.Parse(parsArr[3]);
                    if (forInverse)
                        for (int j = 0; j < GetColumnCount; j++)
                            tb[indexNextLine, j] += tb[indexLine, j] * kof;
                    else
                        gausSolution[indexNextLine] += gausSolution[indexLine] * kof;
                }
            }
        }

        #region Свойства

        public short Eps
        {
            set { eps = value; }
            get { return eps; }
        }


        //----------GetMatrixTable-----------
        public double[,] GetMatrix
        {
            get { return table; }
        }
        public double[,] GetInverseMatrix
        {
            get { return inverseTable; }
        }
        public double[,] GetTransMatrix
        {
            get { return transpositionTable; }
        }
        public double[,] GetChangeGausMatrix
        {                                       
            get { return gausTable; }
        }

        public double[] GetSolution
        {
            get { return lineSolution; }
        }
        public double[] GetGausSolution
        {
            get { return gausSolution; }
        }


        //----------GetAnswer--------------
        public string GetAnswerKramer
        {
            get { return answerKramer; }
        }
        public string GetAnswerGaus
        {
            get { return answerGaus; }
        }
        public string GetAnswerMatrix
        {
            get { return answerMatrix; }
        }

        //----------GetInfoMatrix----------
        public string GetSize
        {
            get { return size; }
        }

        public int GetRowCount
        {
            get { return rowCount; }
        }
        public int GetColumnCount
        {
            get { return columnCount; }
        }

        public double GetDetMatrix
        {
            get { return det; }
        }

        #endregion
    }
}
