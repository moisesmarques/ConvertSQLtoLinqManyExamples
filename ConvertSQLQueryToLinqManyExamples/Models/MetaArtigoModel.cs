namespace ConvertSQLQueryToLinqManyExamples.Models
{
    public class MetaArtigoModel
    {
        public int Id { get; set; }
        public int Ano { get; set; }
        public string Referencia { get; set; }
        public string Descricao { get; set; }
        public bool Embalado { get; set; }
        public int Quantidade { get; set; }
        public bool Amostra { get; set; }
        public int Grupo { get; set; }
        public int FamiliaId { get; set; }
        public string Familia { get; internal set; }
    }
}
