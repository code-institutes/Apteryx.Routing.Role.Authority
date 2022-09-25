using System.ComponentModel.DataAnnotations;

namespace Apteryx.WebApi
{
    public class AddTest
    {
        [Required(ErrorMessage = "“{0}”必填")]
        public string? Name { get; set; }
    }
}
