using ConvertSQLQueryToLinqManyExamples.Entities;
using System.Collections.Generic;

namespace ConvertSQLQueryToLinqManyExamples.Models
{
    public struct MetaTotaisFamilia2
    {
        public int FamiliaId;
        public string Token;
        public int QuantidadeArtigos;
        public int QuantidadeVendidas;
        public decimal PercentualVendidasFamilia;
        public int AmostrasEtapa1;
        public int AmostrasEtapa2;
        public IEnumerable<MetaGrupoFamilia> Grupos;
        public bool PossuiRepresentatividade;
        public decimal PercentualQuantidadeVendidasGrupo1;
        public decimal PercentualQuantidadeVendidasGrupo2;
        public decimal PercentualQuantidadeVendidasGrupo3;
        public int NumeroGrupo;

        public MetaTotaisFamilia2( int familiaId, string token, int quantidadeArtigos, int quantidadeVendidas, decimal percentualVendidasFamilia, int amostrasEtapa1, int amostrasEtapa2, IEnumerable<MetaGrupoFamilia> grupos, bool possuiRepresentatividade, decimal percentualQuantidadeVendidasGrupo1, decimal percentualQuantidadeVendidasGrupo2, decimal percentualQuantidadeVendidasGrupo3, int numeroGrupo)
        {
            FamiliaId = familiaId;
            Token = token;
            QuantidadeArtigos = quantidadeArtigos;
            QuantidadeVendidas = quantidadeVendidas;
            PercentualVendidasFamilia = percentualVendidasFamilia;
            AmostrasEtapa1 = amostrasEtapa1;
            AmostrasEtapa2 = amostrasEtapa2;
            Grupos = grupos;
            PossuiRepresentatividade = possuiRepresentatividade;
            PercentualQuantidadeVendidasGrupo1 = percentualQuantidadeVendidasGrupo1;
            PercentualQuantidadeVendidasGrupo2 = percentualQuantidadeVendidasGrupo2;
            PercentualQuantidadeVendidasGrupo3 = percentualQuantidadeVendidasGrupo3;
            NumeroGrupo = numeroGrupo;
        }

        public override bool Equals( object obj )
        {
            return obj is MetaTotaisFamilia2 other &&
                     FamiliaId == other.FamiliaId &&
                     Token == other.Token &&
                     QuantidadeArtigos == other.QuantidadeArtigos &&
                     QuantidadeVendidas == other.QuantidadeVendidas &&
                     PercentualVendidasFamilia == other.PercentualVendidasFamilia &&
                     AmostrasEtapa1 == other.AmostrasEtapa1 &&
                     AmostrasEtapa2 == other.AmostrasEtapa2 &&
                    EqualityComparer<IEnumerable<MetaGrupoFamilia>>.Default.Equals( Grupos, other.Grupos ) &&
                     PossuiRepresentatividade == other.PossuiRepresentatividade &&
                     PercentualQuantidadeVendidasGrupo1 == other.PercentualQuantidadeVendidasGrupo1 &&
                     PercentualQuantidadeVendidasGrupo2 == other.PercentualQuantidadeVendidasGrupo2 &&
                     PercentualQuantidadeVendidasGrupo3 == other.PercentualQuantidadeVendidasGrupo3 &&
                     NumeroGrupo == other.NumeroGrupo;
        }

        public override int GetHashCode()
        {
            int hashCode = 1681446885;
            hashCode = hashCode * -1521134295 + FamiliaId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode( Token );
            hashCode = hashCode * -1521134295 + QuantidadeArtigos.GetHashCode();
            hashCode = hashCode * -1521134295 + QuantidadeVendidas.GetHashCode();
            hashCode = hashCode * -1521134295 + PercentualVendidasFamilia.GetHashCode();
            hashCode = hashCode * -1521134295 + AmostrasEtapa1.GetHashCode();
            hashCode = hashCode * -1521134295 + AmostrasEtapa2.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<MetaGrupoFamilia>>.Default.GetHashCode( Grupos );
            hashCode = hashCode * -1521134295 + PossuiRepresentatividade.GetHashCode();
            hashCode = hashCode * -1521134295 + PercentualQuantidadeVendidasGrupo1.GetHashCode();
            hashCode = hashCode * -1521134295 + PercentualQuantidadeVendidasGrupo2.GetHashCode();
            hashCode = hashCode * -1521134295 + PercentualQuantidadeVendidasGrupo3.GetHashCode();
            hashCode = hashCode * -1521134295 + NumeroGrupo.GetHashCode();
            return hashCode;
        }

