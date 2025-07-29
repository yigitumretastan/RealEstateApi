namespace RealEstateApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RealEstateApi.DTOs;
    using RealEstateApi.Services;
    using Microsoft.AspNetCore.Authorization;
    using System.Security.Claims;

    [ApiController]
    [Route("api/[controller]")]
    public class ListingsController : ControllerBase
    {
        private readonly ListingService _listingService;

        public ListingsController(ListingService listingService)
        {
            _listingService = listingService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] ListingFilterDto filter)
        {
            try
            {
                var result = _listingService.GetFilteredListings(filter);
                return Ok(new
                {
                    success = true,
                    data = result.Listings,
                    pagination = new
                    {
                        currentPage = result.Page,
                        pageSize = result.PageSize,
                        totalPages = result.TotalPages,
                        totalCount = result.TotalCount
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ GetAll Error: " + ex.Message);
                return StatusCode(500, new { success = false, message = "Server error", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var listing = _listingService.GetListingById(id);
                if (listing == null)
                    return NotFound(new { success = false, message = "Listing not found"});

                return Ok(new { success = true, data = listing });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ GetById Error: " + ex.Message);
                return StatusCode(500, new { success = false, message = "Server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("my-listings")]
        public IActionResult GetMyListings()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var listings = _listingService.GetUserListings(userId);
                return Ok(new { success = true, data = listings });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ GetMyListings Error: " + ex.Message);
                return StatusCode(500, new { success = false, message = "Server Error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] CreateListingDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid data",
                        errors = errors
                    });
                }

                var listing = _listingService.CreateListing(dto, User);
                return CreatedAtAction(nameof(GetById), new { id = listing.Id },
                    new { success = true, message = "Listing created successfully", data = listing });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Create Error: " + ex.Message);
                return StatusCode(500, new { success = false, message = "Server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateListingDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid data",
                        errors = errors
                    });
                }

                var result = _listingService.UpdateListing(id, dto, User);
                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Update Error: " + ex.Message);
                return StatusCode(500, new { success = false, message = "Server Error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _listingService.DeleteListing(id, User);
                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Delete Error: " + ex.Message);
                return StatusCode(500, new { success = false, message = "Server Error", error = ex.Message });
            }
        }

        [HttpGet("sorted-by-price")]
        public IActionResult GetListingsSortedByPrice()
        {
            try
            {
                var listings = _listingService.GetListingsOrderedByPrice();
                return Ok(new { success = true, data = listings });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ GetListingsSortedByPrice Error: " + ex.Message);
                return StatusCode(500, new { success = false, message = "Server Error", error = ex.Message });
            }
        }

    }
}