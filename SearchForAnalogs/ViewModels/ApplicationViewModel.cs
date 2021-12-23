using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using SearchForAnalogs.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Windows;
namespace SearchForAnalogs
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
        private RecordViewModel selectedRecord;
        private AnalogsDBContext db;
        private RelayCommand addCommand;
        private RelayCommand editCommand;
        private RelayCommand deleteCommand;
        private RelayCommand findCommand;
        private List<Product> way;
        private List<List<Product>> ways;

        public ObservableCollection<RecordViewModel> Records { get; set; }
        public RecordViewModel SelectedRecord
        {
            get { return selectedRecord; }
            set
            {
                selectedRecord = value;
                OnPropertyChanged("SelectedRecord");
            }
        }

        public ApplicationViewModel()
        {

            db = new AnalogsDBContext();
            //загрузка данных из бд
            Records = new ObservableCollection<RecordViewModel>(db.Records
                                .Include(u => u.Product1)
                                    .ThenInclude(c => c.Manufacturer)
                                .Include(u => u.Product2)
                                    .ThenInclude(c => c.Manufacturer)
                                .Select(r => new RecordViewModel(r)).ToList());
        }
        // команда добавления
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand((o) =>
                  {
                      
                      AddEditRecordSearchWindow recordWindow = new AddEditRecordSearchWindow(new RecordViewModel(),false);
                      if (recordWindow.ShowDialog() == true)
                      {
                          RecordAddEdit(recordWindow.CurrentRecord,true);
                      }
                  }));
            }
        }
      
        // команда обновления
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((selectedItem) =>
                  {

                      if (selectedItem == null) return;
                      // получаем выделенный объект
                      RecordViewModel recordViewModel = selectedItem as RecordViewModel;
                     
                      AddEditRecordSearchWindow recordWindow = new AddEditRecordSearchWindow(recordViewModel, false);
                     
                      if (recordWindow.ShowDialog() == true)
                      {
                          // получаем измененный объект
                          RecordViewModel editRecord=recordWindow.CurrentRecord;                   
                          if (editRecord != null)
                          {
                              RecordAddEdit(recordViewModel, false);
                          }
                      }
                  }));
            }
        }
        // команда удаления
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand((selectedItem) =>
                  {
                      if (selectedItem == null) return;
                      // получаем выделенный объект
                       RecordViewModel recordViewModel = selectedItem as RecordViewModel;
                     
                      Record recordFind=db.Records.Find(recordViewModel.Id);
                      Records.Remove(recordViewModel);
                      db.Records.Remove(recordFind);
                      db.SaveChanges();
                  }));
            }
        }
        //команда поиска маршрутов
        public RelayCommand FindCommand
        {
            get
            {
                return findCommand ??
                  (findCommand = new RelayCommand((o) =>
                  {
                      RecordViewModel recordViewModel = new RecordViewModel();

                      //глубина рекурси по умолчанию 5
                      recordViewModel.Confidence = 5;

                       AddEditRecordSearchWindow recordWindow = new AddEditRecordSearchWindow(recordViewModel, true);
                      
                      if (recordWindow.ShowDialog() == true)
                      {
                          ways = new List<List<Product>>();
                          way = new List<Product>();
                        
                          RecordViewModel newRecord = recordWindow.CurrentRecord;

                          //исходный товар
                          Product productStart = new Product(newRecord.Article1, new Manufacturer(newRecord.Manufacturer1));
                          
                          // искомый товар
                          Product productEnd = new Product(newRecord.Article2, new Manufacturer(newRecord.Manufacturer2));
                          
                          SearchWay(productStart, productEnd, newRecord.Confidence, 0);
                          
                          if (ways.Count == 0)
                          {
                              MessageBox.Show($"Искомый товар { productEnd}  не найден за {newRecord.Confidence} шагов");
                          }
                          else
                          {
                              FoundWaysWindow foundWaysWindow = new FoundWaysWindow(ways, newRecord.Confidence, productEnd);
                              foundWaysWindow.ShowDialog();
                          }
                      }
                  }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Добавление или обновление записи в бд
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="add"></param>
        private void RecordAddEdit(RecordViewModel newRecord, bool add)
        {
           
            Product product1= ProductCheckAdd(newRecord.Manufacturer1, newRecord.Article1);
            Product product2 = ProductCheckAdd(newRecord.Manufacturer2, newRecord.Article2);

            if (Records.Where(rv=>rv.EqualsWithoutId(newRecord) && rv.Id!=newRecord.Id).Count() == 0)
            {
                if (add)
                {

                    Record record = newRecord.GetRecord();
                    db.Entry(record).State = EntityState.Added;
                    db.SaveChanges();

                    newRecord.Id = record.Id;
                    newRecord.Manufacturer1 = product1.Manufacturer.Name;
                    newRecord.Manufacturer2 = product2.Manufacturer.Name;
                    Records.Add(newRecord);

                    //выделение добавленной записи
                    MainWindow mainWindow = Application.Current.Windows[0] as MainWindow;
                    mainWindow.recordsGrid.SelectedIndex=Records.Count-1;
                 
                }

                else
                {
                    Record record = newRecord.GetRecord();
                    record.Id = 0;
                    db.Entry(record).State = EntityState.Added;
                    db.SaveChanges();

                }                 
            }
            else
                MessageBox.Show("Такая запись уже существует!");
           
        }
        /// <summary>
        /// Добавление в бд отсутствующих Приоизводителей и Продуктов
        /// </summary>
        /// <param name="manufacturerName"></param>
        /// <param name="article"></param>
        /// <returns></returns>
        private Product ProductCheckAdd(string manufacturerName, string article)
        {  
            Manufacturer manufacturer = new Manufacturer(manufacturerName);
           
            if (db.Manufactureres.Find(manufacturerName) == null)
            {
                db.Manufactureres.Add(manufacturer);
                db.SaveChanges();
            }
            else
            {
                manufacturer = db.Manufactureres.Find(manufacturerName);
                
            }

            Product product = new Product(article, manufacturer);
           
            if (db.Products.Find(article)==null)
            {
                db.Entry(product).State = EntityState.Added;
                db.SaveChanges();
                
            }
            return product;
        }
        /// <summary>
        /// рекурсивный поиск аналогов
        /// </summary>
        /// <param name="productStart"></param>
        /// <param name="productEnd"></param>
        /// <param name="depthRecursion"></param>
        /// <param name="currentDepthRecursion"></param>
         private void SearchWay(Product productStart, Product productEnd,int depthRecursion,  int currentDepthRecursion)
        {
            if (currentDepthRecursion != depthRecursion)
            {
                // поиск аналогов текущего товара
                var searchRecords = Records.Where(r => r.GetRecord().Product1.Equals(productStart));

                foreach (var searchRecord in searchRecords)
                {
                   //добавление товара в маршрут
                    way.Add(searchRecord.GetRecord().Product1);

                    if (searchRecord.GetRecord().Product2.Equals(productEnd))
                    {
                        //маршрут найден, добавляется последний товар
                        Product firstProduct = way.First();
                        way = way.Distinct().ToList();
                        way.Add(productEnd);
                        ways.Add(way);
                        way = new List<Product>() { firstProduct };
                    }
                    if (searchRecord.Confidence != 0)
                        SearchWay(searchRecord.GetRecord().Product2, productEnd, depthRecursion, currentDepthRecursion + 1);
                    
                }
            
            }

        }
        
    }
}
