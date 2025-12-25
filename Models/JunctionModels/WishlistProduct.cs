using P2WebMVC.Models.DomainModels;

public class WishlistProduct
{
    public Guid WishlistId { get; set; }
    public Wishlist? Wishlist { get; set; }

    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
}
