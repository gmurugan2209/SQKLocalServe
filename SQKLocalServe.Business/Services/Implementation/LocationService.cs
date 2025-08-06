using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQKLocalServe.Business.Services.Interfaces;
using SQKLocalServe.Common;
using SQKLocalServe.Contract.DTOs;
using SQKLocalServe.DataAccess;
using SQKLocalServe.DataAccess.Entities;

namespace SQKLocalServe.Business.Services.Implementation;

public class LocationService : ILocationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LocationService> _logger;

    public LocationService(ApplicationDbContext context, ILogger<LocationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<List<LocationDto>>> GetAllLocationsAsync()
    {
        try
        {
            var locations = await _context.Locations
                .Where(l => l.IsActive)
                .Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    City = l.City,
                    State = l.State,
                    PostalCode = l.PostalCode,
                    IsActive = l.IsActive,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt
                })
                .ToListAsync();

            return ApiResponse<List<LocationDto>>.Success(locations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving locations");
            throw;
        }
    }

    public async Task<ApiResponse<LocationDto>> CreateLocationAsync(CreateLocationDto dto)
    {
        try
        {
            var location = new Location
            {
                Name = dto.Name,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();

            return ApiResponse<LocationDto>.Success(MapToDto(location));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating location");
            throw;
        }
    }

    public async Task<ApiResponse<LocationDto>> UpdateLocationAsync(int id, UpdateLocationDto dto)
    {
        try
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
                return ApiResponse<LocationDto>.NotFound("Location not found");

            location.Name = dto.Name;
            location.City = dto.City;
            location.State = dto.State;
            location.PostalCode = dto.PostalCode;
            location.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse<LocationDto>.Success(MapToDto(location));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating location {LocationId}", id);
            throw;
        }
    }

    public async Task<ApiResponse<bool>> DeactivateLocationAsync(int id)
    {
        try
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
                return ApiResponse<bool>.NotFound("Location not found");

            location.IsActive = false;
            location.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating location {LocationId}", id);
            throw;
        }
    }

    private static LocationDto MapToDto(Location location)
    {
        return new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            City = location.City,
            State = location.State,
            PostalCode = location.PostalCode,
            IsActive = location.IsActive,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };
    }
}