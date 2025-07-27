using BaylongoApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace BaylongoApi.Services
{
    public class TemplateHelper(IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache) : ITemplateHelper
    {
        private const string TEMPLATES_FOLDER = "Templates/EmailTemplates";

        public async Task<string> GetTemplateHtmlAsStringAsync(string templateName)
        {
            if (string.IsNullOrWhiteSpace(templateName))
                throw new ArgumentNullException(nameof(templateName));

            // Asegurar que el nombre del template tenga la extensión .html
            if (!templateName.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                templateName += ".html";

            // Cachear plantillas para mejorar rendimiento
            var cacheKey = $"template_{templateName}";

            if (!memoryCache.TryGetValue(cacheKey, out string templateContent))
            {
                var templatePath = Path.Combine(
                    hostingEnvironment.ContentRootPath,
                    TEMPLATES_FOLDER,
                    templateName);

                if (!File.Exists(templatePath))
                    throw new FileNotFoundException($"Template file not found at path: {templatePath}");

                templateContent = await File.ReadAllTextAsync(templatePath);

                // Cachear la plantilla por 1 hora
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(1));

                memoryCache.Set(cacheKey, templateContent, cacheOptions);
            }

            return templateContent;
        }

        public async Task<string> GetTemplateHtmlAsStringAsync<T>(string templateName, T model) where T : class
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var templateContent = await GetTemplateHtmlAsStringAsync(templateName);
            return ReplacePlaceholders(templateContent, model);
        }

        public string ReplacePlaceholders(string templateContent, Dictionary<string, string> placeholders)
        {
            if (string.IsNullOrWhiteSpace(templateContent))
                throw new ArgumentNullException(nameof(templateContent));

            if (placeholders == null)
                return templateContent;

            foreach (var placeholder in placeholders)
            {
                templateContent = templateContent.Replace(
                    $"{{{{{placeholder.Key}}}}}",
                    placeholder.Value);
            }

            return templateContent;
        }

        public string ReplacePlaceholders(string templateContent, object placeholders)
        {
            if (string.IsNullOrWhiteSpace(templateContent))
                throw new ArgumentNullException(nameof(templateContent));

            if (placeholders == null)
                return templateContent;

            var properties = placeholders.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var placeholder = $"{{{{{prop.Name}}}}}";
                var value = prop.GetValue(placeholders)?.ToString() ?? string.Empty;
                templateContent = templateContent.Replace(placeholder, value);
            }

            return templateContent;
        }
    }
}
