﻿using DeveloperStore.Domain.ValueObjects;

namespace DeveloperStore.Domain.Entities;

public sealed class Product
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string Image { get; set; }
    public Rating Rating { get; set; }
}
