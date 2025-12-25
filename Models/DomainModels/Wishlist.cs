using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P2WebMVC.Models;
using P2WebMVC.Models.JunctionModels;

public class Wishlist
{
    [Key]
    public Guid WishlistId { get; set; } = Guid.NewGuid();

    public required Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User? Buyer { get; set; }

    public ICollection<WishlistProduct> WishlistProducts { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
