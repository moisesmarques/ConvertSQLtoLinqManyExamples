namespace ConvertSQLQueryToLinqManyExamples.Models
{
    public class MetaArtigoOrdenado
    {
        public int Id { get; private set; }
        public int? FamiliaId { get; set; }
        public int FamiliaOrdem { get; set; }
        public string FamiliaDescricao { get; set; }
        public int VariabilidadeId { get; set; }
        public string Referencia { get; set; }
        public int Quantidade { get; set; }
        public int QuantidadeArtigos { get; set; }
        public long QuantidadeVendidas { get; set; }
        public decimal PercentualVendidasArtigo { get; set; }
        public decimal PercentualQuantidadeAcumulada { get; set; }
        public int OrdemPercentualQuantidadeAcumulada { get; set; }
        public bool Homogeneo { get; set; }
        public int AmostrasEtapa1 { get; set; }
        public bool PossuiRepresentacao { get; set; }
        public int AmostrasEtapa2 { get; set; }

        public MetaArtigoOrdenado( 
            int id
            , int? familiaId
            , int familiaOrdem
            , string familiaDescricao
            , int variabilidadeId
            , string referencia
            , int quantidade
            , int quantidadeArtigos
            , long quantidadeVendidas
            , decimal percentualVendidasArtigo
            , decimal percentualQuantidadeAcumulada
            , int ordemPercentualQuantidadeAcumulada
            , bool homogeneo
            , int amostrasEtapa1
            , bool possuiRepresentacao
            , int amostrasEtapa2 )
        {
            Id = id;
            FamiliaId = familiaId;
            FamiliaOrdem = familiaOrdem;
            FamiliaDescricao = familiaDescricao;
            VariabilidadeId = variabilidadeId;
            Referencia = referencia;
            Quantidade = quantidade;
            QuantidadeArtigos = quantidadeArtigos;
            QuantidadeVendidas = quantidadeVendidas;
            PercentualVendidasArtigo = percentualVendidasArtigo;
            PercentualQuantidadeAcumulada = percentualQuantidadeAcumulada;
            OrdemPercentualQuantidadeAcumulada = ordemPercentualQuantidadeAcumulada;
            Homogeneo = homogeneo;
            AmostrasEtapa1 = amostrasEtapa1;
            PossuiRepresentacao = possuiRepresentacao;
            AmostrasEtapa2 = amostrasEtapa2;
        }

    }
}
