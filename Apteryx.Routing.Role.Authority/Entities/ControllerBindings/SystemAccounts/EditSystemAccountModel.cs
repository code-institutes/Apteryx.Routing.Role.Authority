using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority
{
    public class EditSystemAccountModel:AddSystemAccountModel
    {
        [Required(ErrorMessage = "Id必填")]
        public string? Id { get; set; }
    }
}
