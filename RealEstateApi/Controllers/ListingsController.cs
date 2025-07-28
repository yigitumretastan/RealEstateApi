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

        /// <summary>
        /// Tüm ilanları filtreli olarak getir
        /// </summary>
        [HttpGet]
        public IActionResult GetAll([FromQuery] ListingFilterDto filter)
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

        /// <summary>
        /// Belirli bir ilanı ID ile getir
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var listing = _listingService.GetListingById(id);
            if (listing == null)
                return NotFound(new { success = false, message = "İlan bulunamadı" });

            return Ok(new { success = true, data = listing });
        }

        /// <summary>
        /// Kullanıcının kendi ilanlarını getir
        /// </summary>
        [Authorize]
        [HttpGet("my-listings")]
        public IActionResult GetMyListings()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var listings = _listingService.GetUserListings(userId);
            return Ok(new { success = true, data = listings });
        }

        /// <summary>
        /// Yeni ilan oluştur
        /// </summary>
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] CreateListingDto dto)
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
                    message = "Geçersiz veriler",
                    errors = errors
                });
            }

            var listing = _listingService.CreateListing(dto, User);
            return CreatedAtAction(nameof(GetById), new { id = listing.Id },
                new { success = true, message = "İlan başarıyla oluşturuldu", data = listing });
        }

        /// <summary>
        /// İlan güncelle
        /// </summary>
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateListingDto dto)
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
                    message = "Geçersiz veriler",
                    errors = errors
                });
            }

            var result = _listingService.UpdateListing(id, dto, User);
            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new { success = true, message = result.Message });
        }

        /// <summary>
        /// İlan sil
        /// </summary>
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _listingService.DeleteListing(id, User);
            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new { success = true, message = result.Message });
        }

        /// <summary>
        /// İlan istatistiklerini getir
        /// </summary>
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            var stats = _listingService.GetListingStats();
            return Ok(new { success = true, data = stats });
        }
    }
}