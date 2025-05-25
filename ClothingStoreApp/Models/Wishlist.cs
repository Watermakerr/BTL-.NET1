namespace ClothingStoreApp.Models
{
    public class Wishlist
    {
        public int WishlistID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public DateTime AddedDate { get; set; }
    }
}