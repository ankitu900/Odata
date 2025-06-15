// Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using Odata.Entities;
using System.Threading.Tasks;
using System.Linq;
using Entities.Business.Services;

namespace Odata.Controllers;

public class ProductsController : ODataController
{
    private readonly IProduct _productService;

    public ProductsController(IProduct productService)
    {
        _productService = productService;
    }

    [EnableQuery]
    public IQueryable<Product> Get()
    {
        return _productService.GetAllProducts().AsQueryable();
    }

    [EnableQuery]
    public SingleResult<Product> Get([FromODataUri] int key)
    {
        var product = _productService.GetProductById(key);
        return SingleResult.Create(new[] { product }.AsQueryable());
    }

    public IActionResult Post([FromBody] Product product)
    {
        _productService.CreateProduct(product);
        return Created(product);
    }

    public IActionResult Patch([FromODataUri] int key, [FromBody] Delta<Product> productDelta)
    {
        var product = _productService.GetProductById(key);
        if (product == null) return NotFound();

        productDelta.Patch(product);
        _productService.UpdateProduct(key, product);
        return Updated(product);
    }

    public IActionResult Delete([FromODataUri] int key)
    {
        var product = _productService.GetProductById(key);
        if (product == null) return NotFound();

        _productService.DeleteProduct(key);
        return NoContent();
    }
}
