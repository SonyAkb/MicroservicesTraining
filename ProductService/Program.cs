using Microsoft.EntityFrameworkCore;
using ProductService;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductDbContext>(options => options.UseInMemoryDatabase("ProductsDB"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var options = new JsonSerializerOptions // чтоб избежать ошибки из за того что в файле .json используется сamelCase
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // автоматически преобразует "Name" → "name"
};

var productsJson = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "products.json"));

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
app.MapGet("/products", async (ProductDbContext db) => await db.Products.ToListAsync());

// Получить продукт по ID
app.MapGet("/products/{id}", async (int id, ProductDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    return product == null ? Results.NotFound() : Results.Ok(product);
});

// Добавить новый продукт
app.MapPost("/products", async (Product product, ProductDbContext db) =>
{
    if (product.Price <= 0)
        return Results.BadRequest("Price must be greater than 0");

    if (product.Stock < 0)
        return Results.BadRequest("Stock cannot be negative");
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

// Изменить количество товара на складе
app.MapPut("/products/{id}/stock", async (int id, int stock, ProductDbContext db) =>
{
    if (stock < 0)
        return Results.BadRequest("Stock cannot be negative");
    var product = await db.Products.FindAsync(id);
    if (product == null) return Results.NotFound();

    product.Stock = stock;
    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.Run();
