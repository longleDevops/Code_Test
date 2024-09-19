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
        /// Search for products with filtering options
        /// </summary>
        /// <param name="productName">Name of the product</param>
        /// <param name="minPrice">Minimum price</param>
        /// <param name="maxPrice">Maximum price</param>
        /// <param name="startDate">Start date for creation date</param>
        /// <param name="endDate">End date for creation date</param>
        /// <returns>List of filtered products</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string productName,
            [FromQuery] int? minPrice,
            [FromQuery] int? maxPrice,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var products = await _productService.GetFilteredProducts(productName, minPrice, maxPrice, startDate, endDate);
            return Ok(products);
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
        
        [HttpGet("approval-queue")]
        public async Task<IActionResult> GetApprovalQueue()
        {
            var approvalQueue = await _approvalQueueService.GetAllQueueItems();
            return Ok(approvalQueue);
        }

        [HttpPost("approve-reject")]
        public async Task<IActionResult> ApproveOrReject([FromBody] ApprovalRequestModel request)
        {
            if (request == null || !ModelState.IsValid)
                return BadRequest("Invalid request.");

            try
            {
                await _approvalQueueService.ApproveOrReject(request.ApprovalQueueId, request.Approve);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
