namespace Real_Estate_Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Real_Estate_Api.DTOs;
    using Real_Estate_Api.Services;
    using Microsoft.AspNetCore.Authorization;

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
            var listings = _listingService.GetFilteredListings(filter);
            return Ok(listings);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] CreateListingDto dto)
        {
            var result = _listingService.CreateListing(dto, User);
            return CreatedAtAction(nameof(GetAll), result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateListingDto dto)
        {
            var result = _listingService.UpdateListing(id, dto, User);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _listingService.DeleteListing(id, User);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result);
        }
    }

}