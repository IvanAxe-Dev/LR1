using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using System;
using System.Collections.Generic;
using Calculator;

using Grid = Microsoft.Maui.Controls.Grid;

namespace SpreadSheets_v14
{
    public partial class MainPage : ContentPage
    {

        public static int CountColumn = 20;
        public static int CountRow = 50;

        public MainPage()
        {
            InitializeComponent();
            CreateGrid();

            //press Enter to go to the next cell in a column
            foreach (var entry in grid.Children.OfType<Entry>())
            {
                entry.Completed += (s, e) =>
                {
                    var row = Grid.GetRow(entry);
                    var col = Grid.GetColumn(entry);
                    var content = entry.Text;
                    var nextEntry = grid.Children
                        .OfType<Entry>()
                        .FirstOrDefault(e => Grid.GetRow(e) == (row + 1) &&
                        Grid.GetColumn(e) == col);
                    CalculateButton_Clicked(s, e);
                    nextEntry?.Focus();
                };


            }
        }

        private void CreateGrid()
        {
            AddColumnsAndColumnLabels();
            AddColumnsAndCellEntries();
            InitializeVariables();
        }

        private void AddColumnsAndColumnLabels()
        {
            for (int col = 0; col < CountColumn + 1; col++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                if (col > 0)
                {
                    var label = new Label()
                    {
                        Text = GetColumnName(col),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    Grid.SetRow(label, 0);
                    Grid.SetColumn(label, col);
                    grid.Children.Add(label);
                }
            }

        }

        private void AddColumnsAndCellEntries()
        {
            for (int row = 0; row < CountRow; row++)
            {
                grid.RowDefinitions.Add(new RowDefinition());

                var label = new Label()
                {
                    Text = (row + 1).ToString(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                Grid.SetRow(label, row + 1);
                Grid.SetColumn(label, 0);
                grid.Children.Add(label);


                for (int col = 0; col < CountColumn; col++)
                {
                    var entry = new Entry
                    {
                        Text = "",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        WidthRequest = 200
                    };
                    entry.Unfocused += Entry_Unfocused;
                    entry.Focused += Entry_Focused;
                    Grid.SetRow(entry, row + 1);
                    Grid.SetColumn(entry, col + 1);
                    grid.Children.Add(entry);
                }
            }
        }

        private string GetColumnName(int colIndex)
        {
            int divident = colIndex;
            string columnName = string.Empty;

            while (divident > 0)
            {
                int modulo = (divident - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                divident = (divident - modulo) / 26;
            }

            return columnName;
        }

        private void Entry_Focused(object sender, EventArgs e)
        {
            var entry = (Entry)sender;
            textInput.Text = entry.Text;
        }

        private async void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            var row = Grid.GetRow(entry) - 1;
            var col = Grid.GetColumn(entry) - 1;
            textInput.Text = "";


            // =... tap on the cell you want to use in the formula
            if (entry.Text.StartsWith("=")) {
 
                    await Task.Delay(100);
                    foreach (var anotherEntry in grid.Children.OfType<Entry>())
                    {
                        if (anotherEntry.IsFocused && anotherEntry != entry)
                        {
                            var anotherRow = Grid.GetRow(anotherEntry);
                            var anotherCol = Grid.GetColumn(anotherEntry);
                            entry.Text += (GetColumnName(anotherCol) + anotherRow).ToString();
                            entry.Focus();
                        }
                    }
                
            }
            double value = double.TryParse(entry.Text, out value) ? value : 0;
            CalculatorVisitor.tableIdentifier[(GetColumnName(col + 1) + (row + 1)).ToString()] = value;
            entry.Unfocus();
        }

        private void CalculateButton_Clicked(object sender, EventArgs e)
        {
            foreach (var entry in grid.Children.OfType<Entry>())
            {
                try
                {
                    string expression = entry.Text;
                    if (expression.StartsWith("="))
                    {
                        var result = Calculator.Calculator.Evaluate(expression.Substring(1));
                        entry.Text = result.ToString();
                        var row = Grid.GetRow(entry);
                        var col = Grid.GetColumn(entry);
                        CalculatorVisitor.tableIdentifier[row+GetColumnName(col)] = result;
                    }
                }
                catch (Exception ex)
                {
                    entry.Text = ex.Message;
                }

            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            string[][] rows = new string[CountRow][];
            int countRows = 0;
            string[] cols = new string[CountColumn];
            int countCols = 0;
            foreach (var entry in grid.Children.OfType<Entry>())
            {
                if (countCols != 0 && countCols % CountColumn == 0)
                {
                    rows[countRows] = cols;
                    countRows++;
                    countCols = 0;
                    continue;
                }
                else
                {
                    cols[countCols] = (entry.Text==null ? " " : entry.Text);
                    countCols++;
                }
            }
            SaveTable.DrawTable(rows);
            await DisplayAlert("Увага!", "Ваша таблиця була збережена до папки проекту у файл table.txt", "ОК");
        }

        private async void ExitButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Підтвердження", "Ви дійсно хочете покинути застосунок?", "Так", "Ні");
            if (answer)
            {
                System.Environment.Exit(0);
            }
        }

        private async void HelpButton_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Довідка", "ЛР1 Аксані Івана. Таблиця", "ОК");
        }

        private void AddRowButton_Clicked(Object sender, EventArgs e)
        {
            int newRow = grid.RowDefinitions.Count;

            grid.RowDefinitions.Add(new RowDefinition());

            var label = new Label
            {
                Text = newRow.ToString(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, newRow);
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);

            for (int col = 0; col < CountColumn; col++)
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };

                entry.Unfocused += Entry_Unfocused;

                Grid.SetRow(entry, newRow);
                Grid.SetColumn(entry, col + 1);
                grid.Children.Add(entry);
                CountRow++;
            }
        }

