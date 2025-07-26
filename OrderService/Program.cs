using Microsoft.EntityFrameworkCore;
using OrderService;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// ��� ������ �������

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderDbContext>(options => options.UseInMemoryDatabase("OrdersDB")); // ��������� In-Memory ���� ������ ��� �������
// ����� ����������� HttpClientFactory ��� ������ ProductService
builder.Services.AddHttpClient("ProductServiceClient", client => { client.BaseAddress = new Uri("https://localhost:7094");});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// �������� ��� ������������ ������
app.MapGet("/orders", async (OrderDbContext db) => await db.Orders.ToListAsync());

// ������� �����
app.MapPost("/orders", async (Order order, OrderDbContext db, IHttpClientFactory clientFactory) =>
{
    if (order.Quantity <= 0)
        return Results.BadRequest("Quantity must be greater than 0");
    try
    {
        // ���������� �����
        var httpClient = clientFactory.CreateClient("ProductServiceClient");
        var productResponse = await httpClient.GetAsync($"/products/{order.ProductId}");

        // ���� ������ ���, �� ������ (404)
        if (!productResponse.IsSuccessStatusCode)
            return Results.NotFound($"Product {order.ProductId} not found");

        // �������� ������� �� �� ������
        var product = await productResponse.Content.ReadFromJsonAsync<ProductService.Product>();
        if (product != null)
        {
            if (product.Stock < order.Quantity)
            {
                return Results.BadRequest($"Not enough stock. Available: {product.Stock}");
            }
        }

        // �������� �����
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return Results.Created($"/orders/{order.Id}", order);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }

});

// �������� ����� �� ID
app.MapGet("/orders/{id}", async (int id, OrderDbContext db) =>
    await db.Orders.FindAsync(id) is Order order 
    ? Results.Ok(order) : Results.NotFound());

app.Run();

// ��� �������������� ������ �� ProductService
namespace ProductService
{
    public class Product { public int Stock { get; set; } }
}