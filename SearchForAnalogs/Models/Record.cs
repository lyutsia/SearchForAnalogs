using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SearchForAnalogs.Models
{
    public class Record
    {
        public int Id { get; set; }

        [Required]
        public Product Product1 { get; set; } 
        [Required]
        public Product Product2 { get; set; }

        [Required]
        public int Confidence { get; set; }
        
        public Record() {
           
        }
       
        public Record(Product product1, Product product2, int confidence)
        {
            Product1 = product1;
            Product2 = product2;
            Confidence = confidence;
        }
        public Record(int id,Product product1, Product product2, int confidence):this(product1,product2,confidence)
        {
            Id = id;
        }

       
    }
}
