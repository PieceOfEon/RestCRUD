using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<Product> products = new List<Product>();

app.MapPost("/api/products", (HttpContext context) =>
{
    var form = context.Request.Form;
    var product = new Product
    {
        Id = products.Count + 1,
        Name = form["name"],
        Description = form["description"],
        Price = int.Parse(form["price"]),
        ImageUrl = form["imageUrl"],
        Quantity = int.Parse(form["quantity"])
    };

    products.Add(product);

    context.Response.StatusCode = 201; // Created
    return context.Response.WriteAsync("Product created successfully.");
});

app.MapGet("/api/products", (HttpContext context) =>
{
    context.Response.StatusCode = 200; // OK
    return context.Response.WriteAsJsonAsync(products);
});

app.MapGet("/api/products/{id}", (HttpContext context) =>
{
    int id = int.Parse(context.Request.RouteValues["id"].ToString());
    var product = products.FirstOrDefault(p => p.Id == id);

    if (product != null)
    {
        context.Response.StatusCode = 200; // OK
        return context.Response.WriteAsJsonAsync(product);
    }

    context.Response.StatusCode = 404; // Not Found
    return context.Response.WriteAsync("Product not found.");
});

app.MapPut("/api/products/{id}", (HttpContext context) =>
{
    int id = int.Parse(context.Request.RouteValues["id"].ToString());
    var product = products.FirstOrDefault(p => p.Id == id);

    if (product != null)
    {
        var form = context.Request.Form;
        if (int.TryParse(form["quantity"], out int quantity))
        {
            if (quantity > product.Quantity)
            {
                // В наличии нет столько товара
                context.Response.StatusCode = 400; // Bad Request
                return context.Response.WriteAsync("Insufficient quantity in stock.");
            }

            product.Quantity -= quantity;

            context.Response.StatusCode = 200; // OK
            return context.Response.WriteAsync("Product updated successfully.");
        }
        else
        {
            // Обработка некорректного значения количества
            context.Response.StatusCode = 400; // Bad Request
            return context.Response.WriteAsync("Invalid quantity value.");
        }
    }

    context.Response.StatusCode = 404; // Not Found
    return context.Response.WriteAsync("Product not found.");
});

app.MapDelete("/api/products/{id}", (HttpContext context) =>
{
    int id = int.Parse(context.Request.RouteValues["id"].ToString());
    var product = products.FirstOrDefault(p => p.Id == id);

    if (product != null)
    {
        products.Remove(product);

        context.Response.StatusCode = 200; // OK
        return context.Response.WriteAsync("Product deleted successfully.");
    }

    context.Response.StatusCode = 404; // Not Found
    return context.Response.WriteAsync("Product not found.");
});

app.MapGet("/", (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    var indexPath = Path.Combine(Environment.CurrentDirectory, "html/index.html");
    return context.Response.SendFileAsync(indexPath);
});

app.Run();

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public string ImageUrl { get; set; }
    public int Quantity { get; set; }
}
