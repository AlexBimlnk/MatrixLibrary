using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace MatrixLibrary
{
    public abstract class Determinant
    {
        private static List<string> history = new List<string>();

        private static double[,] table;
        private static int size;
        private static double det;

        private static short eps = 12;

        public static void CreateDetTable(double[,] arrValue)
        {
            size = (int)Math.Sqrt(arrValue.Length);
            table = (double[,])arrValue.Clone();
            det = 1;
            DetFind();
        }

        private static void DetFind()
        {
            history.Clear();

            //Приводим к ступечатому виду
            bool swap;
            void SwapLine(int indexLine, int indexSwapLine)
            {
                swap = true;
                history.Add($"line_{indexLine}_{indexSwapLine}");
                for(int j = 0; j<size; j++)
                {
                    double num = table[indexLine, j];
                    table[indexLine, j] = -table[indexSwapLine, j];
                    table[indexSwapLine, j] = num;
                }
            }

            for (int i = 0; i < size - 1; i++)
            {
                //Если на главной диагонали 0
                if(table[i,i] == 0)
                {
                    swap = false;
                    for(int j = i+1; j<size; j++)
                        if(table[j, i] != 0)
                        {
                            SwapLine(i, j);
                            break;
                        }

                    //Если в столбце нет ненулевых элементов
                    if (!swap)
                    {
                        det = 0;
                        continue;
                    }
                }
                //Обнуляем под главной диагональю
                for (int j = i + 1; j < size; j++)
                {
                    double kofForLine = Math.Round((table[j, i] * -1) / table[i, i], eps);
                    if (kofForLine == 0)
                        continue;
                    history.Add($"sum_{i}_{j}_{kofForLine}");
                    for (int k = 0; k < size; k++)
                    {
                        table[j, k] = Math.Round((table[i, k] * kofForLine)
                                                              +
                                                  table[j, k],eps);
                    }
                }
            }

            //Обнуляем над главной диагональю
            for (int i = size-1; i>0; i--)
            {
                for (int j = i-1; j >= 0; j--)
                {
                    double kofForLine = Math.Round((table[j, i] * -1) / table[i, i], eps);
                    if (kofForLine == 0 || table[i,i] == 0)
                        continue;
                    history.Add($"sum_{i}_{j}_{kofForLine}");
                    table[j, i] = Math.Round((table[i, i] * kofForLine)
                                                          +
                                              table[j, i], eps);
                }
            }
            if (det != 0)
            {
                //Считаем определитель
                for (int i = 0; i < size; i++)
                    det *= table[i, i];
                det = Math.Round(det, eps);
            }
        }

        #region Свойства
        public static short Eps
        {
            set { eps = value; }
            get { return eps; }
        }

        public static List<string> GetHistoryChangeTable
        {
            get { return history; }
        }

        public static double GetDeterminant
        {
            get
            {
                if (table != null)
                    return det;
                else
                    return double.NaN;
            }
        }

        public static double[,] GetTable
        {
            get { return table; }
        }

        public static int GetSize
        {
            get { return size; }
        }
        #endregion
    }
}
