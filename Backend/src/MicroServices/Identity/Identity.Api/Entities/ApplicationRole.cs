using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Api.Entities
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole()
        {
            CreatedOn = DateTime.Now;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public override int Id { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}