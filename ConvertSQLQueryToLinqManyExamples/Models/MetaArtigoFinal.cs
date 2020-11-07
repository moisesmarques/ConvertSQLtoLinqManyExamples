namespace ConvertSQLQueryToLinqManyExamples.Models

{
    public class MetaArtigoFinal
    {
        public int Id { get; set; }
        public int? FamiliaId { get; set; }
        public string Referencia { get; set; }
        public long QuantidadeVendidas { get; set; }
        public int NumeroGrupo { get; set; }
        public int AmostrasFinal { get; set; }
        public int QuantidadeArtigosGrupo { get; set; }
        public long QuantidadeVendidasGrupo { get; set; }
        public decimal PercentualVendidasNoGrupo { get; set; }
        public decimal PercentualAcumuladoNoGrupo { get; set; }
        public int Rank { get; set; }
        public bool Amostra { get; set; }
        public bool PossuiSubGrupo { get; set; }
        public bool FamiliaUnica { get; set; }
        public decimal PercentualFamiliaNoGrupo { get; set; }

        public MetaArtigoFinal(int id, int? familiaId, string referencia, long quantidadeVendidas, int numeroGrupo, int amostrasFinal, int quantidadeArtigosGrupo, long quantidadeVendidasGrupo, decimal percentualVendidasNoGrupo, decimal percentualAcumuladoNoGrupo, int rank, bool amostra, bool possuiSubGrupo, bool familiaUnica, decimal percentualFamiliaNoGrupo )
        {
            Id = id;
            FamiliaId = familiaId;
            Referencia = referencia;
            QuantidadeVendidas = quantidadeVendidas;
            NumeroGrupo = numeroGrupo;
            AmostrasFinal = amostrasFinal;
            QuantidadeArtigosGrupo = quantidadeArtigosGrupo;
            QuantidadeVendidasGrupo = quantidadeVendidasGrupo;
            PercentualVendidasNoGrupo = percentualVendidasNoGrupo;
            PercentualAcumuladoNoGrupo = percentualAcumuladoNoGrupo;
            Rank = rank;
            Amostra = amostra;
            PossuiSubGrupo = possuiSubGrupo;
            FamiliaUnica = familiaUnica;
            PercentualFamiliaNoGrupo = percentualFamiliaNoGrupo;
        }
    }
}
