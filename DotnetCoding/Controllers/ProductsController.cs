using Microsoft.AspNetCore.Mvc;
using DotnetCoding.Core.Models;
using DotnetCoding.Services.Interfaces;

namespace DotnetCoding.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IApprovalQueueService _approvalQueueService;
        public ProductsController(IProductService productService, IApprovalQueueService approvalQueueService)
        {
            _productService = productService;
            _approvalQueueService = approvalQueueService;
        }

        /// <summary>
        /// Get the list of product
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProductList()
        {
            var productDetailsList = await _productService.GetAllProducts();
            if(productDetailsList == null)
            {
                return NotFound();
            }
            return Ok(productDetailsList);
        }       
        
         [HttpGet]
        public async Task<IActionResult> GetAllActiveProducts()
        {
            var activeProducts = await _productService.GetActiveProducts();
            if (activeProducts == null || activeProducts.Count() == 0)
            {
                return NotFound("No products found.");
            }
            return Ok(activeProducts);
        }

        /// <summary>
        /// Get details of a specific product by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProduct(id);
            if (product == null) 
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return Ok(product);
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDetails product)
        {
            try
            {
                var result = await _productService.CreateProduct(product);
                if (result != 0)
                {
                    return Ok("Created" + result + " product(s) successfully");
                }

                return BadRequest("Failed to create products");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDetails product)
        {
            if (id != product.Id)
            {
                return BadRequest("Product ID mismatch.");
            }

            try
            {
                var result = await _productService.UpdateProduct(product);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete a product by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(ProductDetails product)
        {
            try
            {
                var result = await _productService.DeleteProduct(product);
                if (result == 0) return BadRequest("Failed to delete product");
                return Ok("Successfully delete the item");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
