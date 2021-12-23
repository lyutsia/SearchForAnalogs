using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SearchForAnalogs.Models
{
    public class Manufacturer
    {
        [Key]
        public string Name { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public Manufacturer() {
           
        }
        public Manufacturer(string name)
        {
            Name = name;
        }
    }
}
