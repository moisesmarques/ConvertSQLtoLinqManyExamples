using ConvertSQLQueryToLinqManyExamples.Entities;
using System.Collections.Generic;

namespace ConvertSQLQueryToLinqManyExamples.Models
{
    public class MetaTotaisFamilia
    {
        public int FamiliaId { get; set; }
        public int Ordem { get; set; }
        public string Token { get; set; }
        public string Descricao { get; set; }
        public int VariabilidadeId { get; set; }
        public int QuantidadeArtigos { get; set; }
        public int QuantidadeVendidas { get; set; }
        public decimal PercentualVendidasFamilia { get; }
        public int AmostrasEtapa1 { get; set; }
        public int AmostrasEtapa2 { get; set; }
        public bool PossuiRepresentacao { get; set; }
        public IEnumerable<MetaGrupoFamilia> Grupos { get; set; }
        public MetaFamilia Familia { get; set; }
        public decimal PercentualQuantidadeVendidasGrupo1 { get; set; }
        public decimal PercentualQuantidadeVendidasGrupo2 { get; set; }
        public decimal PercentualQuantidadeVendidasGrupo3 { get; set; }
        public bool PossuiRepresentatividade { get; set; }
        public int NumeroGrupo { get; internal set; }
        public MetaTotaisFamilia( 
            int id
            , int ordem
            , string token
            , string descricao
            , int variabilidadeId
            , int quantidadeArtigos
            , int quantidadeVendidas
            , decimal percentualVendidasFamilia
            , int amostrasEtapa1
            , int amostrasEtapa2
            , bool possuiRepresentacao
            , IEnumerable<MetaGrupoFamilia> grupos
            , MetaFamilia familia)
        {
            FamiliaId = id;
            Ordem = ordem;
            Token = token;
            Descricao = descricao;
            VariabilidadeId = variabilidadeId;
            QuantidadeArtigos = quantidadeArtigos;
            QuantidadeVendidas = quantidadeVendidas;
            PercentualVendidasFamilia = percentualVendidasFamilia;
            AmostrasEtapa1 = amostrasEtapa1;
            AmostrasEtapa2 = amostrasEtapa2;
            PossuiRepresentacao = possuiRepresentacao;
            Grupos = grupos;
            Familia = familia;
        }
    }
}

