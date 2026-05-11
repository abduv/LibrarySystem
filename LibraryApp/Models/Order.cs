using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string UserLogin { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Статус по умолчанию
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Связь с книгой
        public virtual Book? Book { get; set; }
    }
}