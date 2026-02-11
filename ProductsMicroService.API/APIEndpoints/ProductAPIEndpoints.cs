using eCommerce.BusinessLogicLayer.DTO;
using eCommerce.BusinessLogicLayer.ServiceContracts;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ProductsMicroService.API.APIEndpoints;

public static class ProductAPIEndpoints
{
    public static IEndpointRouteBuilder MapProductAPIEndpoints(this IEndpointRouteBuilder app)
    {
        //GET //api/products
        app.MapGet("/api/products", async (IProductsService productsService) =>
        {
            var products = await productsService.GetProducts();
            return Results.Ok(products);
        });

        //GET //api/products/search/product-id/XXXXXXXXXXXXXXXXX
        app.MapGet("/api/products/search/product-id/{productID:guid}", async (Guid productID, IProductsService productsService) =>
        {
            var product = await productsService.GetProductByCondition(p => p.ProductID == productID);
            if (product is null)
                return Results.NotFound();
            return Results.Ok(product);
        });

        //GET //api/products/search/XXXXXXXXXXXXXXXXXXXXX
        app.MapGet("/api/products/search/{SearchString}", async (string SearchString, IProductsService productsService) =>
        {
            string searchLower = SearchString.ToLower();
            List<ProductResponse?> productsByProductName = await productsService.GetProductsByCondition(p => p.ProductName != null && p.ProductName.ToLower().Contains(searchLower));
            List<ProductResponse?> productsByCategory = await productsService.GetProductsByCondition(p => p.Category != null && p.Category.ToLower().Contains(searchLower));
            var products = productsByProductName.Union(productsByCategory).ToList();
            return Results.Ok(products);
        });

        //POST //api/products
        app.MapPost("/api/products", async ([FromBody] ProductAddRequest productAddRequest, IValidator<ProductAddRequest> validator, IProductsService productsService) =>
        {
            var validationResult = await validator.ValidateAsync(productAddRequest);
            if (!validationResult.IsValid)
            {
                Dictionary<string, string[]> errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }

            var addedProduct = await productsService.AddProduct(productAddRequest);
            if (addedProduct is null)
                return Results.Problem("An error occurred while adding the product. Please try again.");
            return Results.Created($"/api/products/search/product-id/{addedProduct.ProductID}", addedProduct);
        });

        //PUT //api/products
        app.MapPut("/api/products", async ([FromBody] ProductUpdateRequest productUpdateRequest, IValidator<ProductUpdateRequest> validator, IProductsService productsService) =>
        {
            var validationResult = await validator.ValidateAsync(productUpdateRequest);
            if (!validationResult.IsValid)
            {
                Dictionary<string, string[]> errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }
            var updatedProduct = await productsService.UpdateProduct(productUpdateRequest);
            if (updatedProduct is null)
                return Results.Problem("An error occurred while updating the product. Please try again.");
            return Results.Ok(updatedProduct);
        });

        return app;
    }
}
