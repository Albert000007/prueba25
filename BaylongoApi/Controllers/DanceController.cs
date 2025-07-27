using BaylongoApi.DTOs.Dance;
using BaylongoApi.Services;
using BaylongoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaylongoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanceController(ICatalogService catalogService, IDanceService danceService, IDanceCategoryService danceCategoryService, ILogger<DanceController> logger) : ControllerBase
    {
        [HttpGet("types-with-levels")]
        public async Task<ActionResult<IEnumerable<DanceTypeWithLevelsDto>>> GetDanceTypesWithLevels()
        {
            return Ok(await danceService.GetDanceTypesWithLevels());
        }

        [HttpGet("{danceTypeId}/levels")]
        public async Task<ActionResult<IEnumerable<DanceLevelDto>>> GetLevelsForDanceType(int danceTypeId)
        {
            return Ok(await danceService.GetLevelsForDanceType(danceTypeId));
        }

        [HttpGet("levels")]
        public async Task<ActionResult<IEnumerable<DanceLevelDto>>> GetAllDanceLevels()
        {
            try
            {
                var result = await danceService.GetAllDanceLevels();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all dance levels");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("types/{danceTypeId}")]
        public async Task<ActionResult<DanceTypeDto>> GetDanceTypeDetails(int danceTypeId)
        {
            try
            {
                var result = await danceService.GetDanceTypeDetails(danceTypeId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error getting details for dance type {danceTypeId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("categories/{categoryId}/types")]
        public async Task<ActionResult<IEnumerable<DanceTypeDto>>> GetDanceTypesByCategory(int categoryId)
        {
            try
            {
                var result = await danceService.GetDanceTypesByCategory(categoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error getting dance types for category {categoryId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DanceCategoryDto>>> GetCategoryAll(
        [FromQuery] bool includeDanceTypes = false)
        {
            try
            {
                var categories = await danceCategoryService.GetAllCategoriesAsync(includeDanceTypes);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all dance categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DanceCategoryDto>> GetCategoryById(
            int id,
            [FromQuery] bool includeDanceTypes = false)
        {
            try
            {
                var category = await danceCategoryService.GetCategoryByIdAsync(id, includeDanceTypes);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error getting dance category with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<DanceCategoryDto>> CreateCategory(
            [FromBody] CreateDanceCategoryDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdCategory = await danceCategoryService.CreateCategoryAsync(createDto);
                return CreatedAtAction(
                    nameof(GetCategoryById),
                    new { id = createdCategory.CategoryId },
                    createdCategory);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating dance category");
                return StatusCode(500, "Internal server error");
            }
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<DanceCategoryDto>> UpdateCategory(
            int id,
            [FromBody] UpdateDanceCategoryDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedCategory = await danceCategoryService.UpdateCategoryAsync(id, updateDto);
                return Ok(updatedCategory);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error updating dance category with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await danceCategoryService.DeleteCategoryAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error deleting dance category with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
