namespace ConvertSQLQueryToLinqManyExamples.Models

{
    public class MetaPesoArtigoModel
    {
        public int Id { get; set; }
        public int Ano { get; set; }
        public string Referencia { get; set; }
        public int MaterialEmbalagemId { get; set; }
        public decimal Primario { get; set; }
        public decimal Secondario { get; set; }
        public decimal Terciario { get; set; }
        public decimal MultiEmbalagens { get; set; }
        public string Material { get; set; }
        public int Ordem { get; set; }
    }
}
