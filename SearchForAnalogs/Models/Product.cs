using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SearchForAnalogs.Models
{
    public class Product
    {
        [Key]
        public string Article { get; set; }


        [Required]
        public Manufacturer Manufacturer { get; set; }


        public List<Record> Records1 { get; set; } = new List<Record>();


        public List<Record> Records2 { get; set; } = new List<Record>();


        public Product()
        {

        }
        public Product(string article, Manufacturer manufacturer)
        {
            Article = article;
            Manufacturer = manufacturer;
        }

        public override string ToString()
        {
            return $"({Article} + {Manufacturer.Name})";
        }
        public override bool Equals(object obj)
        {
            if (obj == null && obj.GetType() != this.GetType()) return false;

            Product product = (Product)obj;
           string article1String = String.Concat<char>(Article.Where(c => !char.IsSeparator(c)
                  && !char.IsPunctuation(c))).ToLower();
            string article2String = String.Concat<char>(product.Article.Where(c => !char.IsSeparator(c)
                  && !char.IsPunctuation(c))).ToLower();
          
            return article1String==article2String &&
                Manufacturer.Name.ToLower() == product.Manufacturer.Name.ToLower();
        }

       
    }
}
