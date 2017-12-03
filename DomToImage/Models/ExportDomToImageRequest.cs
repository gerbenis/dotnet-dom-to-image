using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DomToImage.Models
{
    public class ExportDomToImageRequest
    {
        public IEnumerable<string> CssUrls { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Filename { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }
    }
}