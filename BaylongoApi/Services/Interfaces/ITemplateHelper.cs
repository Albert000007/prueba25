namespace BaylongoApi.Services.Interfaces
{
    public interface ITemplateHelper
    {
        /// <summary>
        /// Obtiene el contenido de una plantilla HTML como string
        /// </summary>
        /// <param name="templateName">Nombre del archivo de plantilla (con o sin extensión .html)</param>
        /// <returns>Contenido HTML de la plantilla</returns>
        Task<string> GetTemplateHtmlAsStringAsync(string templateName);

        /// <summary>
        /// Obtiene una plantilla HTML y reemplaza los placeholders con los valores del modelo
        /// </summary>
        /// <typeparam name="T">Tipo del modelo</typeparam>
        /// <param name="templateName">Nombre del archivo de plantilla</param>
        /// <param name="model">Objeto con los datos para la plantilla</param>
        /// <returns>Plantilla HTML procesada</returns>
        Task<string> GetTemplateHtmlAsStringAsync<T>(string templateName, T model) where T : class;

        /// <summary>
        /// Reemplaza placeholders en un template usando un diccionario
        /// </summary>
        /// <param name="templateContent">Contenido del template</param>
        /// <param name="placeholders">Diccionario de placeholders y valores</param>
        /// <returns>Template con placeholders reemplazados</returns>
        string ReplacePlaceholders(string templateContent, Dictionary<string, string> placeholders);

        /// <summary>
        /// Reemplaza placeholders en un template usando un objeto anónimo
        /// </summary>
        /// <param name="templateContent">Contenido del template</param>
        /// <param name="placeholders">Objeto con propiedades que coinciden con los placeholders</param>
        /// <returns>Template con placeholders reemplazados</returns>
        string ReplacePlaceholders(string templateContent, object placeholders);
    }
}
