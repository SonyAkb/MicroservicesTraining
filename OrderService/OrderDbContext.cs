﻿using Microsoft.EntityFrameworkCore;

namespace OrderService
{
    public class OrderDbContext: DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
        public DbSet<Order> Orders { get; set; }
    }
}
