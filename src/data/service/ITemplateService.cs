using LicenseGenerator.domain.model;

namespace LicenseGenerator.data.service;

public interface ITemplateService
{
    public void InitService();

    public void Save(Template template);

    public Template? SearchById(int id);

    public List<Template> List();

    public void Update(Template template);

    public void Delete(int id);
}
