namespace OrderService
{
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }  // id товара который заказан
        public int Quantity { get; set; }   // количество товаров
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // дата
    }
}
