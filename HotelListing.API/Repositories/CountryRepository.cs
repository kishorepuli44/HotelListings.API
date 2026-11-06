using HotelListing.API.Data;

namespace HotelListing.API.Repositories;

public class CountryRepository : ICountryRepository
{
    private static List<Country> countries = new List<Country>
    {
        new Country { Id = 1, Name = "United States", Code = "US", Region = "North America" },
        new Country { Id = 2, Name = "Canada", Code = "CA", Region = "North America" },
        new Country { Id = 3, Name = "United Kingdom", Code = "UK", Region = "Europe" },
        new Country { Id = 4, Name = "France", Code = "FR", Region = "Europe" },
        new Country { Id = 5, Name = "Japan", Code = "JP", Region = "Asia" }
    };

    public IEnumerable<Country> GetAll()
    {
        return countries;
    }

    public Country? GetById(int id)
    {
        return countries.FirstOrDefault(c => c.Id == id);
    }

    public void Add(Country country)
    {
        countries.Add(country);
    }

    public void Update(int id, Country country)
    {
        var existingCountry = GetById(id);
        if (existingCountry != null)
        {
            existingCountry.Name = country.Name;
            existingCountry.Code = country.Code;
            existingCountry.Region = country.Region;
        }
    }

    public void Delete(int id)
    {
        var country = GetById(id);
        if (country != null)
        {
            countries.Remove(country);
        }
    }
}
