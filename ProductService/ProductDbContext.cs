﻿using Microsoft.EntityFrameworkCore;

namespace ProductService
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; } // таблица продуктов
    }
}
