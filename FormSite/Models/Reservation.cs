using System.ComponentModel.DataAnnotations;

namespace FormSite.Models;

public class Reservation
{
    public int Id { get; set; }

    [Required]
    public int OptionId { get; set; }

    public string? UserToken { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Option? Option { get; set; }
}
