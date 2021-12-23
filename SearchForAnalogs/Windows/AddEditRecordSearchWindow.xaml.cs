using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SearchForAnalogs.Models;
using Microsoft.EntityFrameworkCore;

namespace SearchForAnalogs
{
    /// <summary>
    /// Логика взаимодействия для AddEditRecordSearchWindow.xaml
    /// </summary>
    public partial class AddEditRecordSearchWindow : Window
    {
       
        public RecordViewModel CurrentRecord { get; private set; }
        AnalogsDBContext db;
        public AddEditRecordSearchWindow(RecordViewModel currentRecord, bool search)
        {
            InitializeComponent();
            
            db = new AnalogsDBContext();
            CurrentRecord = currentRecord;

            if (currentRecord.Id == 0)
                Title = "Создание";
            else
                Title = "Обновление";

            if (search == true)
            {
                confidenceLabel.Content = "Глубина рекурсии";
                Title = "Поиск связи";
            }         

            this.DataContext = CurrentRecord ;

            articleCombobox1.ItemsSource = db.Products.Select(u => u.Article).ToList();
            articleCombobox2.ItemsSource = db.Products.Select(u => u.Article).ToList();
            manufacturerCombobox1.ItemsSource = db.Manufactureres.Select(u => u.Name).ToList();
            manufacturerCombobox2.ItemsSource = db.Manufactureres.Select(u => u.Name).ToList();
           
            this.Closing += MainWindow_Closing;
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }
        
        private void confidenceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
            if (confidenceTextBox.Text == "0")
                confidenceTextBox.Text=confidenceTextBox.Text.Remove(0, 1);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (articleCombobox1.Text != "" && articleCombobox2.Text != ""
                    && manufacturerCombobox1.Text != "" && manufacturerCombobox2.Text != ""
                    && confidenceTextBox.Text != "" )
            {
                if (CurrentRecord.GetRecord().Product1.Equals(CurrentRecord.GetRecord().Product2))
                    MessageBox.Show("Два товара идентичны!");
                else
                {
                    DialogResult = true;
                    Close();
                }
            }
            else
                MessageBox.Show("Не все поля заполнены!");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
     
        private void articleCombobox_Selected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            ComboBox manufacturerCombobox;
            if (comboBox.Name == "articleCombobox1")
                manufacturerCombobox = manufacturerCombobox1;
            else
                manufacturerCombobox = manufacturerCombobox2;

            //если товар выбран из списка, то выводим соответствующего ему производителя
            //производителя для этого товара менять нельзя
            if (comboBox.SelectedItem != null)
            {
                string article = comboBox.SelectedItem.ToString();
                Product product = db.Products.Include(m => m.Manufacturer).Where(u => u.Article == article).First();
              
                manufacturerCombobox.Text = product.Manufacturer.Name;
                manufacturerCombobox.IsEnabled = false;

            }
            else //если это новый товар, то разрешается ввод производителя
                manufacturerCombobox.IsEnabled = true;


        }
        

        
    }
}
