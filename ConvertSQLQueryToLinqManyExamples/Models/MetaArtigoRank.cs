using ConvertSQLQueryToLinqManyExamples.Entities;

namespace ConvertSQLQueryToLinqManyExamples.Models

{
    public class MetaArtigoRank
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
        public int AmostrasPorGrupo { get; set; }
        public bool PossuiSubGrupo { get; set; }
        public bool FamiliaUnica { get; set; }
        public double PercentualFamiliaNoGrupo { get; set; }
        public decimal PercentualQuantidadeAcumulada { get; internal set; }
        public int Rank { get; set; }
        public int FamiliaOrdem { get; internal set; }
        public string FamiliaDescricao { get; internal set; }
        public int FamiliaVariabilidadeId { get; internal set; }
        public int FamiliaQuantidadeArtigos { get; internal set; }
        public int FamiliaQuantidadeVendidas { get; internal set; }
        public string FamiliaVariabilidadeToken { get; internal set; }
        public int FamiliaAmostrasEtapa1 { get; internal set; }
        public bool FamiliaPossuiRepresentacao { get; internal set; }
        public decimal PercentualQuantidadeVendidas { get; internal set; }

        public MetaArtigoRank(MetaArtigo artigo)
        {
            Id = artigo.Id;
            FamiliaId = artigo.FamiliaId ?? 0;
            Ano = artigo.Ano;
            Referencia = artigo.Referencia;
            Descricao = artigo.Descricao;
            Embalado = artigo.Embalado;
            Quantidade = artigo.Quantidade;
            Amostra = artigo.Amostra;
            Grupo = artigo.Grupo;
            
        }
    }
}
