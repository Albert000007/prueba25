using BaylongoApi.Templates.Contracts;

namespace BaylongoApi.Templates
{
    public class TemplateService : ITemplateService
    {

        private readonly IWebHostEnvironment _env;
        private readonly ILogger<TemplateService> _logger;

        public TemplateService(
            IWebHostEnvironment env,
            ILogger<TemplateService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<string> RenderTemplateAsync(string templateName, IDictionary<string, string> parameters)
        {
            var templateContent = await LoadTemplateAsync(templateName);
            return ReplacePlaceholders(templateContent, parameters);
        }

        private async Task<string> LoadTemplateAsync(string templateName)
        {
            try
            {
                var templatePath = Path.Combine(
     _env.ContentRootPath,  // Usa ContentRootPath en lugar de WebRootPath
     "Templates",          // Carpeta Templates
     "EmailTemplates",     // Subcarpeta EmailTemplates
     $"{templateName}.html");

                if (!File.Exists(templatePath))
                {
                    throw new FileNotFoundException($"Template not found: {templatePath}");
                }

                return await File.ReadAllTextAsync(templatePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading template {TemplateName}", templateName);
                throw;
            }
        }

        private static string ReplacePlaceholders(string templateContent, IDictionary<string, string> parameters)
        {
            foreach (var param in parameters)
            {
                templateContent = templateContent.Replace($"{{{{{param.Key}}}}}", param.Value);
            }
            return templateContent;
        }

    }
}
