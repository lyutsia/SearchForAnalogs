using System;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using SearchForAnalogs.Models;

namespace SearchForAnalogs
{
    public class RecordViewModel : INotifyPropertyChanged
    {
        private Record record;
        public RecordViewModel() {
            Product product1 = new Product("", new Manufacturer(""));
            Product product2 = new Product("", new Manufacturer(""));
            record = new Record(product1,product2,0);
        }
        public RecordViewModel(Record record)
        {
            this.record = record;
        }
        public int Id
        {

            get { return record.Id; }
            set
            {
                record.Id = value;
                OnPropertyChanged("Id");
            }
        }
        public string Article1
        {
            get { return record.Product1.Article; }
            set
            {
                record.Product1.Article = value;
                OnPropertyChanged("Article1");
            }
        }
        public string Manufacturer1
        {
            get { return record.Product1.Manufacturer.Name; }
            set
            {
                record.Product1.Manufacturer.Name = value;
                OnPropertyChanged("Manufacturer1");
            }
        }
        public string Article2
        {
            get { return record.Product2.Article; }
            set
            {
                record.Product2.Article = value;
                OnPropertyChanged("Article2");
            }
        }
        public string Manufacturer2
        {
            get { return record.Product2.Manufacturer.Name; }
            set
            {
                record.Product2.Manufacturer.Name = value;
                OnPropertyChanged("Manufacturer2");
            }
        }
        public int Confidence
        {
            get { return record.Confidence; }
            set
            {
                record.Confidence = value;
                OnPropertyChanged("Confidence");
            }
        }

        public Record GetRecord()
        {
            Product product1 = new Product(Article1, new Manufacturer(Manufacturer1));
            Product product2 = new Product(Article2, new Manufacturer(Manufacturer2));
            return new Record(Id,product1, product2, Confidence);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Сравнение записей без Id для исключения повторяющихся записей
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool EqualsWithoutId(RecordViewModel record)
        {
            return Confidence == record.Confidence && Article1 == record.Article1
                && Manufacturer1 == record.Manufacturer1 && Article2 == record.Article2
                && Manufacturer2 == record.Manufacturer2;
        }
       
    }
}
