namespace BaylongoApi.Templates.Contracts
{
    public interface ITemplateService
    {
        Task<string> RenderTemplateAsync(string templateName, IDictionary<string, string> parameters);
    }
}
