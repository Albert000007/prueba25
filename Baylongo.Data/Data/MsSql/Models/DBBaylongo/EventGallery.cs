using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class EventGallery
{
    public int GalleryId { get; set; }

    public int EventId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? Description { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;
}
