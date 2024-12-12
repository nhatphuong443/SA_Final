using System.ComponentModel.DataAnnotations;

namespace Authentication.Helper
{
    public abstract class ItemFactory
    {
        public abstract Item CreateItem(int id, string description, string category, int quantity, double price, double discount);
    }

    public class ConcreteItemFactory : ItemFactory
    {
        public override Item CreateItem(int id, string description, string category, int quantity, double price, double discount)
        {
            return new Item
            {
                Id = id,
                Description = description,
                Category = category,
                Quantity = quantity,
                Price = price,
                Discount = discount
            };
        }
    }
    public class Item
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }

        [Display(Name = "Old Price")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public double Discount { get; set; }

        [Display(Name = "New Price")]
        [DataType(DataType.Currency)]
        public double NewPrice
        {
            get
            {
                return Price * (1 - Discount);
            }
        }

        [DataType(DataType.Currency)]
        public double Amount
        {
            get
            {
                return Quantity * NewPrice;
            }
        }
    }
}
