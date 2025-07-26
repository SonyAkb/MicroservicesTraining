namespace ProductService
{
    public class Product // структура продукта
    {
        public int Id { get; set; }
        public string Name { get; set; } // имя продукта
        public decimal Price { get; set; } // цена
        public int Stock { get; set; } // количество на складе

    }
}
