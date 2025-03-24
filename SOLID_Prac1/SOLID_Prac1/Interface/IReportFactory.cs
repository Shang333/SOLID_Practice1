namespace SOLID_Prac1.Interface
{
    public interface IReportFactory
    {
        IReport Create(string type, int maxSize);
    }
}
