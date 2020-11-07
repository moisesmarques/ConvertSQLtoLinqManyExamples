using System.Collections.Generic;

namespace ConvertSQLQueryToLinqManyExamples.Entities
{
    public class MetaFamilia
    {
        public int Id { get; set; }
        public int VariabilidadeId { get; set; }
        public MetaVariabilidadeFamilia Variabilidade { get; set; }
        public string Token { get; set; }
        public string Descricao { get; set; }
        public int Ordem { get; set; }
        public decimal Limite { get; set; }
        public virtual ICollection<MetaGrupoFamilia> Grupos { get; set; }

    }
}
