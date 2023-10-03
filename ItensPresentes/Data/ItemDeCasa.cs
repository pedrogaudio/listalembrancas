using Postgrest.Attributes;
using Postgrest.Models;

namespace ItensPresentes.Data
{
    [Table("itenscasa")]
    public class ItemDeCasa : BaseModel
    {
        [PrimaryKey("id")]
        public long Id { get; set; }
        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [Column("quantidade")]
        public long Quantidade { get; set; }
        [Column("nome_itens")]
        public string NomeItem { get; set; }
        [Column("nome_pessoas")]
        public List<string> NomePessoas { get; set; }
        [Column("ativo")]
        public bool Ativo { get; set; }
        [Column("ambiente")]
        public int Ambiente { get; set; }
        [Column("link")]
        public string Link { get; set; }

    }
}
