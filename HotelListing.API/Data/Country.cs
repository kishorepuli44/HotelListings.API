using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Data;

public class  Country
{
    [Key]
    public int CountryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ShortName { get; set; } = string.Empty;

    public IList<Hotel> Hotels { get; set; } = [];
}
