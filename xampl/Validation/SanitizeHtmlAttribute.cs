#nullable disable
using System.ComponentModel.DataAnnotations;
using Ganss.Xss;

namespace xampl.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SanitizeHtmlAttribute : ValidationAttribute
    {
        public string AllowedTags { get; set; }
        public string AllowedAttributes { get; set; }

        public SanitizeHtmlAttribute()
        {
            ErrorMessage = "The content contains disallowed HTML elements or attributes.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var content = value as string;
            if (string.IsNullOrWhiteSpace(content)) return ValidationResult.Success;

            var sanitizer = new HtmlSanitizer();
            if (!string.IsNullOrEmpty(AllowedTags))
            {
                sanitizer.AllowedTags.Clear();
                foreach (var tag in AllowedTags.Split([','], StringSplitOptions.RemoveEmptyEntries))
                {
                    sanitizer.AllowedTags.Add(tag.Trim());
                }
            }
            if (!string.IsNullOrEmpty(AllowedAttributes))
            {
                sanitizer.AllowedAttributes.Clear();
                foreach (var attr in AllowedAttributes.Split([ ',' ], StringSplitOptions.RemoveEmptyEntries))
                {
                    sanitizer.AllowedAttributes.Add(attr.Trim());
                }
            }

            bool removedDisallowedContent = false;
            sanitizer.RemovingTag += (s, e) => { removedDisallowedContent = true; };
            sanitizer.RemovingAttribute += (s, e) => { removedDisallowedContent = true; };
            sanitizer.RemovingCssClass += (s, e) => { removedDisallowedContent = true; };
            sanitizer.RemovingStyle += (s, e) => { removedDisallowedContent = true; };
            sanitizer.Sanitize(content);

            if (removedDisallowedContent)
            {
                return new ValidationResult(ErrorMessage, [ validationContext.MemberName ]);
            }

            return ValidationResult.Success;
        }
    }
}
