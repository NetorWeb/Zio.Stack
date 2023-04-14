using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Api.Entities
{
    public class Entity
    {
        public Entity()
        {
            CreatedOn = DateTime.Now;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
