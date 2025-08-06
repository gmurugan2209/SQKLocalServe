using SQKLocalServe.Common;
using SQKLocalServe.Contract.DTOs;

namespace SQKLocalServe.Business.Services.Interfaces;

public interface ILocationService
{
    Task<ApiResponse<List<LocationDto>>> GetAllLocationsAsync();
    Task<ApiResponse<LocationDto>> CreateLocationAsync(CreateLocationDto dto);
    Task<ApiResponse<LocationDto>> UpdateLocationAsync(int id, UpdateLocationDto dto);
    Task<ApiResponse<bool>> DeactivateLocationAsync(int id);
}