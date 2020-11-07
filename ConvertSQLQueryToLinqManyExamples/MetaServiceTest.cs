using ConvertSQLQueryToLinqManyExamples.Entities;
using ConvertSQLQueryToLinqManyExamples.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConvertSQLQueryToLinqManyExamples
{
    public class MetaServiceTest
    {
        private readonly MetaService _metaArtigoService;
        public MetaServiceTest()
        {
            _metaArtigoService = new MetaService();
        }
        public IEnumerable<TestResult> RunTests()
        {
            var parametros = _metaArtigoService.ObterParametrosCalculoAmostraDistribuicao();

            var results = new List<TestResult>();

            var result1 = TestQuantidadeArtigos();
            results.Add( result1 );

            var result2 = TestTotaisFamilias( parametros, result1.ResultObject );
            results.Add( result2 );

            var result3 = TestArtigosOrdenados( result1.ResultObject, result2.ResultObject );
            results.Add( result3 );

            var result4 = TestCalcularPercentualQuantidadeAcumulada( result3.ResultObject );
            results.Add( result4 );

            var result5 = TestCalcularAmostrasEtapa2( parametros, result4.ResultObject );
            results.Add( result5 );

            var result6 = TestCalcularAmostrasEtapa2PorFamilia( parametros, result2.ResultObject, result5.ResultObject );
            results.Add( result6 );

            var result7 = TestCalcularTotaisGruposPorFamilia( result6.ResultObject );
            results.Add( result7 );

            var result8 = TestTotaisFamilia2( result6.ResultObject, result7.ResultObject );
            results.Add( result8 );

            var result9 = TestAtribuirNumeroGrupo( result8.ResultObject );
            results.Add( result9 );

            var result10 = TestCalcularAcertos( parametros, result9.ResultObject );
            results.Add( result10 );

            var result11 = TestCalcularAmostrasEtapa5( parametros, result5.ResultObject, result9.ResultObject, result10.ResultObject );
            results.Add( result11 );

            var result12 = TestCalcularAmostrasEtapa6( parametros, result5.ResultObject, result9.ResultObject, result11.ResultObject );
            results.Add( result12 );

            var result13 = TestCalcularAmostrasFinaisEtapa6( parametros, result12.ResultObject );
            results.Add( result13 );

            var result14 = TestCalcularArtigosFinais( parametros, result5.ResultObject, result9.ResultObject, result13.ResultObject );
            results.Add( result14 );

            return results;

        }

        private TestResult<MetaArtigo[]> TestQuantidadeArtigos()
        {
            var inicio = DateTime.Now;
            var artigos =  _metaArtigoService.ObterArtigos();
            var expected = 6841;
            var result = artigos.Length.Equals( expected );

            return new TestResult<MetaArtigo[]>( "TestQuantidadeArtigos"
                , result
                , artigos.Length.ToString()
                , expected.ToString()
                , artigos
                , DateTime.Now.Subtract( inicio ) );

        }

        private TestResult<MetaTotaisFamilia[]> TestTotaisFamilias( MetaParametrosCalculoAmostraDistribuicao parametros, MetaArtigo[] artigos )
        {
            var inicio = DateTime.Now;

            var familiasCount = 25;
            var totalArtigos = 6841;
            var familiasComRepresentacao = 17;
            var percentualTotalQuantidadesVendidas = 100.00;
            var totalAmostrasEtapa1 = 284;

            var totaisFamilia = _metaArtigoService.ObterTotaisFamilia( parametros, artigos );

            var _familiasCount = totaisFamilia.Count();
            var _totalArtigos = totaisFamilia.Sum( x => x.QuantidadeArtigos );
            var _familiasComRepresentacao = totaisFamilia.Where( x => x.PossuiRepresentacao ).Count();
            var _percentualTotalQuantidadesVendidas = totaisFamilia.Sum( x => x.PercentualVendidasFamilia );
            var _totalAmostrasEtapa1 = totaisFamilia.Sum( x => x.AmostrasEtapa1 );

            var result = _familiasCount.Equals( familiasCount )
             && _totalArtigos.Equals( totalArtigos )
             && _familiasComRepresentacao.Equals( familiasComRepresentacao )
             && _percentualTotalQuantidadesVendidas.Equals( percentualTotalQuantidadesVendidas )
             && _totalAmostrasEtapa1.Equals( totalAmostrasEtapa1 );

            var output = "familiasCount: {0}, totalArtigos: {1}, familiasComRepresentacao: {2}, percentualTotalQuantidadesVendidas: {3}, totalAmostrasEtapa1: {4}";

            return new TestResult<MetaTotaisFamilia[]>( "TestTotaisFamilias"
                , result
                , string.Format( output
                , _familiasCount
                , _totalArtigos
                , _familiasComRepresentacao
                , _percentualTotalQuantidadesVendidas
                , _totalAmostrasEtapa1
                )
                , string.Format( output
                , familiasCount
                , totalArtigos
                , familiasComRepresentacao
                , percentualTotalQuantidadesVendidas
                , totalAmostrasEtapa1
                )
                , totaisFamilia
                , DateTime.Now.Subtract( inicio ) );

        }

        private TestResult<MetaArtigoOrdenado[]> TestArtigosOrdenados( MetaArtigo[] artigos, MetaTotaisFamilia[] totaisFamilia )
        {
            var inicio = DateTime.Now;

            var totalArtigos = 6841;
            var percentualVendidasArtigo = 2500;
            var totalHomogeneos = 80;

            var artigosOrdenados = _metaArtigoService.ObterArtigosOrdenados( artigos, totaisFamilia );

            var _totalArtigos = artigosOrdenados.Count();
            var _percentualVendidasArtigo = artigosOrdenados.Sum( x => x.PercentualVendidasArtigo );
            var _totalHomogeneos = artigosOrdenados.Count( x => x.Homogeneo );

            var result = _totalArtigos.Equals( totalArtigos )
                && _percentualVendidasArtigo.Equals( percentualVendidasArtigo )
                && _totalHomogeneos.Equals( totalHomogeneos );

            var output = "totalArtigos: {0}, percentualVendidasArtigo: {1}, totalHomogeneos: {2}";

            return new TestResult<MetaArtigoOrdenado[]>( "TestArtigosOrdenados"
                , result
                , string.Format( output, _totalArtigos, _percentualVendidasArtigo, _totalHomogeneos )
                , string.Format( output, totalArtigos, percentualVendidasArtigo, totalHomogeneos )
                , artigosOrdenados
                , DateTime.Now.Subtract( inicio ) );
        }

        private TestResult<MetaArtigoOrdenado[]> TestCalcularPercentualQuantidadeAcumulada( MetaArtigoOrdenado[] artigosOrdenados )
        {
            var inicio = DateTime.Now;

            var totalPercentualQuantidadeAcumulada = 614213.496139174;

            artigosOrdenados = _metaArtigoService.CalcularPercentualQuantidadeAcumulada( artigosOrdenados );

            var _totalPercentualQuantidadeAcumulada = artigosOrdenados.Sum( a => Math.Round( a.PercentualQuantidadeAcumulada, 13 ) );

            var output = "totalPercentualQuantidadeAcumulada: {0}";

            return new TestResult<MetaArtigoOrdenado[]>( "TestCalcularPercentualQuantidadeAcumulada"
               , _totalPercentualQuantidadeAcumulada.Equals( totalPercentualQuantidadeAcumulada )
               , string.Format( output, _totalPercentualQuantidadeAcumulada )
               , string.Format( output, totalPercentualQuantidadeAcumulada )
               , artigosOrdenados
               , DateTime.Now.Subtract( inicio ) );
        }

        private TestResult<MetaArtigoOrdenado[]> TestCalcularAmostrasEtapa2( MetaParametrosCalculoAmostraDistribuicao parametros, MetaArtigoOrdenado[] artigosOrdenados )
        {
            var inicio = DateTime.Now;

            var totalAmostrasEtapa2 = 3076186;

            artigosOrdenados = _metaArtigoService.CalcularAmostrasEtapa2( parametros, artigosOrdenados );

            var _totalAmostrasEtapa2 = artigosOrdenados.Sum( a => a.AmostrasEtapa2 );

            var output = "totalAmostrasEtapa2: {0}";

            return new TestResult<MetaArtigoOrdenado[]>( "TestCalcularAmostrasEtapa2"
               , _totalAmostrasEtapa2.Equals( totalAmostrasEtapa2 )
               , string.Format( output, _totalAmostrasEtapa2 )
               , string.Format( output, totalAmostrasEtapa2 )
               , artigosOrdenados
               , DateTime.Now.Subtract( inicio ) );
        }

        private TestResult<MetaTotaisFamilia[]> TestCalcularAmostrasEtapa2PorFamilia( MetaParametrosCalculoAmostraDistribuicao parametros, MetaTotaisFamilia[] totaisFamilias, MetaArtigoOrdenado[] artigosOrdenados )
        {
            var inicio = DateTime.Now;

            var totalAmostrasFamiliaEtapa2 = 220;

            totaisFamilias = _metaArtigoService.CalcularAmostrasEtapa2PorFamilia( parametros, totaisFamilias, artigosOrdenados );

            var _totalAmostrasFamiliaEtapa2 = totaisFamilias.Sum( a => a.AmostrasEtapa2 );

            var output = "totalAmostrasFamiliaEtapa2: {0}";

            return new TestResult<MetaTotaisFamilia[]>( "TestCalcularAmostrasEtapa2PorFamilia"
               , _totalAmostrasFamiliaEtapa2.Equals( totalAmostrasFamiliaEtapa2 )
               , string.Format( output, _totalAmostrasFamiliaEtapa2 )
               , string.Format( output, totalAmostrasFamiliaEtapa2 )
               , totaisFamilias
               , DateTime.Now.Subtract( inicio ) );
        }

        private TestResult<MetaTotaisGrupoFamilia[]> TestCalcularTotaisGruposPorFamilia( MetaTotaisFamilia[] totaisFamilias )
        {
            var inicio = DateTime.Now;

            var percentualTotalVendidasGrupoFamilia = 2363.658435321177277600;
            var totalAmostrasEtapa2GrupoFamilia = 5145;

            var totaisGrupos = _metaArtigoService.CalcularTotaisGruposPorFamilia( totaisFamilias );

            var _percentualTotalVendidasGrupoFamilia = totaisGrupos.Sum( a => a.PercentualVendidasGrupo );
            var _totalAmostrasEtapa2GrupoFamilia = totaisGrupos.Sum( a => a.AmostrasEtapa2Grupo );

            var result = _percentualTotalVendidasGrupoFamilia.Equals( percentualTotalVendidasGrupoFamilia )
                && _totalAmostrasEtapa2GrupoFamilia.Equals( totalAmostrasEtapa2GrupoFamilia );

            var output = "percentualTotalVendidasGrupoFamilia: {0}, totalAmostrasEtapa2GrupoFamilia: {1}";

            return new TestResult<MetaTotaisGrupoFamilia[]>( "TestCalcularTotaisGruposPorFamilia"
               , result
               , string.Format( output, _percentualTotalVendidasGrupoFamilia, _totalAmostrasEtapa2GrupoFamilia )
               , string.Format( output, percentualTotalVendidasGrupoFamilia, totalAmostrasEtapa2GrupoFamilia )
               , totaisGrupos
               , DateTime.Now.Subtract( inicio ) );
        }

        private TestResult<MetaTotaisFamilia2[]> TestTotaisFamilia2( MetaTotaisFamilia[] totaisFamilias, MetaTotaisGrupoFamilia[] totaisGruposFamilia )
        {
            var inicio = DateTime.Now;

            var familiasCount = 25;
            var familiasComRepresentatividade = 17;

            var totaisFamilia2 = _metaArtigoService.ObterTotaisFamilia2( totaisFamilias, totaisGruposFamilia );

            var _familiasCount = totaisFamilia2.Count();
            var _familiasComRepresentatividade = totaisFamilia2.Count( x => x.PossuiRepresentatividade );

            var result = _familiasCount.Equals( familiasCount )
                && _familiasComRepresentatividade.Equals( familiasComRepresentatividade );

            var output = "familiasCount: {0}, familiasComRepresentatividade: {1}";

            return new TestResult<MetaTotaisFamilia2[]>( "TestTotaisFamilia2"
               , result
               , string.Format( output, _familiasCount, _familiasComRepresentatividade )
               , string.Format( output, familiasCount, familiasComRepresentatividade )
               , totaisFamilia2
               , DateTime.Now.Subtract( inicio ) );
        }

        private TestResult<MetaTotaisFamilia2[]> TestAtribuirNumeroGrupo( MetaTotaisFamilia2[] totaisFamilias2 )
        {
            var inicio = DateTime.Now;

            var familiaIdXGrupoNumero = new (int, int)[] { (1, 1), (2, 1), (3, 2), (4, 3), (6, 4), (7, 5), (9, 6), (10, 6), (11, 7), (12, 8), (13, 8), (14, 9), (15, 10), (16, 10), (17, 10), (19, 11), (20, 11), (21, 12), (22, 12), (23, 13), (24, 12), (25, 14), (26, 15), (27, 16), (28, 17) };

            var totaisFamilia2 = _metaArtigoService.AtribuirNumeroGrupo( totaisFamilias2 );

            var _familiaIdXGrupoNumero = totaisFamilia2.Select( x => (x.FamiliaId, x.NumeroGrupo) ).ToArray();

            var result = familiaIdXGrupoNumero.All( a => _familiaIdXGrupoNumero.Any( b => b.FamiliaId == a.Item1 && b.NumeroGrupo == a.Item2 ) );

            return new TestResult<MetaTotaisFamilia2[]>( "TestAtribuirNumeroGrupo"
               , result
               , string.Join( ",", _familiaIdXGrupoNumero )
               , string.Join( ",", familiaIdXGrupoNumero )
               , totaisFamilia2
               , DateTime.Now.Subtract( inicio ) );
        }

        private TestResult<MetaTotaisPorNumeroGrupo[]> TestCalcularAcertos( MetaParametrosCalculoAmostraDistribuicao parametros, MetaTotaisFamilia2[] totaisFamilias2 )
        {
            var inicio = DateTime.Now;

            var totalEtapa5 = 227;

            var totaisPorNumeroGrupos = _metaArtigoService.CalcularAcertos( parametros, totaisFamilias2 );

            var _totalEtapa5 = totaisPorNumeroGrupos.Sum( x => x.AmostrasEtapa5 );

            var result = _totalEtapa5.Equals( totalEtapa5 );

            return new TestResult<MetaTotaisPorNumeroGrupo[]>( "TestCalcularAcertos"
               , result
               , _totalEtapa5.ToString()
               , totalEtapa5.ToString()
               , totaisPorNumeroGrupos
               , DateTime.Now.Subtract( inicio ) );
        }

        private TestResult<MetaTotaisPorNumeroGrupo[]> TestCalcularAmostrasEtapa5( MetaParametrosCalculoAmostraDistribuicao parametros, MetaArtigoOrdenado[] artigosOrdenados, MetaTotaisFamilia2[] totaisFamilia2, MetaTotaisPorNumeroGrupo[] totaisPorNumeroGrupo )
        {
            var inicio = DateTime.Now;

            var totalEtapa5 = 269;

            var amostrasEtapa5 = _metaArtigoService.CalcularAmostrasEtapa5( parametros, artigosOrdenados, totaisFamilia2, totaisPorNumeroGrupo );

            var _totalEtapa5 = amostrasEtapa5.Sum( x => x.AmostrasEtapa5 );

            var result = _totalEtapa5.Equals( totalEtapa5 );

            return new TestResult<MetaTotaisPorNumeroGrupo[]>( "TestCalcularAmostrasEtapa5"
               , result
               , _totalEtapa5.ToString()
               , totalEtapa5.ToString()
               , amostrasEtapa5
               , DateTime.Now.Subtract( inicio ) );
        }

        private TestResult<MetaTotaisPorNumeroGrupo[]> TestCalcularAmostrasEtapa6( MetaParametrosCalculoAmostraDistribuicao parametros, MetaArtigoOrdenado[] artigosOrdenados, MetaTotaisFamilia2[] totaisFamilia2, MetaTotaisPorNumeroGrupo[] totaisPorNumeroGrupo )
        {
            var inicio = DateTime.Now;

            var totalEtapa6 = 299;

            var amostrasEtapa6 = _metaArtigoService.CalcularAmostrasEtapa6( parametros, artigosOrdenados, totaisFamilia2, totaisPorNumeroGrupo );

            var _totalEtapa6 = amostrasEtapa6.Sum( x => x.AmostrasEtapa6 );

            var result = _totalEtapa6.Equals( totalEtapa6 );

            return new TestResult<MetaTotaisPorNumeroGrupo[]>( "TestCalcularAmostrasEtapa6"
               , result
               , _totalEtapa6.ToString()
               , totalEtapa6.ToString()
               , amostrasEtapa6
               , DateTime.Now.Subtract( inicio ) );
        }

        private TestResult<MetaAmostraFinal[]> TestCalcularAmostrasFinaisEtapa6( MetaParametrosCalculoAmostraDistribuicao parametros, MetaTotaisPorNumeroGrupo[] totaisPorNumeroGrupo )
        {
            var inicio = DateTime.Now;

            var totalEtapa6 = 300;

            var amostrasEtapa6 = _metaArtigoService.CalcularAmostrasFinaisEtapa6( parametros, totaisPorNumeroGrupo );

            var _totalEtapa6 = amostrasEtapa6.Sum( x => x.AmostrasFinal );

            var result = _totalEtapa6.Equals( totalEtapa6 );

            return new TestResult<MetaAmostraFinal[]>( "CalcularAmostrasFinaisEtapa6"
               , result
               , _totalEtapa6.ToString()
               , totalEtapa6.ToString()
               , amostrasEtapa6
               , DateTime.Now.Subtract( inicio ) );
        }

        private TestResult<MetaArtigoFinal[]> TestCalcularArtigosFinais( MetaParametrosCalculoAmostraDistribuicao parametros, MetaArtigoOrdenado[] artigosOrdenados, MetaTotaisFamilia2[] totaisFamilia2, MetaAmostraFinal[] amostrasFinais )
        {
            var inicio = DateTime.Now;

            var amostrasPorGrupo = new (int Grupo, int Amostras)[] { (1, 10), (2, 6), (3, 11), (4, 10), (5, 27), (6, 11), (7, 4), (8, 10), (9, 10), (10, 28), (11, 12), (12, 41), (13, 10), (14, 32), (15, 34), (16, 12), (17, 32) };

            var artigosFinais = _metaArtigoService.CalcularArtigosFinais( parametros, artigosOrdenados, totaisFamilia2, amostrasFinais );

            var _amostrasPorGrupo = artigosFinais.GroupBy( x => (x.NumeroGrupo, x.AmostrasFinal) )
            .OrderBy( x => x.Key.NumeroGrupo )
            .Select( x => (Grupo: x.Key.NumeroGrupo, Amostras: x.Key.AmostrasFinal) ).ToArray();

            var result = _amostrasPorGrupo.All( a => amostrasPorGrupo.Any( b => b.Grupo == a.Grupo && b.Amostras == a.Amostras ) );

            return new TestResult<MetaArtigoFinal[]>( "TestCalcularArtigosFinais"
               , result
               , string.Join( ",", _amostrasPorGrupo )
               , string.Join( ",", amostrasPorGrupo )
               , artigosFinais
               , DateTime.Now.Subtract( inicio ) );
        }
    }
}