        public void Deconstruct( out int familiaId, out string token, out int quantidadeArtigos, out int quantidadeVendidas, out decimal percentualVendidasFamilia, out int amostrasEtapa1, out int amostrasEtapa2, out IEnumerable<MetaGrupoFamilia> grupos, out bool possuiRepresentatividade, out decimal percentualQuantidadeVendidasGrupo1, out decimal percentualQuantidadeVendidasGrupo2, out decimal percentualQuantidadeVendidasGrupo3, out int numeroGrupo )
        {
            familiaId = FamiliaId;
            token = Token;
            quantidadeArtigos = QuantidadeArtigos;
            quantidadeVendidas = QuantidadeVendidas;
            percentualVendidasFamilia = PercentualVendidasFamilia;
            amostrasEtapa1 = AmostrasEtapa1;
            amostrasEtapa2 = AmostrasEtapa2;
            grupos = Grupos;
            possuiRepresentatividade = PossuiRepresentatividade;
            percentualQuantidadeVendidasGrupo1 = PercentualQuantidadeVendidasGrupo1;
            percentualQuantidadeVendidasGrupo2 = PercentualQuantidadeVendidasGrupo2;
            percentualQuantidadeVendidasGrupo3 = PercentualQuantidadeVendidasGrupo3;
            numeroGrupo = NumeroGrupo;
        }

        public static implicit operator (int FamiliaId, string Token, int QuantidadeArtigos, int QuantidadeVendidas, decimal PercentualVendidasFamilia, int AmostrasEtapa1, int AmostrasEtapa2, IEnumerable<MetaGrupoFamilia> Grupos, bool PossuiRepresentatividade, decimal PercentualQuantidadeVendidasGrupo1, decimal PercentualQuantidadeVendidasGrupo2, decimal PercentualQuantidadeVendidasGrupo3, int NumeroGrupo)( MetaTotaisFamilia2 value )
        {
            return (value.FamiliaId, value.Token, value.QuantidadeArtigos, value.QuantidadeVendidas, value.PercentualVendidasFamilia, value.AmostrasEtapa1, value.AmostrasEtapa2, value.Grupos, value.PossuiRepresentatividade, value.PercentualQuantidadeVendidasGrupo1, value.PercentualQuantidadeVendidasGrupo2, value.PercentualQuantidadeVendidasGrupo3, value.NumeroGrupo);
        }

        public static implicit operator MetaTotaisFamilia2( (int FamiliaId, string Token, int QuantidadeArtigos, int QuantidadeVendidas, decimal PercentualVendidasFamilia, int AmostrasEtapa1, int AmostrasEtapa2, IEnumerable<MetaGrupoFamilia> Grupos, bool PossuiRepresentatividade, decimal PercentualQuantidadeVendidasGrupo1, decimal PercentualQuantidadeVendidasGrupo2, decimal PercentualQuantidadeVendidasGrupo3, int NumeroGrupo) value )
        {
            return new MetaTotaisFamilia2( value.FamiliaId, value.Token, value.QuantidadeArtigos, value.QuantidadeVendidas, value.PercentualVendidasFamilia, value.AmostrasEtapa1, value.AmostrasEtapa2, value.Grupos, value.PossuiRepresentatividade, value.PercentualQuantidadeVendidasGrupo1, value.PercentualQuantidadeVendidasGrupo2, value.PercentualQuantidadeVendidasGrupo3, value.NumeroGrupo );
        }
    }
}
