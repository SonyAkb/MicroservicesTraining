using Microsoft.EntityFrameworkCore;
using ProductService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductDbContext>(options => options.UseInMemoryDatabase("ProductsDB"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/products", async (ProductDbContext db) => await db.Products.ToListAsync());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Метод 1: Получить продукт по ID
app.MapGet("/products/{id}", async (int id, ProductDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    return product == null ? Results.NotFound() : Results.Ok(product);
});

// Метод 2: Добавить новый продукт
app.MapPost("/products", async (Product product, ProductDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

// Метод 3: Изменить количество товара на складе
app.MapPut("/products/{id}/stock", async (int id, int stock, ProductDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null) return Results.NotFound();

    product.Stock = stock;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();
