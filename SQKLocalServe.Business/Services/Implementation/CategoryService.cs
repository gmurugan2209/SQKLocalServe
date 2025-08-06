using Microsoft.EntityFrameworkCore;
using SQKLocalServe.DataAccess;
using SQKLocalServe.Business.Services.Interfaces;
using SQKLocalServe.Common;
using Microsoft.Extensions.Logging;

namespace SQKLocalServe.Business.Services.Implementation;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<List<CategoryDto>>> GetAllAsync()
    {
        try
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();

            var categoryDtos = categories.Select(MapToDto).ToList();
            return ApiResponse<List<CategoryDto>>.Success(categoryDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return ApiResponse<List<CategoryDto>>.Failed("100","An error occurred while retrieving categories");
        }
    }

    public async Task<ApiResponse<CategoryDto>> GetByIdAsync(int id)
    {
        try
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (category == null)
                return ApiResponse<CategoryDto>.NotFound($"Category with ID {id} not found");

            return ApiResponse<CategoryDto>.Success(MapToDto(category));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category {CategoryId}", id);
            return ApiResponse<CategoryDto>.Failed("100","An error occurred while retrieving the category");
        }
    }

    public async Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryDto dto)
    {
        try
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return ApiResponse<CategoryDto>.Success(MapToDto(category));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return ApiResponse<CategoryDto>.Failed("100","An error occurred while creating the category");
        }
    }

    public async Task<ApiResponse<CategoryDto>> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        try
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (category == null)
                return ApiResponse<CategoryDto>.NotFound($"Category with ID {id} not found");

            category.Name = dto.Name;
            category.Description = dto.Description;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse<CategoryDto>.Success(MapToDto(category));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);
            return ApiResponse<CategoryDto>.Failed("100","An error occurred while updating the category");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        try
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (category == null)
                return ApiResponse<bool>.NotFound($"Category with ID {id} not found");

            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", id);
            return ApiResponse<bool>.Failed("100","An error occurred while deleting the category");
        }
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
          //  CreatedAt = category.CreatedAt,
           // UpdatedAt = category.UpdatedAt
        };
    }
}