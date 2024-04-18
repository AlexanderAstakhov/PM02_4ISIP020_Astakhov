using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace PM02_4ISIP020_Astakhov
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

      
        private bool IsNumeric(string input)
        {
            return int.TryParse(input, out _);
        }

        // Проверка полей
        private bool IsFilled(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

      
        private bool ValidateInput(string[] inputs)
        {
            foreach (string input in inputs)
            {
                if (!IsFilled(input) || !IsNumeric(input))
                {
                    return false;
                }
            }
            return true;
        }


        private void SolveByMinimumCost(object sender, RoutedEventArgs e)
        {
            string[] demandInputs = txtConsumerDemand.Text.Split(',');
            string[] supplyInputs = txtSupplierSupply.Text.Split(',');
            string[] costRows = txtCostMatrix.Text.Split(';');

            if (!ValidateInput(demandInputs) || !ValidateInput(supplyInputs) || costRows.Any(row => !ValidateInput(row.Split(','))))
            {
                if (!ValidateInput(demandInputs) || !ValidateInput(supplyInputs))
                {
                    MessageBox.Show("Проверь данные полей");
                }
                else
                {
                    MessageBox.Show("Заполни все поля.");
                }
                return;
            }

            int[] demand = Array.ConvertAll(demandInputs, int.Parse);
            int[] supply = Array.ConvertAll(supplyInputs, int.Parse);

            int[][] costMatrix = new int[costRows.Length][];
            for (int i = 0; i < costRows.Length; i++)
            {
                costMatrix[i] = costRows[i].Split(',').Select(int.Parse).ToArray();
            }

            var (allocation, totalCost) = MinimumCostMethod(supply, demand, costMatrix);
            DisplayResult(allocation, totalCost);
        }


        static (int[][], int) MinimumCostMethod(int[] supply, int[] demand, int[][] costs)
        {
            int[][] allocation = new int[supply.Length][];
            for (int i = 0; i < supply.Length; i++)
            {
                allocation[i] = new int[demand.Length];
            }

            int[] supplyCopy = supply.ToArray();
            int[] demandCopy = demand.ToArray();
            int totalCost = 0;

            while (true)
            {
                int minCost = int.MaxValue;
                int minRow = -1, minCol = -1;

                for (int row = 0; row < supply.Length; row++)
                {
                    for (int col = 0; col < demand.Length; col++)
                    {
                        if (supplyCopy[row] > 0 && demandCopy[col] > 0)
                        {
                            if (costs[row][col] < minCost)
                            {
                                minCost = costs[row][col];
                                minRow = row;
                                minCol = col;
                            }
                        }
                    }
                }

                if (minRow == -1 || minCol == -1)
                {
                    break;
                }

                int x = Math.Min(supplyCopy[minRow], demandCopy[minCol]);
                allocation[minRow][minCol] = x;
                supplyCopy[minRow] -= x;
                demandCopy[minCol] -= x;
                totalCost += x * minCost;
            }

            return (allocation, totalCost);
        }

        private void DisplayResult(int[][] allocation, int totalCost)
        {
            StringBuilder resultBuilder = new StringBuilder();

         
            resultBuilder.AppendLine("Опорный план:");
            for (int i = 0; i < allocation.Length; i++)
            {
                for (int j = 0; j < allocation[i].Length; j++)
                {
                    resultBuilder.Append(allocation[i][j]);
                    resultBuilder.Append("\t");
                }
                resultBuilder.AppendLine();
            }

            
            resultBuilder.AppendLine($"Общая стоимость: {totalCost}");

            txtSolution.Text = resultBuilder.ToString();
        }

        private void ClearFields(object sender, RoutedEventArgs e)
        {
            txtSupplierSupply.Clear();
            txtConsumerDemand.Clear();
            txtCostMatrix.Clear();
        }

        private void ClearSolution(object sender, RoutedEventArgs e)
        {
            txtSolution.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Результаты"; // Имя файла по умолчанию
            dlg.DefaultExt = ".txt"; // Расширение файла по умолчанию
            dlg.Filter = "Текстовые файлы (.txt)|*.txt"; // Фильтр для типов файлов

            // Показать диалог сохранения файла
            bool? result = dlg.ShowDialog();

            // Продолжить только если пользователь выбрал файл
            if (result == true)
            {
                // Получить путь к файлу
                string filePath = dlg.FileName;

                // Экспорт результатов в текстовый файл
                ExportToTxt(filePath);
            }
        }

        private void ExportToTxt(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(txtSolution.Text);
                }

                MessageBox.Show("Результаты успешно экспортированы в текстовый файл.", "Экспорт завершён", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при экспорте результатов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

