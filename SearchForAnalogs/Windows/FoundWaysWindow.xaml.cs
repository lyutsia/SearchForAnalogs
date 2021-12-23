using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SearchForAnalogs.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SearchForAnalogs
{
    /// <summary>
    /// Логика взаимодействия для FoundWaysWindow.xaml
    /// </summary>
    public partial class FoundWaysWindow : Window
    {
        public List<List<Product>> Ways { get; set; } = new List<List<Product>>();
        public List<List<string>> WaysString { get; set; } = new List<List<string>>();
        public List<string> WaysName { get; set; } = new List<string>();

        public FoundWaysWindow(List<List<Product>> ways, int depthRecursion, Product productEnd)
        {
            InitializeComponent();          
            Ways = ways;
            CreateWaysList();
            WaysNameListView.ItemsSource = WaysName;
            WaysNameListView.SelectedIndex = 0;

        }
        /// <summary>
        /// Создание списков правой и левой части окна: 
        /// WaysName {Маршрут 1, Маршрут 2, ...}
        /// WaysString {Product1 -> Product2, Product2 -> Product3, ...}
        /// </summary>
        private void CreateWaysList()
        {
            for (int i = 0; i < Ways.Count; i++)
            {
                List<string> way = new List<string>();
                WaysName.Add($"Маршрут {i + 1}");
                for (int j = 0; j < Ways[i].Count() - 1; j++)
                {
                    way.Add(Ways[i][j].ToString() + " -> " + Ways[i][j + 1].ToString());
                }
                WaysString.Add(way);

            }
        }
        private void waysName_SelectedIndexChanged(object sender, EventArgs e)
        {
            WayListView.ItemsSource = WaysString[WaysNameListView.SelectedIndex];
        }
    }
}