        private void DeleteRowButton_Clicked(Object sender, EventArgs e)
        {
            if (grid.RowDefinitions.Count > 1)
            {
                int lastRowIndex = grid.RowDefinitions.Count - 1;
                grid.RowDefinitions.RemoveAt(lastRowIndex);
                grid.Children.RemoveAt(lastRowIndex * (CountColumn + 1));
                for (int col = 0; col < CountColumn; col++)
                {
                    grid.Children.RemoveAt((lastRowIndex * CountColumn) + col + 1);
                }
            }
        }

        private void AddColumnButton_Clicked(Object sender, EventArgs e)
        {
            int newColumn = grid.ColumnDefinitions.Count;

            grid.ColumnDefinitions.Add(new ColumnDefinition());

            var label = new Label
            {
                Text = GetColumnName(newColumn),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, 0);
            Grid.SetColumn(label, newColumn);
            grid.Children.Add(label);

            for (int row = 0; row < CountRow; row++)
            {
                var entry = new Entry
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                entry.Unfocused += Entry_Unfocused;
                Grid.SetRow(entry, row + 1);
                Grid.SetColumn(entry, newColumn);
                grid.Children.Add(entry);
                CountColumn++;
            }
        }

        private void DeleteColumnButton_Clicked(Object sender, EventArgs e)
        {
            if (grid.ColumnDefinitions.Count > 1)
            {
                int lastColumnIndex = grid.ColumnDefinitions.Count - 1;
                grid.ColumnDefinitions.RemoveAt(lastColumnIndex);
                grid.Children.RemoveAt(lastColumnIndex);
                for (int row = 0; row < CountColumn; row++)
                {
                    grid.Children.RemoveAt(row * (CountColumn + 1) + lastColumnIndex + 1);
                }
            }
        }

        private void InitializeVariables()
        {
            for(int row = 1; row <= CountRow; row++)
            {
                for(int col = 1; col <= CountColumn; col++)
                {
                    CalculatorVisitor.tableIdentifier.Add((GetColumnName(col) + row), 0);
                }
            }
        }
    }
}