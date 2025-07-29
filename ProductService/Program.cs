using Microsoft.EntityFrameworkCore;
using ProductService;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, services, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddDbContext<ProductDbContext>(options => options.UseInMemoryDatabase("ProductsDB"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((ctx, cfg) => cfg.WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var options = new JsonSerializerOptions // чтоб избежать ошибки из за того что в файле .json используется сamelCase
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // автоматически преобразует: Name -> name
};

var productsJson = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "products.json")); // сразу загрузка некоторых продуктов

var products = JsonSerializer.Deserialize<List<Product>>(productsJson, options);
if (products != null)
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
        if (!db.Products.Any())
        {
            db.Products.AddRange(products);
            db.SaveChanges();
        }
    }
}

// Получить все существующие продукты
app.MapGet("/products", async (ProductDbContext db) =>
{
    Log.Information("Requesting a list of all products");

    try
    {
        var products = await db.Products.ToListAsync();
        Log.Information($"Products found {products.Count}");

        return Results.Ok(products);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error when receiving the product list");
        return Results.Problem("An error occurred while processing the request");
    }
});

// Получить продукт по ID
app.MapGet("/products/{id}", async (int id, ProductDbContext db) =>
{
    Log.Information($"Requesting a product with an ID: {id}");
    var product = await db.Products.FindAsync(id);

    if (product == null)
    {
        Log.Warning($"Product with ID not found {id}");
        return Results.NotFound();
    }

    Log.Debug($"Returning the product: {product}");
    return product == null ? Results.NotFound() : Results.Ok(product);
});

// Добавить новый продукт
app.MapPost("/products", async (Product product, ProductDbContext db) =>
{
    Log.Information($"Creating a new product: {product}");
    if (product.Price <= 0)
    {
        Log.Warning($"Incorrect price: {product.Price}");
        return Results.BadRequest("Price must be greater than 0");
    }

    if (product.Stock < 0)
    {
        Log.Warning($"Incorrect quantity: {product.Stock}");
        return Results.BadRequest("Stock cannot be negative");
    }

    db.Products.Add(product);
    await db.SaveChangesAsync();
    Log.Information($"The product has been created. ID: {product.Id}");

    return Results.Created($"/products/{product.Id}", product);
});

// Изменить количество товара на складе
app.MapPut("/products/{id}/stock", async (int id, int stock, ProductDbContext db) =>
{
    Log.Information($"Сhanging the product quantity: {id}");
    if (stock < 0)
    {
        Log.Warning($"Incorrect quantity: {stock}");
        return Results.BadRequest("Stock cannot be negative");
    }
    var product = await db.Products.FindAsync(id);
    if (product == null) return Results.NotFound();

    product.Stock = stock;
    await db.SaveChangesAsync();

    Log.Information($"The quantity has been changed. New quantity: {product.Stock} Product name: {product.Name}");

    return Results.NoContent();
});

Log.Information($"Launching ProductService at addresses: {string.Join(", ", app.Urls)}");
app.UseSerilogRequestLogging();

app.Run();
