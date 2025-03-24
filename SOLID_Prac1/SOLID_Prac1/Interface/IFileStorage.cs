namespace SOLID_Prac1.Interface
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(IFormFile file);
    }
}
