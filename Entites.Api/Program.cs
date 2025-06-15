// Program.cs
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using Odata.Entities;
using Odata;
using Microsoft.EntityFrameworkCore;
using Entities.Business;
using Entities.Business.Services;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProduct, ProductService>(); // Register the service here
// OData setup
var odataBuilder = new ODataConventionModelBuilder();
odataBuilder.EntitySet<Product>("Products");

builder.Services.AddControllers().AddOData(opt =>
    opt.AddRouteComponents("odata", odataBuilder.GetEdmModel())
       .Filter()
       .Select()
       .Expand()
       .OrderBy()
       .Count()
       .SetMaxTop(100));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
