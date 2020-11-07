using System.Collections.Generic;

namespace ConvertSQLQueryToLinqManyExamples.Models
{
    public struct MetaTotaisGrupoFamilia
    {
        public int FamiliaId;
        public string Token;
        public int AmostrasEtapa1;
        public decimal AmostrasEtapa2;
        public int NivelGrupoFamilia;
        public int OrdemGrupoFamilia;
        public decimal PercentualVendidasFamilia;
        public decimal PercentualVendidasGrupo;
        public decimal AmostrasEtapa2Grupo;

        public MetaTotaisGrupoFamilia( int familiaId, string token, int amostrasEtapa1, decimal amostrasEtapa2, int nivelGrupoFamilia, int ordemGrupoFamilia, decimal percentualVendidasFamilia, decimal percentualVendidasGrupo, decimal amostrasEtapa2Grupo )
        {
            FamiliaId = familiaId;
            Token = token;
            AmostrasEtapa1 = amostrasEtapa1;
            AmostrasEtapa2 = amostrasEtapa2;
            NivelGrupoFamilia = nivelGrupoFamilia;
            OrdemGrupoFamilia = ordemGrupoFamilia;
            PercentualVendidasFamilia = percentualVendidasFamilia;
            PercentualVendidasGrupo = percentualVendidasGrupo;
            AmostrasEtapa2Grupo = amostrasEtapa2Grupo;
        }

        public override bool Equals( object obj )
        {
            return obj is MetaTotaisGrupoFamilia other &&
                     FamiliaId == other.FamiliaId &&
                     Token == other.Token &&
                     AmostrasEtapa1 == other.AmostrasEtapa1 &&
                     AmostrasEtapa2 == other.AmostrasEtapa2 &&
                     NivelGrupoFamilia == other.NivelGrupoFamilia &&
                     OrdemGrupoFamilia == other.OrdemGrupoFamilia &&
                     PercentualVendidasFamilia == other.PercentualVendidasFamilia &&
                     PercentualVendidasGrupo == other.PercentualVendidasGrupo &&
                     AmostrasEtapa2Grupo == other.AmostrasEtapa2Grupo;
        }

        public override int GetHashCode()
        {
            int hashCode = 1924056354;
            hashCode = hashCode * -1521134295 + FamiliaId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode( Token );
            hashCode = hashCode * -1521134295 + AmostrasEtapa1.GetHashCode();
            hashCode = hashCode * -1521134295 + AmostrasEtapa2.GetHashCode();
            hashCode = hashCode * -1521134295 + NivelGrupoFamilia.GetHashCode();
            hashCode = hashCode * -1521134295 + OrdemGrupoFamilia.GetHashCode();
            hashCode = hashCode * -1521134295 + PercentualVendidasFamilia.GetHashCode();
            hashCode = hashCode * -1521134295 + PercentualVendidasGrupo.GetHashCode();
            hashCode = hashCode * -1521134295 + AmostrasEtapa2Grupo.GetHashCode();
            return hashCode;
        }

        public void Deconstruct( out int familiaId, out string token, out int amostrasEtapa1, out decimal amostrasEtapa2, out int nivelGrupoFamilia, out int ordemGrupoFamilia, out decimal percentualVendidasFamilia, out decimal percentualVendidasGrupo, out decimal amostrasEtapa2Grupo )
        {
            familiaId = FamiliaId;
            token = Token;
            amostrasEtapa1 = AmostrasEtapa1;
            amostrasEtapa2 = AmostrasEtapa2;
            nivelGrupoFamilia = NivelGrupoFamilia;
            ordemGrupoFamilia = OrdemGrupoFamilia;
            percentualVendidasFamilia = PercentualVendidasFamilia;
            percentualVendidasGrupo = PercentualVendidasGrupo;
            amostrasEtapa2Grupo = AmostrasEtapa2Grupo;
        }

        public static implicit operator (int FamiliaId, string Token, int AmostrasEtapa1, decimal AmostrasEtapa2, int NivelGrupoFamilia, int OrdemGrupoFamilia, decimal PercentualVendidasFamilia, decimal PercentualVendidasGrupo, decimal AmostrasEtapa2Grupo)( MetaTotaisGrupoFamilia value )
        {
            return (value.FamiliaId, value.Token, value.AmostrasEtapa1, value.AmostrasEtapa2, value.NivelGrupoFamilia, value.OrdemGrupoFamilia, value.PercentualVendidasFamilia, value.PercentualVendidasGrupo, value.AmostrasEtapa2Grupo);
        }

        public static implicit operator MetaTotaisGrupoFamilia( (int FamiliaId, string Token, int AmostrasEtapa1, decimal AmostrasEtapa2, int NivelGrupoFamilia, int OrdemGrupoFamilia, decimal PercentualVendidasFamilia, decimal PercentualVendidasGrupo, decimal AmostrasEtapa2Grupo) value )
        {
            return new MetaTotaisGrupoFamilia( value.FamiliaId, value.Token, value.AmostrasEtapa1, value.AmostrasEtapa2, value.NivelGrupoFamilia, value.OrdemGrupoFamilia, value.PercentualVendidasFamilia, value.PercentualVendidasGrupo, value.AmostrasEtapa2Grupo );
        }
    }
}
