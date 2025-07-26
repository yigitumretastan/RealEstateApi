namespace Real_Estate_Api.Models
{
    public class Listing
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string City { get; set; }

        public int RoomCount { get; set; }

        public decimal Price { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}
