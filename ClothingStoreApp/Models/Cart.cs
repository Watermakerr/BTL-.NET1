namespace ClothingStoreApp.Models
{
    public class Cart
    {
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate { get; set; }
    }
}