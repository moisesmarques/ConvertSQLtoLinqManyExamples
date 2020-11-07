namespace ConvertSQLQueryToLinqManyExamples.Models
{
    public struct MetaAmostraFinal
    {
        public int NumeroGrupo;
        public int AmostrasFinal;

        public MetaAmostraFinal( int numeroGrupo, int amostrasFinal )
        {
            NumeroGrupo = numeroGrupo;
            AmostrasFinal = amostrasFinal;
        }

        public override bool Equals( object obj )
        {
            return obj is MetaAmostraFinal other &&
                     NumeroGrupo == other.NumeroGrupo &&
                     AmostrasFinal == other.AmostrasFinal;
        }

        public override int GetHashCode()
        {
            int hashCode = -1475546221;
            hashCode = hashCode * -1521134295 + NumeroGrupo.GetHashCode();
            hashCode = hashCode * -1521134295 + AmostrasFinal.GetHashCode();
            return hashCode;
        }

        public void Deconstruct( out int numeroGrupo, out int amostrasFinal )
        {
            numeroGrupo = NumeroGrupo;
            amostrasFinal = AmostrasFinal;
        }

        public static implicit operator (int NumeroGrupo, int AmostrasFinal)( MetaAmostraFinal value )
        {
            return (value.NumeroGrupo, value.AmostrasFinal);
        }

        public static implicit operator MetaAmostraFinal( (int NumeroGrupo, int AmostrasFinal) value )
        {
            return new MetaAmostraFinal( value.NumeroGrupo, value.AmostrasFinal );
        }
    }
}
