namespace ItensPresentes.Model
{
    public class ItemCasaModel
    {
        public long Id { get; set; }
        public string NomeItem { get; set; }
        public bool Ativo { get; set; }
        public int Ambiente { get; set; }
        public List<string> NomePessoas { get; set; }
        public long Quantidade { get; set; }
    }
}
