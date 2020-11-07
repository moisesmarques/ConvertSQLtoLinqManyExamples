namespace ConvertSQLQueryToLinqManyExamples.Models

{
    public class MetaGrupoArtigoFinal
    {
        public MetaGrupoArtigoFinal(int numero, MetaArtigoFinal[] amostras)
        {
            Numero = numero;
            Amostras = amostras;
        }
        public int Numero { get; set; }
        public MetaArtigoFinal[] Amostras { get; set; }

    }
}
