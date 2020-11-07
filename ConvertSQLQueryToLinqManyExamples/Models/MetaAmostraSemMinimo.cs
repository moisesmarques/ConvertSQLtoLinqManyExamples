namespace ConvertSQLQueryToLinqManyExamples.Models
{
    public struct MetaAmostraSemMinimo
    {
        public int NumeroGrupo;
        public int AmostrasEtapa5;
        public int SomaAmostrasEtapa2;

        public MetaAmostraSemMinimo( int numeroGrupo, int amostrasEtapa5, int somaAmostrasEtapa2 )
        {
            NumeroGrupo = numeroGrupo;
            AmostrasEtapa5 = amostrasEtapa5;
            SomaAmostrasEtapa2 = somaAmostrasEtapa2;
        }

        public override bool Equals( object obj )
        {
            return obj is MetaAmostraSemMinimo other &&
                     NumeroGrupo == other.NumeroGrupo &&
                     AmostrasEtapa5 == other.AmostrasEtapa5 &&
                     SomaAmostrasEtapa2 == other.SomaAmostrasEtapa2;
        }

        public override int GetHashCode()
        {
            int hashCode = 2037574199;
            hashCode = hashCode * -1521134295 + NumeroGrupo.GetHashCode();
            hashCode = hashCode * -1521134295 + AmostrasEtapa5.GetHashCode();
            hashCode = hashCode * -1521134295 + SomaAmostrasEtapa2.GetHashCode();
            return hashCode;
        }

        public void Deconstruct( out int numeroGrupo, out int amostrasEtapa5, out int somaAmostrasEtapa2 )
        {
            numeroGrupo = NumeroGrupo;
            amostrasEtapa5 = AmostrasEtapa5;
            somaAmostrasEtapa2 = SomaAmostrasEtapa2;
        }

        public static implicit operator (int NumeroGrupo, int AmostrasEtapa5, int SomaAmostrasEtapa2)( MetaAmostraSemMinimo value )
        {
            return (value.NumeroGrupo, value.AmostrasEtapa5, value.SomaAmostrasEtapa2);
        }

        public static implicit operator MetaAmostraSemMinimo( (int NumeroGrupo, int AmostrasEtapa5, int SomaAmostrasEtapa2) value )
        {
            return new MetaAmostraSemMinimo( value.NumeroGrupo, value.AmostrasEtapa5, value.SomaAmostrasEtapa2 );
        }
    }
}
