using Core.Dtos;

namespace Core.Interfaces;

public interface IMaterialService
{
    Task<IEnumerable<MaterialDto>> GetAllMaterialsAsync();
    Task<MaterialDto> GetMaterialByIdAsync(int id);
    Task<string> AddMaterialAsync(string material);
    Task<string> UpdateMaterialAsync(MaterialDto materialDto);
    Task<string> DeleteMaterialAsync(int id);
}
