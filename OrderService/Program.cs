using Microsoft.EntityFrameworkCore;
using OrderService;
using Serilog;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// ��� ������ �������
builder.Host.UseSerilog((ctx, services, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderDbContext>(options => options.UseInMemoryDatabase("OrdersDB")); // ��������� In-Memory ���� ������ ��� �������
// ����� ����������� HttpClientFactory ��� ������ ProductService
builder.Services.AddHttpClient("ProductServiceClient", client => { client.BaseAddress = new Uri("https://localhost:7094");});

builder.Host.UseSerilog((ctx, cfg) => cfg.WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// �������� ��� ������������ ������
app.MapGet("/orders", async (OrderDbContext db) =>
{
    Log.Information("Requesting a list of all orders");

    try
    {
        var orders = await db.Orders.ToListAsync();
        Log.Information($"Orders found {orders.Count}");

        return Results.Ok(orders);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error when receiving the orders list");
        return Results.Problem("��������� ������ ��� ��������� �������");
    }
});

// ������� �����
app.MapPost("/orders", async (Order order, OrderDbContext db, IHttpClientFactory clientFactory) =>
{
    Log.Information("Creating order for product ", order.ProductId);
    if (order.Quantity <= 0)
    {
        Log.Warning("Incorrect Quanity: ", order.Quantity);
        return Results.BadRequest("Quantity must be greater than 0");
    }
    try
    {
        // ���������� �����
        var httpClient = clientFactory.CreateClient("ProductServiceClient");
        var productResponse = await httpClient.GetAsync($"/products/{order.ProductId}");

        // ���� ������ ���, �� ������ (404)
        if (!productResponse.IsSuccessStatusCode)
        {
            Log.Warning($"Product {order.ProductId} not found");
            return Results.NotFound($"Product {order.ProductId} not found");
        }

        // �������� ������� �� �� ������
        var product = await productResponse.Content.ReadFromJsonAsync<ProductService.Product>();
        if (product != null)
        {
            if (product.Stock < order.Quantity)
            {
                Log.Warning($"Not enough stock");
                return Results.BadRequest($"Not enough stock. Available: {product.Stock}");
            }
        }

        // �������� �����
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        Log.Information($"The order has been created. ID: {order.Id}");
        return Results.Created($"/orders/{order.Id}", order);
    }
    catch (Exception ex)
    {
        Log.Information("Error when creating an order");
        return Results.Problem($"Error: {ex.Message}");
    }

});

// �������� ����� �� ID
app.MapGet("/orders/{id}", async (int id, OrderDbContext db) =>
    await db.Orders.FindAsync(id) is Order order 
    ? Results.Ok(order) : Results.NotFound());

Log.Information($"������ {"OrderService"} �� �������: {string.Join(", ", app.Urls)}");
app.UseSerilogRequestLogging();

app.Run();

// ��� �������������� ������ �� ProductService
namespace ProductService
{
    public class Product { public int Stock { get; set; } }
}