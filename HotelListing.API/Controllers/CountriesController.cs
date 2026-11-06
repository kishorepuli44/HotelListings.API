using HotelListing.API.Data;
using HotelListing.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly ICountryRepository _repository;

    public CountriesController(ICountryRepository repository)
    {
        _repository = repository;
    }

    // GET: api/countries
    [HttpGet]
    [AllowAnonymous]  // ✅ Explicitly allow public read access
    public ActionResult<IEnumerable<Country>> Get()
    {
        return Ok(_repository.GetAll());
    }

    // GET: api/countries/5
    [HttpGet("{id}")]
    [AllowAnonymous]  // ✅ Explicitly allow public read access
    public ActionResult<Country> Get(int id)
    {
        var country = _repository.GetById(id);
        if (country == null)
        {
            return NotFound($"Country with Id {id} not found.");
        }
        return Ok(country);
    }

    // POST: api/countries
    [HttpPost]
    [Authorize(Roles = "Admin")]  // ✅ Require Admin role
    public ActionResult<Country> Post([FromBody] Country newCountry)
    {
        if (newCountry == null)
        {
            return BadRequest("Country data is required.");
        }

        if (_repository.GetById(newCountry.Id) != null)
        {
            return BadRequest("A country with the same Id already exists.");
        }

        if (string.IsNullOrWhiteSpace(newCountry.Name) || string.IsNullOrWhiteSpace(newCountry.Code))
        {
            return BadRequest("Country Name and Code are required.");
        }

        _repository.Add(newCountry);
        return CreatedAtAction(nameof(Get), new { id = newCountry.Id }, newCountry);
    }

    // PUT: api/countries/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]  // ✅ Require Admin role
    public ActionResult<Country> Put(int id, [FromBody] Country updatedCountry)
    {
        if (updatedCountry == null)
        {
            return BadRequest("Country data is required.");
        }

        var existingCountry = _repository.GetById(id);
        if (existingCountry == null)
        {
            return NotFound($"Country with Id {id} not found.");
        }

        if (string.IsNullOrWhiteSpace(updatedCountry.Name) || string.IsNullOrWhiteSpace(updatedCountry.Code))
        {
            return BadRequest("Country Name and Code are required.");
        }

        _repository.Update(id, updatedCountry);
        return Ok(_repository.GetById(id));
    }

    // DELETE: api/countries/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]  // ✅ Require Admin role
    public ActionResult Delete(int id)
    {
        var country = _repository.GetById(id);
        if (country == null)
        {
            return NotFound($"Country with Id {id} not found.");
        }

        _repository.Delete(id);
        return NoContent();
    }
}
