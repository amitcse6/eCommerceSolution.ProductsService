using AutoMapper;
using DataAccessLayer.Entities;
using eCommerce.BusinessLogicLayer.DTO;
using eCommerce.BusinessLogicLayer.ServiceContracts;
using eCommerce.DataAccessLayer.RepositoryContracts;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services;

public class ProductService : IProductsService
{
    private readonly IValidator<ProductAddRequest> _validationService;
    private readonly IValidator<ProductUpdateRequest> _updateValidationService;

    private readonly IMapper _mapper;
    private readonly IProductsRepository _productsRepository;

    public ProductService(IValidator<ProductAddRequest> validationService,
        IValidator<ProductUpdateRequest> updateValidationService,
        IMapper mapper,
        IProductsRepository productsRepository)
    {
        _validationService = validationService;
        _updateValidationService = updateValidationService;
        _mapper = mapper;
        _productsRepository = productsRepository;
    }

    public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
    {
        if (productAddRequest is null)
        {
            throw new ArgumentNullException(nameof(productAddRequest));
        }

        var validationResult = await _validationService.ValidateAsync(productAddRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            throw new ArgumentException($"ProductAddRequest validation failed: {string.Join(", ", errors)}");
        }
        
        Product priductInput = _mapper.Map<Product>(productAddRequest);
        Product? priductFromDb = await _productsRepository.AddProduct(priductInput);
        ProductResponse? productResponse = _mapper.Map<ProductResponse?>(priductFromDb);
        return productResponse;
    }

    public async Task<bool> DeleteProduct(Guid productID)
    {
        if (productID == Guid.Empty)
        {
            throw new ArgumentException("ProductID cannot be empty.", nameof(productID));
        }
        Product? existingProduct = await _productsRepository.GetProductByCondition(p => p.ProductID == productID);
        if (existingProduct == null)
        {
            throw new ArgumentException("Product not found.", nameof(productID));
        }
        bool isDeleted = await _productsRepository.DeleteProduct(productID);
        return isDeleted;
    }

    public async Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        if (conditionExpression == null)
        {
            throw new ArgumentNullException(nameof(conditionExpression));
        }
        Product? productFromDb = await _productsRepository.GetProductByCondition(conditionExpression);
        ProductResponse? productResponse = _mapper.Map<ProductResponse?>(productFromDb);
        return productResponse;
    }

    public async Task<List<ProductResponse?>> GetProducts()
    {
        IEnumerable<Product>? productsFromDb = await _productsRepository.GetProducts();
        IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse?>>(productsFromDb);
        return productResponses.ToList();
    }

    public async Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        IEnumerable<Product?>? productsFromDb = await _productsRepository.GetProductsByCondition(conditionExpression);
        IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse?>>(productsFromDb);
        return productResponses.ToList();
    }

    public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest)
    {
        if (productUpdateRequest is null)
        {
            throw new ArgumentNullException(nameof(productUpdateRequest));
        }
        var validationResult = await _updateValidationService.ValidateAsync(productUpdateRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            throw new ArgumentException($"ProductUpdateRequest validation failed: {string.Join(", ", errors)}");
        }
        Product productInput = _mapper.Map<Product>(productUpdateRequest);
        Product? updatedProductFromDb = await _productsRepository.UpdateProduct(productInput);
        ProductResponse? productResponse = _mapper.Map<ProductResponse?>(updatedProductFromDb);
        return productResponse;
    }
}
