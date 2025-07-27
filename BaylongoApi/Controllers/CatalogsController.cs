using BaylongoApi.DTOs.Catalogs;
using BaylongoApi.Services;
using BaylongoApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaylongoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogsController(ICatalogService catalogService) : ControllerBase
    {
        [HttpGet("all-catalog")]
        public async Task<ActionResult<IEnumerable<CatalogDto>>> GetAllCatalogs()
        {
            var catalogs = await catalogService.GetAllCatalogs();
            return Ok(catalogs);
        }
    }
}
