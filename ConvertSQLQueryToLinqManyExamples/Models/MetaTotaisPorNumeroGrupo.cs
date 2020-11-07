namespace ConvertSQLQueryToLinqManyExamples.Models
{
    public struct MetaTotaisPorNumeroGrupo
    {
        public int NumeroGrupo;
        public int QuantidadeArtigosGrupo;
        public int QuantidadeVendidasGrupo;
        public decimal PercentualVendidasGrupo;
        public decimal PercentualVendidasPerdida;
        public decimal PercentualVendidasNaoUsadas;
        public decimal PercentualVendidasAcertada;
        public decimal PercentualAmostrasRestantes;
        public int AmostrasEtapa1;
        public int AmostrasEtapa2;
        public int AmostrasEtapa5;
        public int AmostrasEtapa6;

        public MetaTotaisPorNumeroGrupo( int numeroGrupo, int quantidadeArtigosGrupo, int quantidadeVendidasGrupo, decimal percentualVendidasGrupo, decimal percentualVendidasPerdida, decimal percentualVendidasNaoUsadas, decimal percentualVendidasAcertada, decimal percentualAmostrasRestantes, int amostrasEtapa1, int amostrasEtapa2, int amostrasEtapa5, int amostrasEtapa6)
        {
            NumeroGrupo = numeroGrupo;
            QuantidadeArtigosGrupo = quantidadeArtigosGrupo;
            QuantidadeVendidasGrupo = quantidadeVendidasGrupo;
            PercentualVendidasGrupo = percentualVendidasGrupo;
            PercentualVendidasPerdida = percentualVendidasPerdida;
            PercentualVendidasNaoUsadas = percentualVendidasNaoUsadas;
            PercentualVendidasAcertada = percentualVendidasAcertada;
            PercentualAmostrasRestantes = percentualAmostrasRestantes;
            AmostrasEtapa1 = amostrasEtapa1;
            AmostrasEtapa2 = amostrasEtapa2;
            AmostrasEtapa5 = amostrasEtapa5;
            AmostrasEtapa6 = amostrasEtapa6;
        }

        public override bool Equals( object obj )
        {
            return obj is MetaTotaisPorNumeroGrupo other &&
                     NumeroGrupo == other.NumeroGrupo &&
                     QuantidadeArtigosGrupo == other.QuantidadeArtigosGrupo &&
                     QuantidadeVendidasGrupo == other.QuantidadeVendidasGrupo &&
                     PercentualVendidasGrupo == other.PercentualVendidasGrupo &&
                     PercentualVendidasPerdida == other.PercentualVendidasPerdida &&
                     PercentualVendidasNaoUsadas == other.PercentualVendidasNaoUsadas &&
                     PercentualVendidasAcertada == other.PercentualVendidasAcertada &&
                     PercentualAmostrasRestantes == other.PercentualAmostrasRestantes &&
                     AmostrasEtapa1 == other.AmostrasEtapa1 &&
                     AmostrasEtapa2 == other.AmostrasEtapa2 &&
                     AmostrasEtapa5 == other.AmostrasEtapa5 &&
                     AmostrasEtapa6 == other.AmostrasEtapa6;
        }

        public override int GetHashCode()
        {
            int hashCode = -33107300;
            hashCode = hashCode * -1521134295 + NumeroGrupo.GetHashCode();
            hashCode = hashCode * -1521134295 + QuantidadeArtigosGrupo.GetHashCode();
            hashCode = hashCode * -1521134295 + QuantidadeVendidasGrupo.GetHashCode();
            hashCode = hashCode * -1521134295 + PercentualVendidasGrupo.GetHashCode();
            hashCode = hashCode * -1521134295 + PercentualVendidasPerdida.GetHashCode();
            hashCode = hashCode * -1521134295 + PercentualVendidasNaoUsadas.GetHashCode();
            hashCode = hashCode * -1521134295 + PercentualVendidasAcertada.GetHashCode();
            hashCode = hashCode * -1521134295 + PercentualAmostrasRestantes.GetHashCode();
            hashCode = hashCode * -1521134295 + AmostrasEtapa1.GetHashCode();
            hashCode = hashCode * -1521134295 + AmostrasEtapa2.GetHashCode();
            hashCode = hashCode * -1521134295 + AmostrasEtapa5.GetHashCode();
            hashCode = hashCode * -1521134295 + AmostrasEtapa6.GetHashCode();
            return hashCode;
        }

        public void Deconstruct( out int numeroGrupo, out int quantidadeArtigosGrupo, out int quantidadeVendidasGrupo, out decimal percentualVendidasGrupo, out decimal percentualVendidasPerdida, out decimal percentualVendidasNaoUsadas, out decimal percentualVendidasAcertada, out decimal percentualAmostrasRestantes, out int amostrasEtapa1, out int amostrasEtapa2, out int amostrasEtapa5, out int amostrasEtapa6 )
        {
            numeroGrupo = NumeroGrupo;
            quantidadeArtigosGrupo = QuantidadeArtigosGrupo;
            quantidadeVendidasGrupo = QuantidadeVendidasGrupo;
            percentualVendidasGrupo = PercentualVendidasGrupo;
            percentualVendidasPerdida = PercentualVendidasPerdida;
            percentualVendidasNaoUsadas = PercentualVendidasNaoUsadas;
            percentualVendidasAcertada = PercentualVendidasAcertada;
            percentualAmostrasRestantes = PercentualAmostrasRestantes;
            amostrasEtapa1 = AmostrasEtapa1;
            amostrasEtapa2 = AmostrasEtapa2;
            amostrasEtapa5 = AmostrasEtapa5;
            amostrasEtapa6 = AmostrasEtapa6;
        }

        public static implicit operator (int NumeroGrupo, int QuantidadeArtigosGrupo, int QuantidadeVendidasGrupo, decimal PercentualVendidasGrupo, decimal PercentualVendidasPerdida, decimal PercentualVendidasNaoUsadas, decimal PercentualVendidasAcertada, decimal PercentualAmostrasRestantes, int AmostrasEtapa1, int AmostrasEtapa2, int AmostrasEtapa5, int AmostrasEtapa6)( MetaTotaisPorNumeroGrupo value )
        {
            return (value.NumeroGrupo, value.QuantidadeArtigosGrupo, value.QuantidadeVendidasGrupo, value.PercentualVendidasGrupo, value.PercentualVendidasPerdida, value.PercentualVendidasNaoUsadas, value.PercentualVendidasAcertada, value.PercentualAmostrasRestantes, value.AmostrasEtapa1, value.AmostrasEtapa2, value.AmostrasEtapa5, value.AmostrasEtapa6);
        }

        public static implicit operator MetaTotaisPorNumeroGrupo( (int NumeroGrupo, int QuantidadeArtigosGrupo, int QuantidadeVendidasGrupo, decimal PercentualVendidasGrupo, decimal PercentualVendidasPerdida, decimal PercentualVendidasNaoUsadas, decimal PercentualVendidasAcertada, decimal PercentualAmostrasRestantes, int AmostrasEtapa1, int AmostrasEtapa2, int AmostrasEtapa5, int AmostrasEtapa6) value )
        {
            return new MetaTotaisPorNumeroGrupo( value.NumeroGrupo, value.QuantidadeArtigosGrupo, value.QuantidadeVendidasGrupo, value.PercentualVendidasGrupo, value.PercentualVendidasPerdida, value.PercentualVendidasNaoUsadas, value.PercentualVendidasAcertada, value.PercentualAmostrasRestantes, value.AmostrasEtapa1, value.AmostrasEtapa2, value.AmostrasEtapa5, value.AmostrasEtapa6 );
        }
    }
}
