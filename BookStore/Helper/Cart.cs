
namespace Authentication.Helper
{
//Singleton Pattern
public class Cart
    {
        private static Cart instance;
        private SortedList<int, Item> list;

        private Cart()
        {
            list = new SortedList<int, Item>();
        }

        public static Cart Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Cart();
                }
                return instance;
            }
        }

        public SortedList<int, Item> List
        {
            get
            {
                return list;
            }
        }

        public void Add(Item item)
        {
            if (List.ContainsKey(item.Id))
            {
                Item currentItem = List[item.Id];
                currentItem.Quantity += item.Quantity;
            }
            else
            {
                List.Add(item.Id, item);
            }
        }

        public void Remove(int id)
        {
            List.Remove(id);
        }

        public void Empty()
        {
            List.Clear();
        }

        public void Update(int id, int quantity)
        {
            Item item = List[id];
            if (item != null)
            {
                if (quantity <= 0)
                    Remove(id);
                else
                    item.Quantity = quantity;
            }
        }

        public double TotalAmount
        {
            get
            {
                return List.Values.Sum(item => item.Amount);
            }
        }
    }
}