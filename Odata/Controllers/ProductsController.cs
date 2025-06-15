// Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Odata.Entities;
using Odata;

namespace Odata.Controllers;

public class ProductsController : ODataController
{
    private readonly AppDbContext _context;
    public ProductsController(AppDbContext context) => _context = context;

    [EnableQuery]
    public IQueryable<Product> Get() => _context.Products;

    [EnableQuery]
    public SingleResult<Product> Get([FromODataUri] int key) =>
        SingleResult.Create(_context.Products.Where(p => p.Id == key));

    public async Task<IActionResult> Post([FromBody] Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return Created(product);
    }

    public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<Product> productDelta)
    {
        var product = await _context.Products.FindAsync(key);
        if (product == null) return NotFound();

        productDelta.Patch(product);
        await _context.SaveChangesAsync();
        return Updated(product);
    }

    public async Task<IActionResult> Delete([FromODataUri] int key)
    {
        var product = await _context.Products.FindAsync(key);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
