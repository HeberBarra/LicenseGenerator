using LicenseGenerator.data.service;
using LicenseGenerator.domain.model;

namespace LicenseGenerator.data.repository;

public class TemplateRepository(ITemplateService templateService) : IRepository<Template>
{
    public void Save(Template template)
    {
        templateService.Save(template);
    }

    public Template? SearchById(int id)
    {
        return templateService.SearchById(id);
    }

    public List<Template> List()
    {
        return templateService.List();
    }

    public void Update(Template template)
    {
        templateService.Update(template);
    }

    public void Delete(int id)
    {
        templateService.Delete(id);
    }
}
