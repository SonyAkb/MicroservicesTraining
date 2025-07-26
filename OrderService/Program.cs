using Microsoft.EntityFrameworkCore;
using OrderService;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// все нужные сервисы

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderDbContext>(options => options.UseInMemoryDatabase("OrdersDB")); // настройка In-Memory базы данных для заказов
// явная регистрация HttpClientFactory для вызова ProductService
builder.Services.AddHttpClient("ProductServiceClient", client => { client.BaseAddress = new Uri("https://localhost:7094");});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// получить все существующие заказы
app.MapGet("/orders", async (OrderDbContext db) => await db.Orders.ToListAsync());

// создать заказ
app.MapPost("/orders", async (Order order, OrderDbContext db, IHttpClientFactory clientFactory) =>
{
    if (order.Quantity <= 0)
        return Results.BadRequest("Quantity must be greater than 0");
    try
    {
        // запрашиваю товар
        var httpClient = clientFactory.CreateClient("ProductServiceClient");
        var productResponse = await httpClient.GetAsync($"/products/{order.ProductId}");

        // если товара нет, то ошибка (404)
        if (!productResponse.IsSuccessStatusCode)
            return Results.NotFound($"Product {order.ProductId} not found");

        // проверяю хватает ли на складе
        var product = await productResponse.Content.ReadFromJsonAsync<ProductService.Product>();
        if (product != null)
        {
            if (product.Stock < order.Quantity)
            {
                return Results.BadRequest($"Not enough stock. Available: {product.Stock}");
            }
        }

        // сохраняю заказ
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return Results.Created($"/orders/{order.Id}", order);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }

});

// получить заказ по ID
app.MapGet("/orders/{id}", async (int id, OrderDbContext db) =>
    await db.Orders.FindAsync(id) is Order order 
    ? Results.Ok(order) : Results.NotFound());

app.Run();

// для десериализации ответа от ProductService
namespace ProductService
{
    public class Product { public int Stock { get; set; } }
}