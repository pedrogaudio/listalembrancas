using Postgrest.Attributes;
using Postgrest.Models;

namespace ItensPresentes.Data
{
    [Table("listapresenca")]
    public class ListaPresenca : BaseModel
    {
        [PrimaryKey("id")]
        public long Id { get; set; }
        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [Column("nome")]
        public string Nome { get; set; }
        [Column("rg")]
        public string RG { get; set; }
    }
}
