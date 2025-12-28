namespace LicenseGenerator.data.repository;

public interface IRepository<T>
{
    public void Save(T t);

    public T? SearchById(int id);

    public List<T> List();

    public void Update(T t);

    public void Delete(int id);
}
