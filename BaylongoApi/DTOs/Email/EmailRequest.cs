namespace BaylongoApi.DTOs.Email
{
    public record EmailRequest(
        string ToEmail,
        string ToName,
        string Subject,
        string HtmlContent);
}
