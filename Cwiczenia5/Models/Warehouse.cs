using System;
using System.ComponentModel.DataAnnotations;

namespace Cwiczenia5.Models
{
    public class Warehouse
    {
        [Required]
        public int IdWarehouse { get; set; }
        [Required]
        public int IdProduct { get; set; }
        [Required]
        [Range(1,int.MaxValue)]
        public int Amount { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
