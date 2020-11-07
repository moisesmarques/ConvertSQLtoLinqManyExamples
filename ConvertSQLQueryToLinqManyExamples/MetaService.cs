using ConvertSQLQueryToLinqManyExamples.Entities;
using ConvertSQLQueryToLinqManyExamples.Enums;
using ConvertSQLQueryToLinqManyExamples.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ConvertSQLQueryToLinqManyExamples
{
    public class MetaService
    {
        public MetaParametrosCalculoAmostraDistribuicao ObterParametrosCalculoAmostraDistribuicao() => new MetaParametrosCalculoAmostraDistribuicao();

        public MetaArtigoFinal[] ListarAmostrasDitribuida()
        {
            var parametros = ObterParametrosCalculoAmostraDistribuicao();

            MetaArtigo[] artigos = ObterArtigos();

            MetaTotaisFamilia[] totaisFamilia = ObterTotaisFamilia( parametros, artigos );

            MetaArtigoOrdenado[] artigosOrdenados = ObterArtigosOrdenados( artigos, totaisFamilia );

            artigosOrdenados = CalcularPercentualQuantidadeAcumulada( artigosOrdenados );

            artigosOrdenados = CalcularAmostrasEtapa2( parametros, artigosOrdenados );

            totaisFamilia = CalcularAmostrasEtapa2PorFamilia( parametros, totaisFamilia, artigosOrdenados );

            MetaTotaisGrupoFamilia[] totaisGruposFamilia = CalcularTotaisGruposPorFamilia( totaisFamilia );

            MetaTotaisFamilia2[] totaisFamilia2 = ObterTotaisFamilia2( totaisFamilia, totaisGruposFamilia );

            totaisFamilia2 = AtribuirNumeroGrupo( totaisFamilia2 );

            MetaTotaisPorNumeroGrupo[] totaisPorNumeroGrupo = CalcularAcertos( parametros, totaisFamilia2 );

            totaisPorNumeroGrupo = CalcularAmostrasEtapa5( parametros, artigosOrdenados, totaisFamilia2, totaisPorNumeroGrupo );

            MetaAmostraFinal[] amostrasFinais = CalcularAmostrasFinaisEtapa5( totaisPorNumeroGrupo );

            if ( ObterTotalAmostrasPorDistribuir( parametros, totaisPorNumeroGrupo ) > 0 )
            {
                totaisPorNumeroGrupo = CalcularAmostrasEtapa6( parametros, artigosOrdenados, totaisFamilia2, totaisPorNumeroGrupo );
                amostrasFinais = CalcularAmostrasFinaisEtapa6( parametros, totaisPorNumeroGrupo );
            }

            MetaArtigoFinal[] artigosFinais = CalcularArtigosFinais( parametros, artigosOrdenados, totaisFamilia2, amostrasFinais );

            ////return the  number of samples per group
            //var amostrasPorGrupo = artigosFinais.Where( a => a.Amostra )
            //                                    .GroupBy( a => a.NumeroGrupo )
            //                                    .OrderBy( g => g.Key )
            //                                    .Select( g => new MetaGrupoArtigoFinal( g.Key, g.ToArray() )
            //    );

            //return amostrasPorGrupo;

            return artigosFinais;
        }

        public MetaArtigoFinal[] CalcularArtigosFinais( MetaParametrosCalculoAmostraDistribuicao parametros, MetaArtigoOrdenado[] artigosOrdenados, MetaTotaisFamilia2[] totaisFamilia2, MetaAmostraFinal[] amostrasFinais )
        {
            var quantidadeTotalVendidas = artigosOrdenados.Sum( a => a.Quantidade );
            //with Group_Totals as
            // (select
            //     FAM.GroupNumber,
            //    count( ORD.ReferenceArtigo ) as NumArtigosPorGrupo,
            //    sum( ORD.QtVendidasArtigo ) as QtVendidasPorGrupo,
            //    ( sum( ORD.QtVendidasArtigo ) / @QtTotalVendidas ) * 100 as PercQtVendidasPorGrupo
            //from  #MET_ORDERED_ARTICLES as ORD
            //inner join #MET_FAMILY_TOTALS as FAM on FAM.IDFamily = ORD.IDFamily
            //group by FAM.GroupNumber)

            var totaisGrupos = artigosOrdenados.Join( totaisFamilia2
                , outer => outer.FamiliaId
                , inner => inner.FamiliaId
                , ( outer, inner ) => (inner.NumeroGrupo, outer.Referencia, outer.QuantidadeVendidas) )
                .GroupBy( g => g.NumeroGrupo )
                .Select( g => (
                   NumeroGrupo: g.Key
                 , QuantidadeArtigos: g.Count()
                 , QuantidadeVendidas: g.Sum( ss => ss.QuantidadeVendidas )
                 , PercentualVendidas: ( g.Sum( ss => Convert.ToDecimal( ss.QuantidadeVendidas ) ) / quantidadeTotalVendidas ) * 100) )
                .ToArray();

            //    begin
            //    --#MET_ARTICLES_FINAL
            //    select
            //        ORD.IDFamily,
            //        ORD.ReferenceArtigo,
            //        ORD.QtVendidasArtigo,
            //        GT.NumArtigosPorGrupo,
            //        GT.QtVendidasPorGrupo,        
            //        ( convert( float, ORD.QtVendidasArtigo ) / convert( float, GT.QtVendidasPorGrupo ) ) * 100 as PercQtVendidasArtigoPorGrupo,
            //        case
            //            when RANK() OVER
            //                ( partition by FAM.GroupNumber order by ORD.QtVendidasArtigo desc, ORD.ReferenceArtigo asc ) = 1
            //                then( convert( float, ORD.QtVendidasArtigo ) / convert( float, GT.QtVendidasPorGrupo ) ) * 100
            //                else 0
            //        end as PercQtAcumuladaPorGrupo,
            //        FAM.GroupNumber,
            //        SAMP.NumAmostrasFinal,
            //        0 as IsSample,
            //        RANK() OVER
            //                ( partition by FAM.GroupNumber order by ORD.QtVendidasArtigo desc, ORD.ReferenceArtigo asc ) as [ Rank ],
            //        0 as HasSubGrupo,
            //        0 as OnlyOneFamily,
            //        0 as FamilyPercInGroup
            //    from #MET_ORDERED_ARTICLES as ORD
            //    inner join #MET_FAMILY_TOTALS as FAM on FAM.IDFamily = ORD.IDFamily
            //    inner join #FinalSamples as SAMP on SAMP.GroupNumber = FAM.GroupNumber
            //    inner join Group_Totals as GT on GT.GroupNumber = FAM.GroupNumber
            //    order by FAM.GroupNumber asc, ORD.QtVendidasArtigo desc;
            var artigosFinais = artigosOrdenados.Join( totaisFamilia2
                , artigo => artigo.FamiliaId
                , total => total.FamiliaId
                , ( artigo, total ) => (artigo.Id, artigo.FamiliaId, artigo.Referencia, artigo.QuantidadeVendidas, total.NumeroGrupo) )
            .Join( amostrasFinais
            , artigo => artigo.NumeroGrupo
            , amostra => amostra.NumeroGrupo
            , ( artigo, amostra ) => (artigo.Id, artigo.FamiliaId, artigo.Referencia, artigo.QuantidadeVendidas, artigo.NumeroGrupo, amostra.AmostrasFinal) )
            .Join( totaisGrupos
            , artigo => artigo.NumeroGrupo
            , totalGrupo => totalGrupo.NumeroGrupo
            , ( artigo, totalGrupo ) => new MetaArtigoFinal( artigo.Id
            , artigo.FamiliaId
            , artigo.Referencia
            , artigo.QuantidadeVendidas
            , artigo.NumeroGrupo
            , artigo.AmostrasFinal
            , totalGrupo.QuantidadeArtigos
            , totalGrupo.QuantidadeVendidas
            , ( Convert.ToDecimal( artigo.QuantidadeVendidas ) / Convert.ToDecimal( totalGrupo.QuantidadeVendidas ) ) * 100
            , 0.00m
            , 0
            , false
            , false
            , false
            , 0.00m ) )
            .ToArray();

            artigosFinais = artigosFinais.GroupBy( a => a.NumeroGrupo ).SelectMany( grupo => grupo.OrderByDescending( a => a.QuantidadeVendidas )
                  .ThenBy( a => a.Referencia )
                  .Select( ( a, index ) =>
                  {
                      a.Rank = index + 1;
                      return a;
                  } ) ).ToArray();

            //artigosFinais = artigosFinais.Select( artigo =>
            //{
            //    artigo.PercentualAcumuladoNoGrupo = artigo.Rank == 1 ? ( artigo.QuantidadeVendidas / artigo.QuantidadeVendidasGrupo ) * 100 : 0.00m;
            //    return artigo;

            //} ).ToArray();

            //    declare @NumberOfGroups int = ( select max( GroupNumber ) from #MET_ARTICLES_FINAL)
            //    declare @iGroup int = 1
            //    declare @MaxRank int = 1
            //    declare @iRank int = 1
            //    declare @PercQtAcumuladaPorGrupo float = 0
            //    declare @SampleBiggerThen15 bit = 0

            //   while ( @iGroup <= @NumberOfGroups )

            //        begin
            //            select
            //              @SampleBiggerThen15 = case when NumAmostras <= @FinalStepSampleLimit then 0 else 1 end
            //            from #MET_ARTICLES_FINAL where GroupNumber = @iGroup
            //            group by NumAmostras
            //   if (@SampleBiggerThen15 = 1 )
            //    insert into #FINAL_AUX
            //    select
            //          FINAL.GroupNumber,
            //          FINAL.ReferenceArtigo,
            //          FINAL.[ Rank ],
            //          sum( FINAL_AUX.PercQtVendidasArtigoPorGrupo ) as PercQtAcumuladaPorGrupo
            //    from #MET_ARTICLES_FINAL as FINAL
            //    inner join #MET_ARTICLES_FINAL as FINAL_AUX on FINAL_AUX.GroupNumber = @iGroup 
            //    where FINAL.GroupNumber = @iGroup
            //      and FINAL_AUX.[Rank] <= FINAL.[ Rank ]
            //    group by FINAL.GroupNumber, FINAL.[ Rank ], FINAL.ReferenceArtigo
            //      set @iGroup = @iGroup + 1
            //  end
            var artigosFinaisAux = artigosFinais.Where( a => a.AmostrasFinal >= parametros.NumberOfSamplesPerGroup )
                .GroupBy( a => (a.NumeroGrupo, a.Referencia, a.Rank) )
                .Select( grupo =>
                {
                    return (grupo.Key.NumeroGrupo
                    , grupo.Key.Referencia
                    , grupo.Key.Rank
                    , PercentualQuantidadeAcumuladaNoGrupo: artigosFinais.Where( a => a.NumeroGrupo == grupo.Key.NumeroGrupo
                    && a.Rank <= grupo.Key.Rank )
                    .Sum( a => a.PercentualVendidasNoGrupo ));
                } )
                .ToArray();

            //    update FINAL
            //    set FINAL.PercQtAcumuladaPorGrupo = FINAL_AUX.PercQtAcumulada
            //    from #MET_ARTICLES_FINAL as FINAL 
            //    inner join #FINAL_AUX as FINAL_AUX on FINAL_AUX.GroupNumber = FINAL.GroupNumber 
            //                                    and FINAL_AUX.[ Rank ] = FINAL.[ Rank ]
            artigosFinais = artigosFinais.Select( artigo =>
            {
                var artigoFinalAux = artigosFinaisAux.FirstOrDefault( a => a.NumeroGrupo == artigo.NumeroGrupo && a.Rank == artigo.Rank );
                artigo.PercentualAcumuladoNoGrupo = !artigosFinaisAux.Equals( default ) ? artigoFinalAux.PercentualQuantidadeAcumuladaNoGrupo : 0.00m;

                return artigo;
            } ).ToArray();

            //    --Iterate the groups
            //    declare @iAux int = 1
            //    declare @AmostrasPorGrupo int = 1
            //    set @iRank = 1
            //    declare @IsBellowLimit bit = 0
            //    declare @IsPrimeiroSubgrupo bit = 0
            //    declare @40PercAmostras int = 0
            //    declare @60PercAmostras int = 0
            //    declare @OnlyOnefamily bit = 0
            //    declare @Is60Perc int = 0

            var grupos = artigosFinais.Select( a => a.NumeroGrupo ).Distinct().ToArray();


            //    while ( @iAux <= @NumberOfGroups ) --Iterar por GroupNumber
            foreach ( var grupo in grupos )
            {
                var artigosQuery = artigosFinais.Where( a => a.NumeroGrupo == grupo ).ToArray();

                //    begin
                //        set @AmostrasPorGrupo = ( select TOP 1 NumAmostras from #MET_ARTICLES_FINAL where GroupNumber = @iAux)
                //        set @OnlyOnefamily = ( select case when count( distinct IDFamily ) > 1 then 0 else 1 end from #MET_ARTICLES_FINAL where GroupNumber = @iAux)
                var amostrasPorGrupo = artigosQuery.First().AmostrasFinal;
                var familiaUnica = !( artigosQuery.Select( a => a.FamiliaId ).Distinct().Count() > 1 );

                //  select
                //   @MaxRank = max([ Rank ]),
                var maxRank = artigosQuery.GroupBy( a => a.AmostrasFinal ).Last().Max( a => a.Rank );
                //   @IsBellowLimit = case when NumAmostras <= @FinalStepSampleLimit then 1 else 0 end,
                var abaixoLimite = artigosQuery.GroupBy( a => a.AmostrasFinal ).Last().Key <= parametros.NumberOfSamplesPerGroup;
                //   @40PercAmostras = convert( int, round( NumAmostras * ( @PercentageOfArticleDivisionPerSubGroup / 100 ), 0 ) ),
                var percentualAmostras40 = Convert.ToInt32( Math.Round( artigosQuery.GroupBy( a => a.AmostrasFinal ).Last().Key * ( parametros.PercentageOfArticleDivisionPerSubGroup / 100 ) ) );
                //   @60PercAmostras = @AmostrasPorGrupo - @40PercAmostras
                var percentualAmostras60 = amostrasPorGrupo - percentualAmostras40;
                //        from #MET_ARTICLES_FINAL where GroupNumber = @iAux    
                //          group by NumAmostras

                //        if ( @IsBellowLimit = 0 )
                //         update FINAL
                //            set HasSubGrupo = 1
                //            from #MET_ARTICLES_FINAL as FINAL
                //          where FINAL.GroupNumber = @iAux
                if ( abaixoLimite )
                {
                    artigosQuery = artigosQuery.Select( artigo =>
                    {
                        artigo.PossuiSubGrupo = true;
                        return artigo;
                    } ).ToArray();
                }

                //        if ( @OnlyOnefamily = 1 )
                //         update FINAL
                //            set OnlyOneFamily = 1
                //            from #MET_ARTICLES_FINAL as FINAL
                //          where FINAL.GroupNumber = @iAux
                if ( familiaUnica )
                {
                    artigosQuery = artigosQuery.Select( artigo =>
                    {
                        artigo.FamiliaUnica = true;
                        return artigo;
                    } ).ToArray();
                }

                //      set @Is60Perc = ( select min([ RANK ])
                //                          from #MET_ARTICLES_FINAL as F
                //                         where GroupNumber = @iAux
                //                           and PercQtAcumuladaPorGrupo >= @PercentageOfSampleAllocationPerSubGroup)
                var e60Percento = artigosQuery.Where( a => a.PercentualAcumuladoNoGrupo >= parametros.PercentageOfSampleAllocationPerSubGroup ).Min( a => (int?)a.Rank ) ?? 0;


                //  --Se o numero de amostras menor do que 15, entao distribuir amostras de acordo com a sua quantidade
                //  --atÃ© acabarem as amostras

                //        while ( @iRank <= @MaxRank and @AmostrasPorGrupo<> 0)
                var indiceRank = 1;

                while ( indiceRank <= maxRank && amostrasPorGrupo != 0 )
                {
                    //          begin
                    //            set @IsPrimeiroSubgrupo = ( select case when[ RANK ] <= @Is60Perc then 1 else 0 end
                    //                                          from #MET_ARTICLES_FINAL 
                    //		        where GroupNumber = @iAux and[ RANK ] = @iRank)
                    var primeiroSubGrupo = artigosQuery.Where( a => a.Rank == indiceRank ).Last().Rank <= e60Percento;

                    //   --Se o numero de amostras menor do que 15, entao distribuir amostras de acordo com a sua quantidade

                    //            if ( @IsBellowLimit = 1 and @AmostrasPorGrupo > 0)
                    if ( abaixoLimite && amostrasPorGrupo > 0 )
                    {
                        //              begin
                        //                --PRint 'Primeiro caso'

                        //                update FINAL
                        //                set IsSample = 1,
                        //                    HasSubGrupo = 0
                        //                from #MET_ARTICLES_FINAL as FINAL
                        //                where FINAL.GroupNumber = @iAux
                        //                and[ RANK ] = @iRank
                        artigosFinais = artigosFinais.Select( artigo =>
                        {
                            if ( artigo.Rank == indiceRank )
                            {
                                artigo.Amostra = true;
                                artigo.PossuiSubGrupo = false;
                            }
                            return artigo;
                        } ).ToArray();

                        //                set @AmostrasPorGrupo = @AmostrasPorGrupo - 1
                        amostrasPorGrupo -= 1;
                        //            end
                        //            -- Se o numero de amostras for igual ou maior do que 15
                        //            else if ( @IsBellowLimit = 0 and @IsPrimeiroSubgrupo = 1)
                    }
                    else if ( !abaixoLimite && primeiroSubGrupo )
                    {
                        //              begin

                        //                if (@40PercAmostras > 0)
                        if ( percentualAmostras40 > 0 )
                        {
                            //                  begin
                            //                    --PRint 'Segundo caso'
                            //                    update FINAL
                            //                    set IsSample = 1,
                            //                        HasSubGrupo = 1
                            //                    from #MET_ARTICLES_FINAL as FINAL
                            //                    where FINAL.GroupNumber = @iAux
                            //                    and[ RANK ] = @iRank
                            artigosFinais = artigosFinais.Select( artigo =>
                            {
                                if ( artigo.Rank == indiceRank )
                                {
                                    artigo.Amostra = true;
                                    artigo.PossuiSubGrupo = true;
                                }
                                return artigo;
                            } ).ToArray();
                            //                    set @40PercAmostras = @40PercAmostras - 1
                            //                    set @AmostrasPorGrupo = @AmostrasPorGrupo - 1
                            percentualAmostras40 -= 1;
                            amostrasPorGrupo -= 1;
                            //                    --PRint '40PercAmostras: ' + convert( varchar( 5 ), @40PercAmostras )
                            //                end
                        }
                        //            end
                        //            else if ( @IsBellowLimit = 0 and @IsPrimeiroSubgrupo = 0)
                    }
                    else if ( !abaixoLimite && !primeiroSubGrupo )
                    {
                        //              begin
                        //                if (@60PercAmostras > 0)
                        if ( percentualAmostras60 > 0 )
                        {
                            //                  begin
                            //                    --PRint 'Terceiro caso'
                            //                    update FINAL
                            //                    set IsSample = 1,
                            //                        HasSubGrupo = 0
                            //                    from #MET_ARTICLES_FINAL as FINAL
                            //                    where FINAL.GroupNumber = @iAux
                            //                    and[ RANK ] = @iRank
                            artigosFinais = artigosFinais.Select( artigo =>
                            {
                                if ( artigo.Rank == indiceRank )
                                {
                                    artigo.Amostra = true;
                                    artigo.PossuiSubGrupo = false;
                                }
                                return artigo;
                            } ).ToArray();
                            //                    set @60PercAmostras = @60PercAmostras - 1
                            //                    set @AmostrasPorGrupo = @AmostrasPorGrupo - 1
                            percentualAmostras60 -= 1;
                            amostrasPorGrupo -= 1;
                            //                end
                            //                -- Como a tabela estÃ¡ ordenada por quantidades se chegamos ao @IsPrimeiroSubgrupo = 0 entÃ£o jÃ¡ usamos todas as
                            //                --amostras da variÃ¡vel @40PercAmostras que conseguimos, usamos aqui as que sobraram
                            //                else if (@40PercAmostras > 0)
                        }
                        else if ( percentualAmostras40 > 0 )
                        {
                            //                  begin
                            //                    --PRint 'Quarto caso'
                            //                    update FINAL
                            //                    set IsSample = 1,
                            //                        HasSubGrupo = 0
                            //                    from #MET_ARTICLES_FINAL as FINAL
                            //                      where FINAL.GroupNumber = @iAux
                            //                    and[ RANK ] = @iRank
                            artigosFinais = artigosFinais.Select( artigo =>
                            {
                                if ( artigo.Rank == indiceRank )
                                {
                                    artigo.Amostra = true;
                                    artigo.PossuiSubGrupo = false;
                                }
                                return artigo;
                            } ).ToArray();
                            //                    set @40PercAmostras = @40PercAmostras - 1
                            //                    set @AmostrasPorGrupo = @AmostrasPorGrupo - 1
                            percentualAmostras40 -= 1;
                            amostrasPorGrupo -= 1;
                            //                    --PRint '40PercAmostras: ' + convert( varchar( 5 ), @40PercAmostras )
                            //                end
                        }
                        //            end
                    }
                    //            set @iRank = @iRank + 1
                    indiceRank += 1;
                    //        end--(while ( @iRank <= @MaxRank )
                }
                //                update FINAL
                //        set HasSubGrupo = 0
                //        from #MET_ARTICLES_FINAL as FINAL
                //          where FINAL.[ RANK ] >= @iRank
                //        and FINAL.GroupNumber = @iAux
                //        set @iRank = 1
                //        set @iAux = @iAux + 1
                artigosFinais = artigosFinais.Select( artigo =>
                {
                    if ( artigo.Rank >= indiceRank )
                        artigo.PossuiSubGrupo = false;

                    return artigo;
                } ).ToArray();
                //    end
                //end
            }

            //retornando os dados

            //; with AUX as
            // (select
            //     FINAL.GroupNumber ,
            //    sum(FINAL.QtVendidasArtigo) as QtVendidasArtigo,
            //    FINAL.QtVendidasPorGrupo as QtVendidasPorGrupo,
            //    (sum(FINAL.QtVendidasArtigo) * convert(float, 100)) / FINAL.QtVendidasPorGrupo as FamilyPercInGroup
            //from #MET_ARTICLES_FINAL as FINAL
            //where FINAL.IsSample = 1
            //group by FINAL.GroupNumber, FINAL.QtVendidasPorGrupo)    
            //update FINAL
            //set FamilyPercInGroup = T.FamilyPercInGroup
            //from #MET_ARTICLES_FINAL as FINAL
            //inner join AUX as T on T.GroupNumber = FINAL.GroupNumber
            return artigosFinais.Select( artigo =>
            {
                artigo.PercentualFamiliaNoGrupo = artigosFinais.Where( a => a.NumeroGrupo == artigo.NumeroGrupo )
                .GroupBy( a => (a.NumeroGrupo, a.QuantidadeVendidasGrupo) )
                .Select( a => ( a.Sum( s => Convert.ToDecimal( s.QuantidadeVendidas ) ) * 100 ) / a.Key.QuantidadeVendidasGrupo )
                .Last();

                return artigo;
            } ).ToArray();
        }

        public MetaAmostraFinal[] CalcularAmostrasFinaisEtapa6( MetaParametrosCalculoAmostraDistribuicao parametros, MetaTotaisPorNumeroGrupo[] totaisPorNumeroGrupo )
        {
            //        --Calcular o numero de amostras utilizado se for mais ou menos de 300, devemos ajustÃ¡-las
            //           insert into #FinalSamples
            //        select

            //               ID,
            //               GroupNumber,
            //               NumAmostrasFamEtapa6
            //           from #TotalsByGroupNumber
            //        order by NumAmostrasFamEtapa6 desc, GroupNumber desc
            //       --RMC GroupNumber desc embora nÃ£o seja correcto, foi implementado para manter exactamente os mesmos resultados que a aplicaÃ§ao anterior do META Familias
            var amostrasFinais = totaisPorNumeroGrupo
                .OrderByDescending( o => o.AmostrasEtapa6 )
                .ThenByDescending( o => o.NumeroGrupo )
                .Select( t => new MetaAmostraFinal( t.NumeroGrupo, t.AmostrasEtapa6 ) ).ToArray();

            //       declare @NumAmostrasFinal int = ( select sum( NumAmostrasFinal ) from #FinalSamples)
            //        set @TableCount = (select count( ID ) from #FinalSamples)
            //        set @AuxID = 1;
            var amostrasFinal = amostrasFinais.Sum( amostra => amostra.AmostrasFinal );

            //            if ( @NumAmostrasFinal > @TotalAmostras ) --Devemos retirar amostras aos grupos com mais amostras atÃ© chegar aos 300
            if ( amostrasFinal > parametros.MaximumNumberOfSamples )
            {
                //        begin
                //            --PRint '(@NumAmostrasFinal > @TotalAmostras)'
                //            while ( @NumAmostrasFinal > @TotalAmostras )
                //                begin
                //                --PRint ' Primeiro caso'
                //                --PRint '@AuxID: ' + convert( varchar( 5 ), @AuxID )
                //                if ( @AuxID > @TableCount )
                //                set @AuxID = 1
                //                update TOTALS
                //                set TOTALS.NumAmostrasFinal = TOTALS.NumAmostrasFinal - 1
                //                from #FinalSamples as TOTALS            
                //                where ID = @AuxID
                //                set @AuxID = @AuxID + 1
                //                set @NumAmostrasFinal = @NumAmostrasFinal - 1
                //            end
                //        end
                while ( amostrasFinal > parametros.MaximumNumberOfSamples )
                {
                    amostrasFinais = amostrasFinais.Select( amostra =>
                    {
                        if ( amostrasFinal > parametros.MaximumNumberOfSamples )
                        {
                            amostra.AmostrasFinal -= 1;
                            amostrasFinal--;
                        }
                        return amostra;
                    } ).ToArray();
                }
            }
            //        else if ( @NumAmostrasFinal < @TotalAmostras ) --Devemos adicionar amostras aos grupos com mais amostras atÃ© chegar aos 300
            else if ( amostrasFinal < parametros.MaximumNumberOfSamples )
            {
                //        begin
                //            --PRint '(@NumAmostrasFinal < @TotalAmostras)'
                //            while ( @NumAmostrasFinal < @TotalAmostras )
                //                begin
                //                --PRint ' Segundo caso'
                //                --PRint '@AuxID: ' + convert( varchar( 5 ), @AuxID )
                //                if ( @AuxID > @TableCount )
                //                set @AuxID = 1
                //                update TOTALS
                //                set TOTALS.NumAmostrasFinal = TOTALS.NumAmostrasFinal + 1
                //                from #FinalSamples as TOTALS            
                //                where ID = @AuxID
                //                set @AuxID = @AuxID + 1
                //                set @NumAmostrasFinal = @NumAmostrasFinal + 1
                //            end
                //        end
                while ( amostrasFinal < parametros.MaximumNumberOfSamples )
                {
                    if ( amostrasFinais.Length == 0 )
                        break;

                    amostrasFinais = amostrasFinais.Select( amostra =>
                    {
                        if ( amostrasFinal < parametros.MaximumNumberOfSamples )
                        {
                            amostra.AmostrasFinal += 1;
                            amostrasFinal++;
                        }
                        return amostra;
                    } ).ToArray();
                }
            }

            return amostrasFinais;
        }

        public MetaTotaisPorNumeroGrupo[] CalcularAmostrasEtapa6( MetaParametrosCalculoAmostraDistribuicao parametros, MetaArtigoOrdenado[] artigosOrdenados, MetaTotaisFamilia2[] totaisFamilia2, MetaTotaisPorNumeroGrupo[] totaisPorNumeroGrupo )
        {
            //    set @AmostrasPorDistribuir = @TotalAmostras - @AmostrasDistribuidas
            var amostrasPorDistribuir = ObterTotalAmostrasPorDistribuir( parametros, totaisPorNumeroGrupo );
            var amostrasSemMinimo = ObterAmostrasSemMinimo( parametros, artigosOrdenados, totaisFamilia2, totaisPorNumeroGrupo );
            //        begin
            //                    -- Se ainda sobram amostras distribui - las de acordo com a formula( Ponto 12.1 )

            //        update TG
            //        --Multiplica - se por 1000 logo no inicio para nao perdermos precisÃ£o
            //         set TG.PercRestantesAmostras = ( ( convert( float, TG.NumArtigosGrupo ) * 1000 ) / convert( float, TG.QtVendidasGrupo ) ) * TG.PercQtVendidasGrupo
            //        from #TotalsByGroupNumber as TG;
            totaisPorNumeroGrupo = totaisPorNumeroGrupo.Select( total =>
            {
                total.PercentualAmostrasRestantes =
                ( ( Convert.ToDecimal( total.QuantidadeArtigosGrupo ) * 1000 ) / Convert.ToDecimal( total.QuantidadeVendidasGrupo ) ) * total.PercentualVendidasGrupo;
                return total;
            } ).ToArray();

            //  -- Parar sÃ³ qdo as percentagens de diferenÃ§as relativas forem 100 %
            //        declare @SumPercRelativa float = 0;
            var somaPercentualRelativa = 0.0m;
            //            while ( @SumPercRelativa < 100 )
            //                begin
            //                    -- it's inside the while because the first sum doesn't count for the 100 % !
            //                    set @SumPercRelativa = ( select sum( PercRestantesAmostras ) from #TotalsByGroupNumber);
            //                      with PERC as
            //                           (select
            //                               GroupNumber,
            //                            PercRestantesAmostras,
            //                            case 
            //                                when( ( PercRestantesAmostras * convert( float, 100 ) ) / @SumPercRelativa ) > 25
            //                                then 25
            //                        else ( ( PercRestantesAmostras * convert( float, 100 ) ) / @SumPercRelativa )
            //                    end as NewPercRestantesAmostras
            //                from #TotalsByGroupNumber)
            //                update TG
            //                    set TG.PercRestantesAmostras = P.NewPercRestantesAmostras
            //                from #TotalsByGroupNumber as TG
            //                inner join PERC as P on P.GroupNumber = TG.GroupNumber
            //            set @SumPercRelativa = ( select sum( PercRestantesAmostras ) from #TotalsByGroupNumber)
            //        end;
            while ( somaPercentualRelativa < 100 )
            {
                if ( totaisPorNumeroGrupo.Count() == 0 )
                    break;

                somaPercentualRelativa = totaisPorNumeroGrupo.Sum( total => total.PercentualAmostrasRestantes );

                totaisPorNumeroGrupo = totaisPorNumeroGrupo.Select( total =>
                {
                    var percentual = ( total.PercentualAmostrasRestantes * 100.00m ) / somaPercentualRelativa;
                    total.PercentualAmostrasRestantes = percentual > 25.00m ? 25.00m : percentual;
                    return total;
                } ).ToArray();

                somaPercentualRelativa = totaisPorNumeroGrupo.Sum( total => total.PercentualAmostrasRestantes );
            }

            //        --Calcular numero final de amostras
            //        update TOTALS_GROUP
            //        set TOTALS_GROUP.NumAmostrasFamEtapa6 = case
            //                                                    when SM.SumAmostrasEtapa2 < 10 then TOTALS_GROUP.NumAmostrasFamEtapa5
            //                                                    else round( ( ( TOTALS_GROUP.PercRestantesAmostras / 100 ) * @AmostrasPorDistribuir ), 0 ) + TOTALS_GROUP.NumAmostrasFamEtapa5
            //                                                end
            //        from #TotalsByGroupNumber as TOTALS_GROUP
            //        left join @SamplesWithoutMin as SM on SM.GroupNumber = TOTALS_GROUP.GroupNumber;
            totaisPorNumeroGrupo = totaisPorNumeroGrupo.Select( total =>
            {
                if ( amostrasSemMinimo.Any( a => a.NumeroGrupo == total.NumeroGrupo ) && amostrasSemMinimo.First( a => a.NumeroGrupo == total.NumeroGrupo ).SomaAmostrasEtapa2 < 10 )
                    total.AmostrasEtapa6 = total.AmostrasEtapa5;
                else
                    total.AmostrasEtapa6 = Convert.ToInt32( Math.Round( ( ( total.PercentualAmostrasRestantes / 100 ) * amostrasPorDistribuir ), 0 ) ) + total.AmostrasEtapa5;
                return total;
            } ).ToArray();

            return totaisPorNumeroGrupo;
        }

        public MetaAmostraFinal[] CalcularAmostrasFinaisEtapa5( MetaTotaisPorNumeroGrupo[] totaisPorNumeroGrupo )
        {
            return totaisPorNumeroGrupo.Select( t => new MetaAmostraFinal( t.NumeroGrupo, t.AmostrasEtapa5 ) ).ToArray();
        }

        public MetaTotaisPorNumeroGrupo[] CalcularAmostrasEtapa5( MetaParametrosCalculoAmostraDistribuicao parametros, MetaArtigoOrdenado[] artigosOrdenados, MetaTotaisFamilia2[] totaisFamilia2, MetaTotaisPorNumeroGrupo[] totaisPorNumeroGrupo )
        {
            var amostrasPorDistribuir = ObterTotalAmostrasPorDistribuir( parametros, totaisPorNumeroGrupo );
            decimal somaAmostrasSemMinimo = ObterSomaAmostrasSemMinimo( parametros, totaisPorNumeroGrupo );
            var amostrasSemMinimo = ObterAmostrasSemMinimo( parametros, artigosOrdenados, totaisFamilia2, totaisPorNumeroGrupo );
            var amostrasPrecisasParaMinimos = CalcularAmostrasPrecisasParaMinimo( parametros, totaisPorNumeroGrupo, amostrasSemMinimo, somaAmostrasSemMinimo );

            //    --Se houver amostras suficientes para todos os grupos atingirem o numero minimo de amostras
            //    if ( @AmostrasPorDistribuir >= @AmostrasPrecisasParaMinimos )
            //                begin
            //                    update @SamplesWithoutMin
            //                    set NumAmostrasFamEtapa5 = @MinimoAmostrasGrupo
            //            where SumAmostrasEtapa2 >= 10
            //            update @SamplesWithoutMin
            //            set NumAmostrasFamEtapa5 = SumAmostrasEtapa2
            //            where SumAmostrasEtapa2< 10
            //        end
            if ( amostrasPorDistribuir >= amostrasPrecisasParaMinimos )
            {
                amostrasSemMinimo = amostrasSemMinimo.Select( amostra =>
                {
                    amostra.AmostrasEtapa5 = amostra.SomaAmostrasEtapa2 >= 10 ? parametros.MinimumNumberOfSamplesPerGroup : amostra.SomaAmostrasEtapa2;
                    return amostra;
                } ).ToArray();
            }
            //    else --Se o nÃºmero de amostras nÃ£o for suficiente para completar minimos, distribuir equitativamente
            //       begin
            //            while ( @AmostrasPorDistribuir > 0 )
            //                    begin
            //                    if ( @AuxID > @TableCount )
            //                set @AuxID = 1
            //                    set @LimitReached = ( select case
            //                                                when SumAmostrasEtapa2< 10 and NumAmostrasFamEtapa5 = SumAmostrasEtapa2 then 1
            //                                                when NumAmostrasFamEtapa5 = 10 then 1
            //                                                else 0
            //                                            end
            //                                        from @SamplesWithoutMin where ID = @AuxID)
            //                  if ( @LimitReached = 0 )
            //                    begin
            //                        update SAMPLES
            //                        set NumAmostrasFamEtapa5 = NumAmostrasFamEtapa5 + 1
            //                            from @SamplesWithoutMin as SAMPLES
            //                            where ID = @AuxID
            //                            set @AmostrasPorDistribuir = @AmostrasPorDistribuir - 1
            //                        end
            //                        set @AuxID = @AuxID + 1
            //                    end
            //        end;
            //            --Update the totals with the new samples values
            //           -- Calculate new percentage to distribute remaining samples
            else
            {
                while ( amostrasPorDistribuir > 0 )
                {
                    var _amostrasSemMinimo = amostrasSemMinimo.Where( a =>
                    ( a.SomaAmostrasEtapa2 < 10 && a.AmostrasEtapa5 == a.SomaAmostrasEtapa2 ) || a.AmostrasEtapa5 == 10 ).ToArray();

                    _amostrasSemMinimo = _amostrasSemMinimo.Select( amostraSemMinimo =>
                    {
                        if ( amostrasPorDistribuir > 0 )
                        {
                            amostraSemMinimo.AmostrasEtapa5 += 1;
                            amostrasPorDistribuir--;
                        }
                        return amostraSemMinimo;
                    } ).ToArray();

                    if ( _amostrasSemMinimo.Length == 0 )
                        break; // prevent infinite loop
                }
            }

            //    update TOTALS_GROUP
            //    set TOTALS_GROUP.NumAmostrasFamEtapa5 = SAMPLES.NumAmostrasFamEtapa5
            //    from #TotalsByGroupNumber as TOTALS_GROUP
            //    inner join @SamplesWithoutMin as SAMPLES on SAMPLES.GroupNumber = TOTALS_GROUP.GroupNumber
            totaisPorNumeroGrupo = totaisPorNumeroGrupo.Select( total =>
            {
                if ( amostrasSemMinimo.Any( a => a.NumeroGrupo == total.NumeroGrupo ) )
                    total.AmostrasEtapa5 = amostrasSemMinimo.Last( a => a.NumeroGrupo == total.NumeroGrupo ).AmostrasEtapa5;

                return total;
            } ).ToArray();

            //   begin
            //      -- Se nao houve mais calculos colocar os valores das amostras da etapa 5
            //      -- como amostras finais
            //         PRint 'nao hÃ¡ mais calculos colocar os valores das amostras da etapa 5'
            //        insert into #FinalSamples
            //        select
            //            ID,
            //            GroupNumber,
            //            NumAmostrasFamEtapa5
            //        from #TotalsByGroupNumber
            //    end
            //    --01:14
            //    PRint 'Finished distributing the samples'
            //    PRint 'Etapa 6 - Finished'
            //end;
            return totaisPorNumeroGrupo;
        }

        public MetaTotaisPorNumeroGrupo[] CalcularAcertos( MetaParametrosCalculoAmostraDistribuicao parametros, MetaTotaisFamilia2[] totaisFamilia2 )
        {
            var percentualUmaAmostra = Convert.ToDecimal( 1 / ( parametros.MaximumNumberOfSamples * 0.01 ) );
            //-- Calculando os acertos
            //--#TotalsByGroupNumber
            //with TOTALS_GROUP as
            //(select 
            //    FAM.GroupNumber,
            //    (select sum(SOMA.NumArtigosFamilia) from #MET_FAMILY_TOTALS as SOMA where SOMA.GroupNumber = FAM.GroupNumber) 
            //        as NumArtigosGrupo,
            //    (select sum(SOMA.QtVendidasFamilia) from #MET_FAMILY_TOTALS as SOMA where SOMA.GroupNumber = FAM.GroupNumber) 
            //        as QtVendidasGrupo,
            //    (select sum(SOMA.PercQtVendidasFamilia) from #MET_FAMILY_TOTALS as SOMA where SOMA.GroupNumber = FAM.GroupNumber) 
            //        as PercQtVendidasGrupo,
            //    FAM.PercQtVendidasFamilia - ( @PercUmaAmostra * FAM.NumAmostrasFamEtapa1 ) 
            //        as PercQtVendidasPerdida,
            //    (select isnull(sum(SOMA.PercQtVendidasFamilia),0) from #MET_FAMILY_TOTALS as SOMA where SOMA.TemRepresentatividadeGrupo = 0 and SOMA.GroupNumber = FAM.GroupNumber) 
            //        as SumPercQtVendidasNaoUsadas,
            //    null as PercQtVendidasAcertada,
            //    FAM.NumAmostrasFamEtapa1 as NumAmostrasFamEtapa1,
            //    FAM.NumAmostrasFamEtapa2 as NumAmostrasFamEtapa2,
            //    null as NumAmostrasFamEtapa5,
            //    null as NumAmostrasFamEtapa6
            //from #MET_FAMILY_TOTALS as FAM
            //where FAM.TemRepresentatividadeGrupo = 1)
            //select 
            //    GroupNumber, 
            //    NumArtigosGrupo,
            //    QtVendidasGrupo,
            //    PercQtVendidasGrupo,
            //    PercQtVendidasPerdida,
            //    SumPercQtVendidasNaoUsadas,
            //    PercQtVendidasPerdida + SumPercQtVendidasNaoUsadas as PercQtVendidasAcertada,
            //    null as PercRestantesAmostras,
            //    --null as PercDiferencasRelativas,
            //    NumAmostrasFamEtapa1,
            //    NumAmostrasFamEtapa2,
            //    abs( (PercQtVendidasPerdida + SumPercQtVendidasNaoUsadas) / @PercUmaAmostra) + NumAmostrasFamEtapa2 as NumAmostrasFamEtapa5,
            //    null as NumAmostrasFamEtapa6
            //from TOTALS_GROUP
            return totaisFamilia2.Where( t => t.PossuiRepresentatividade ).Select( total =>
            {
                var _totaisGrupo = totaisFamilia2.Where( t => t.NumeroGrupo == total.NumeroGrupo );
                var percentualVendidasPerdida = total.PercentualVendidasFamilia - ( percentualUmaAmostra * total.AmostrasEtapa1 );
                var percentualVendidasNaoUsadas = totaisFamilia2.Where( t => !t.PossuiRepresentatividade && t.NumeroGrupo == total.NumeroGrupo ).Sum( t => t.PercentualVendidasFamilia );
                var percentualVendidasAcertadas = percentualVendidasPerdida + percentualVendidasNaoUsadas;

                return new MetaTotaisPorNumeroGrupo(
                    total.NumeroGrupo,
                    _totaisGrupo.Sum( t => t.QuantidadeArtigos ),
                    _totaisGrupo.Sum( t => t.QuantidadeVendidas ),
                    _totaisGrupo.Sum( t => t.PercentualVendidasFamilia ),
                    percentualVendidasPerdida,
                    percentualVendidasNaoUsadas,
                    percentualVendidasAcertadas,
                    0.0m,
                    total.AmostrasEtapa1,
                    total.AmostrasEtapa2,
                    Convert.ToInt32( Math.Floor( Math.Abs( ( Math.Round( percentualVendidasPerdida, 6 ) + Math.Round( percentualVendidasNaoUsadas, 6 ) ) / Math.Round( percentualUmaAmostra, 6 ) ) ) + total.AmostrasEtapa2 ),
                    0
                );
            } ).ToArray();
        }

        public MetaTotaisFamilia2[] AtribuirNumeroGrupo( MetaTotaisFamilia2[] totaisFamilia2 )
        {

            //-- Numerar as Familias
            //while (@i <= @iCount)
            //begin
            //    update FAM 
            //    set FAM.GroupNumber = (select isnull(max(GroupNumber) + 1, 1) from #MET_FAMILY_TOTALS)
            //    from #MET_FAMILY_TOTALS as FAM
            //    where FAM.TemRepresentatividadeGrupo = 1
            //    and FAM.IDFamily = @i
            //    set @i = @i + 1
            //end;
            var numeroGrupo = 1;
            totaisFamilia2 = totaisFamilia2.OrderBy( t => t.FamiliaId ).Select( total =>
            {
                if ( total.PossuiRepresentatividade )
                    total.NumeroGrupo = numeroGrupo++;
                return total;
            } ).ToArray();
            //-- Atribuição de group number
            //-- Iterate all the Families without GroupNumber, if a family within the same group (level = 1) has a groupNumber
            //-- use that number, if more than one family within the same group have a GroupNumber use the smaller number
            //with GROUP_min as 
            //(select FAM.ID,
            //        min(FAM_AUX.GroupNumber) as GroupNumber
            //from #MET_FAMILY_TOTALS as FAM
            //inner join MET_MATERIAL_FAMILY_GROUP_RELATIon as REL on REL.IDFamily = FAM.IDFamily
            //inner join MET_MATERIAL_FAMILY_GROUP as FG on FG.ID = REL.IDGroup and FG.[Level] = 1 
            //inner join MET_MATERIAL_FAMILY_GROUP_RELATIon as REL_AUX on REL_AUX.IDGroup = REL.IDGroup
            //inner join MET_MATERIAL_FAMILY_GROUP as FG_AUX on FG_AUX.ID = REL_AUX.IDGroup and FG_AUX.[Level] = 1
            //left join #MET_FAMILY_TOTALS as FAM_AUX on FAM_AUX.IDFamily = REL_AUX.IDFamily 
            //where FAM.TemGroupNumber = 0
            //and FAM_AUX.TemGroupNumber = 1
            //group by FAM.ID)
            //update FAM
            //set FAM.GroupNumber = GM.GroupNumber,
            //    FAM.TemGroupNumber = 1
            //from #MET_FAMILY_TOTALS FAM
            //inner join GROUP_min as GM on FAM.ID = GM.ID;

            //-- Iterate all the Families without GroupNumber, if a family within the same group (level = 2) has a groupNumber
            //-- use that number, if more than one family within the same group have a GroupNumber use the smaller number
            //with GROUP_min as 
            //(select FAM.ID,
            //        min(FAM_AUX.GroupNumber) as GroupNumber
            //from #MET_FAMILY_TOTALS as FAM
            //inner join MET_MATERIAL_FAMILY_GROUP_RELATIon as REL on REL.IDFamily = FAM.IDFamily
            //inner join MET_MATERIAL_FAMILY_GROUP as FG on FG.ID = REL.IDGroup and FG.[Level] = 2 
            //inner join MET_MATERIAL_FAMILY_GROUP_RELATIon as REL_AUX on REL_AUX.IDGroup = REL.IDGroup
            //inner join MET_MATERIAL_FAMILY_GROUP as FG_AUX on FG_AUX.ID = REL_AUX.IDGroup and FG_AUX.[Level] = 2
            //left join #MET_FAMILY_TOTALS as FAM_AUX on FAM_AUX.IDFamily = REL_AUX.IDFamily 
            //where FAM.TemGroupNumber = 0
            //and FAM_AUX.TemGroupNumber = 1
            //group by FAM.ID)
            //update FAM
            //set GroupNumber = GM.GroupNumber,
            //    FAM.TemGroupNumber = 1
            //from #MET_FAMILY_TOTALS FAM
            //inner join GROUP_min as GM on FAM.ID = GM.ID;

            //-- Iterate all the Families without GroupNumber, if a family within the same group (level = 3) has a groupNumber
            //-- use that number, if more than one family within the same group have a GroupNumber use the smaller number
            //with GROUP_min as 
            //(select FAM.ID,
            //        min(FAM_AUX.GroupNumber) as GroupNumber
            //from #MET_FAMILY_TOTALS as FAM
            //inner join MET_MATERIAL_FAMILY_GROUP_RELATIon as REL on REL.IDFamily = FAM.IDFamily
            //inner join MET_MATERIAL_FAMILY_GROUP as FG on FG.ID = REL.IDGroup and FG.[Level] = 3 
            //inner join MET_MATERIAL_FAMILY_GROUP_RELATIon as REL_AUX on REL_AUX.IDGroup = REL.IDGroup
            //inner join MET_MATERIAL_FAMILY_GROUP as FG_AUX on FG_AUX.ID = REL_AUX.IDGroup and FG_AUX.[Level] = 3
            //left join #MET_FAMILY_TOTALS as FAM_AUX on FAM_AUX.IDFamily = REL_AUX.IDFamily 
            //where FAM.TemGroupNumber = 0
            //and FAM_AUX.TemGroupNumber = 1
            //group by FAM.ID)
            //update FAM
            //    set GroupNumber = GM.GroupNumber,
            //    FAM.TemGroupNumber = 1
            //from #MET_FAMILY_TOTALS FAM
            //inner join GROUP_min as GM on FAM.ID = GM.ID

            foreach ( var nivel in new[] { 1, 2, 3 } )
            {
                var familiasMesmoGrupoNivel = totaisFamilia2.Select( total =>
                {
                    return new
                    {
                        Key = total.FamiliaId,
                        Value = total.Grupos
                        .Where( g => g.Nivel == nivel )
                        .SelectMany( g => totaisFamilia2
                        .Where( t => t.Grupos.Any( tt => tt.Id == g.Id ) )
                        .Select( f => f.FamiliaId ) ).ToArray()
                    };

                } ).ToDictionary( x => x.Key, x => x.Value );

                totaisFamilia2 = totaisFamilia2.OrderBy( t => t.FamiliaId ).Select( total =>
                {
                    if ( total.NumeroGrupo == 0 )
                        total.NumeroGrupo = totaisFamilia2.Where( t => t.NumeroGrupo > 0
                        && familiasMesmoGrupoNivel[ total.FamiliaId ]
                        .Contains( t.FamiliaId ) )
                        .Min( t => t.NumeroGrupo );

                    return total;
                } ).ToArray();
            }

            return totaisFamilia2;
        }

        public MetaTotaisFamilia2[] ObterTotaisFamilia2( MetaTotaisFamilia[] totaisFamilia, MetaTotaisGrupoFamilia[] totaisGruposFamilia )
        {
            //--#MET_FAMILY_TOTALS
            //select
            //    FAM.IDFamily,
            //    FAM.FamilyToken,
            //    FAM.NumArtigosFamilia,
            //    FAM.QtVendidasFamilia,
            //    FAM.PercQtVendidasFamilia,
            //    FAM_GROUP1.PercQtVendidasGroup as PercQtVendiasGrupo1,
            //    FAM_GROUP2.PercQtVendidasGroup as PercQtVendiasGrupo2,
            //    FAM_GROUP3.PercQtVendidasGroup as PercQtVendiasGrupo3,
            //    case 
            //        when FAM.NumAmostrasFamEtapa2 > 0 then 1
            //        when FAM_GROUP1.PercQtVendidasGroup >= 1 and FAM_GROUP1.GrupoTemAmostras = 0 then 1 --Soma do grupo Ã© maior que 1%?
            //        else 0
            //    end as TemRepresentatividade,
            //    null as GroupNumber,
            //    case 
            //        when FAM.NumAmostrasFamEtapa2 > 0 then 1
            //        when FAM_GROUP1.PercQtVendidasGroup >= 1 and FAM_GROUP1.GrupoTemAmostras = 0 then 1 --Soma do grupo Ã© maior que 1%?
            //        else 0
            //    end as TemGroupNumber,
            //    FAM.NumAmostrasFamEtapa1,
            //    FAM.NumAmostrasFamEtapa2
            //from #FamilyTotals as FAM
            //inner join @FamilyGroupTotals as FAM_GROUP1 on FAM_GROUP1.IDFamily = FAM.IDFamily and FAM_GROUP1.FamilyGroupLevel = 1
            //inner join @FamilyGroupTotals as FAM_GROUP2 on FAM_GROUP2.IDFamily = FAM.IDFamily and FAM_GROUP2.FamilyGroupLevel = 2
            //inner join @FamilyGroupTotals as FAM_GROUP3 on FAM_GROUP3.IDFamily = FAM.IDFamily and FAM_GROUP3.FamilyGroupLevel = 3
            return totaisFamilia.Where( total => total.Grupos.Any( g => g.Nivel == 1 )
            && total.Grupos.Any( g => g.Nivel == 2 )
            && total.Grupos.Any( g => g.Nivel == 3 ) )
                .Join( totaisGruposFamilia.Where( tgf => tgf.NivelGrupoFamilia == 1 ).ToArray()
                , outer => outer.FamiliaId
                , inner => inner.FamiliaId
                , ( outer, inner ) => (outer.FamiliaId
                , outer.Token
                , outer.QuantidadeArtigos
                , outer.QuantidadeVendidas
                , outer.PercentualVendidasFamilia
                , outer.AmostrasEtapa1
                , outer.AmostrasEtapa2
                , outer.Grupos
                , PossuiRepresentatividade: outer.AmostrasEtapa2 > 0 || ( inner.PercentualVendidasGrupo >= 1 && inner.AmostrasEtapa2Grupo == 0 )
                , PercentualQuantidadeVendidasGrupo1: inner.PercentualVendidasGrupo) )
                .Join( totaisGruposFamilia.Where( tgf => tgf.NivelGrupoFamilia == 2 ).ToArray()
                , outer => outer.FamiliaId
                , inner => inner.FamiliaId
                , ( outer, inner ) => (outer.FamiliaId
                , outer.Token
                , outer.QuantidadeArtigos
                , outer.QuantidadeVendidas
                , outer.PercentualVendidasFamilia
                , outer.AmostrasEtapa1
                , outer.AmostrasEtapa2
                , outer.Grupos
                , outer.PossuiRepresentatividade
                , outer.PercentualQuantidadeVendidasGrupo1
                , PercentualQuantidadeVendidasGrupo2: inner.PercentualVendidasGrupo) )
                .Join( totaisGruposFamilia.Where( tgf => tgf.NivelGrupoFamilia == 3 ).ToArray()
                , outer => outer.FamiliaId
                , inner => inner.FamiliaId
                , ( outer, inner ) => new MetaTotaisFamilia2( outer.FamiliaId
                , outer.Token
                , outer.QuantidadeArtigos
                , outer.QuantidadeVendidas
                , outer.PercentualVendidasFamilia
                , outer.AmostrasEtapa1
                , outer.AmostrasEtapa2
                , outer.Grupos
                , outer.PossuiRepresentatividade
                , outer.PercentualQuantidadeVendidasGrupo1
                , outer.PercentualQuantidadeVendidasGrupo2
                , inner.PercentualVendidasGrupo
                , 0 ) )
                .ToArray();


        }

        public MetaTotaisGrupoFamilia[] CalcularTotaisGruposPorFamilia( MetaTotaisFamilia[] totaisFamilia )
        {
            //--@FamilyGroupTotals : Totais por FamilyGroup
            //with GROUP_TOTAL as
            //(select 
            //    FAM_GR.[Level] as FamilyGroupLevel,
            //    FAM_GR.[Order] as FamilyGroupOrder,
            //    sum(FAM_TOTALS.PercQtVendidasFamilia) as PercQtVendidasGroup, -- Percentages by all groups, first, second and third groups(levels)
            //    sum(FAM_TOTALS.NumAmostrasFamEtapa2) as GrupoTemAmostras
            //from #FamilyTotals as FAM_TOTALS
            //inner join MET_MATERIAL_FAMILY_GROUP_RELATIon as REL on REL.IDFamily = FAM_TOTALS.IDFamily
            //inner join MET_MATERIAL_FAMILY_GROUP as FAM_GR on FAM_GR.ID = REL.IDGroup
            //group by FAM_GR.Level, FAM_GR.[Order])

            var totaisGrupos = totaisFamilia.SelectMany( total =>
            {
                return total.Grupos.Select( grupo => (
                   NivelGrupoFamilia: grupo.Nivel,
                   OrdemGrupoFamilia: grupo.Ordem,
                   total.PercentualVendidasFamilia,
                   total.AmostrasEtapa2
               ) ).ToArray();
            } )
                .GroupBy( g => (g.NivelGrupoFamilia, g.OrdemGrupoFamilia) )
                .Select( s => (
                    s.Key.NivelGrupoFamilia,
                    s.Key.OrdemGrupoFamilia,
                    PercentualVendidasGrupo: s.Sum( ss => ss.PercentualVendidasFamilia ),
                    AmostrasEtapa2Grupo: s.Sum( ss => ss.AmostrasEtapa2 )
                ) )
                .ToArray();

            //@FamilyGroupTotals
            //select 
            //    FAM_TOTALS.IDFamily,
            //    FAM_TOTALS.FamilyToken,
            //    FAM_TOTALS.NumAmostrasFamEtapa1,
            //    FAM_TOTALS.NumAmostrasFamEtapa2,
            //    GT.FamilyGroupLevel,
            //    GT.FamilyGroupOrder,
            //    FAM_TOTALS.PercQtVendidasFamilia,
            //    GT.PercQtVendidasGroup,
            //    GT.GrupoTemAmostras
            //from #FamilyTotals as FAM_TOTALS
            //inner join MET_MATERIAL_FAMILY_GROUP_RELATIon REL on REL.IDFamily = FAM_TOTALS.IDFamily
            //inner join MET_MATERIAL_FAMILY_GROUP FAM_GR on FAM_GR.ID = REL.IDGroup
            //inner join GROUP_TOTAL as GT on GT.FamilyGroupLevel = FAM_GR.[Level] and GT.FamilyGroupOrder = FAM_GR.[Order]
            //order by FAM_TOTALS.IDFamily

            return totaisFamilia.SelectMany( total =>
            {
                return total.Grupos
                .Join( totaisGrupos
                , outer => (outer.Nivel, outer.Ordem)
                , inner => (Nivel: inner.NivelGrupoFamilia, Ordem: inner.OrdemGrupoFamilia)
                , ( outer, inner ) => new MetaTotaisGrupoFamilia(
                          total.FamiliaId,
                          total.Token,
                          total.AmostrasEtapa1,
                          total.AmostrasEtapa2,
                          inner.NivelGrupoFamilia,
                          inner.OrdemGrupoFamilia,
                          total.PercentualVendidasFamilia,
                          inner.PercentualVendidasGrupo,
                          inner.AmostrasEtapa2Grupo
                  ) );

            } ).ToArray();
        }

        public MetaTotaisFamilia[] CalcularAmostrasEtapa2PorFamilia( MetaParametrosCalculoAmostraDistribuicao parametros, MetaTotaisFamilia[] totaisFamilia, MetaArtigoOrdenado[] artigosOrdenados )
        {
            var dezPercentualTotalAmostras = Convert.ToInt32( Math.Abs( parametros.MaximumNumberOfSamples * 0.10 ) );

            //    -- O novo número de amostras serão o valor mais baixo entre o numero de amostras da 1a etapa, da 2a etapa e 10% do total de amostras
            //    update FAM_TOTALS
            //    set FAM_TOTALS.NumAmostrasFamEtapa2 = (select min([Amostras]) from (values(ORD_ART.NumAmostrasFam1Etapa),(ORD_ART.NumAmostrasFam2Etapa),(DezPercTotalAmostras)) x([Amostras]))
            //    from #FamilyTotals FAM_TOTALS
            //    inner join #MET_ORDERED_ARTICLES ORD_ART on ORD_ART.IDFamily = FAM_TOTALS.IDFamily
            totaisFamilia = totaisFamilia.Select( total =>
            {
                var artigosOrdenadosFamilia = artigosOrdenados.Where( a => a.FamiliaId == total.FamiliaId ).ToArray();
                total.AmostrasEtapa2 = new int[] {
                      artigosOrdenadosFamilia.Min( a => a.AmostrasEtapa1 )
                     , artigosOrdenadosFamilia.Min( a => a.AmostrasEtapa2 )
                     , dezPercentualTotalAmostras }.Min();
                return total;
            } ).ToArray();
            return totaisFamilia;
        }

        public MetaArtigoOrdenado[] CalcularAmostrasEtapa2( MetaParametrosCalculoAmostraDistribuicao parametros, MetaArtigoOrdenado[] artigosOrdenados )
        {
            //    -- Obtém as Percentagens
            //    -- #Percentagens
            //     select
            //            ORD_ART.IDFamily,
            //            ORD_ART.IDVariability,
            //            FirstRepresPercentage,
            //            (select min(ORD_FIRST.OrderPercQtAcumulada) from #MET_ORDERED_ARTICLES as ORD_FIRST where ORD_FIRST.PercQtAcumulada >= FirstRepresPercentage and ORD_FIRST.IDFamily = ORD_ART.IDFamily) as NumAmostras,
            //            ORD_ART.IsHomogeneo
            //        from #MET_ORDERED_ARTICLES as ORD_ART
            //        group by ORD_ART.IDFamily, ORD_ART.IDVariability, ORD_ART.IsHomogeneo

            //        select
            //            ORD_ART.IDFamily,
            //            ORD_ART.IDVariability,
            //            SecondRepresPercentage,
            //            (select min(ORD_SEConD.OrderPercQtAcumulada) from #MET_ORDERED_ARTICLES as ORD_SEConD where ORD_SEConD.PercQtAcumulada >= SecondRepresPercentage and ORD_SEConD.IDFamily = ORD_ART.IDFamily) as NumAmostras,
            //            ORD_ART.IsHomogeneo
            //        from #MET_ORDERED_ARTICLES as ORD_ART
            //        group by ORD_ART.IDFamily, ORD_ART.IDVariability, ORD_ART.IsHomogeneo
            var grupo2 = artigosOrdenados.GroupBy( a => (a.FamiliaId, a.VariabilidadeId, a.Homogeneo) )
                                         .Select( g => (
                                             g.Key.FamiliaId,
                                             g.Key.VariabilidadeId,
                                             g.Key.Homogeneo,
                                             QuantidadeAmostras: artigosOrdenados.Where( ao => ao.PercentualQuantidadeAcumulada >= parametros.SecondClassPercentage && ao.FamiliaId == g.Key.FamiliaId )
                                             .Min( x => x.OrdemPercentualQuantidadeAcumulada )
                                         ) )
                                         .ToArray();
            //        select
            //            ORD_ART.IDFamily,
            //            ORD_ART.IDVariability,
            //            ThirdRepresPercentage,
            //            (select min(ORD_THIRD.OrderPercQtAcumulada) from #MET_ORDERED_ARTICLES as ORD_THIRD where ORD_THIRD.PercQtAcumulada >= ThirdRepresPercentage and ORD_THIRD.IDFamily = ORD_ART.IDFamily) as NumAmostras,
            //            ORD_ART.IsHomogeneo
            //        from #MET_ORDERED_ARTICLES as ORD_ART
            //        group by ORD_ART.IDFamily, ORD_ART.IDVariability, ORD_ART.IsHomogeneo
            var grupo3 = artigosOrdenados.GroupBy( a => (a.FamiliaId, a.VariabilidadeId, a.Homogeneo) )
                                         .Select( g => (
                                             g.Key.FamiliaId,
                                             g.Key.VariabilidadeId,
                                             g.Key.Homogeneo,
                                             QuantidadeAmostras: artigosOrdenados.Where( ao => ao.PercentualQuantidadeAcumulada >= parametros.ThirdClassPercentage && ao.FamiliaId == g.Key.FamiliaId )
                                             .Min( x => x.OrdemPercentualQuantidadeAcumulada )
                                         ) ).ToArray();

            //    -- Atualiza Ordered Articles (#MET_ORDERED_ARTICLES), Número de Amostras da Família 2a Etapa
            //    update ORD_ART
            //    set ORD_ART.NumAmostrasFam2Etapa = case 
            //                    when ORD_ART.IsHomogeneo = 0 and PERC95.PercentagesToUse = PercRepresNaoHomogeneos
            //                        then PERC95.NumAmostras 
            //                    when PERC75.PercentagesToUse = PercRepresHomogeneosMin --75%
            //                            and PERC75.NumAmostras > ValorMinAmostrasHomogeneos --5 
            //                            and PERC75.NumAmostras < ValorMaxAmostrasHomogeneos --10
            //                        then PERC75.NumAmostras --75%
            //                    when PERC95.PercentagesToUse = PercRepresHomogeneosMax --95%
            //                            and PERC95.NumAmostras < ValorMaxAmostrasHomogeneos --10
            //                        then PERC95.NumAmostras --95%
            //                    else ValorMaxAmostrasHomogeneos --10
            //                end
            //    from #MET_ORDERED_ARTICLES as ORD_ART
            //    inner join #Percentages as PERC50 on PERC50.IDFamily = ORD_ART.IDFamily and PERC50.PercentagesToUse = FirstRepresPercentage
            //    inner join #Percentages as PERC75 on PERC75.IDFamily = ORD_ART.IDFamily and PERC75.PercentagesToUse = SecondRepresPercentage
            //    inner join #Percentages as PERC95 on PERC95.IDFamily = ORD_ART.IDFamily and PERC95.PercentagesToUse = ThirdRepresPercentage        


            artigosOrdenados = artigosOrdenados.Select( artigo =>
            {
                var quantidadeAmostras2 = grupo2.Single( g => g.FamiliaId == artigo.FamiliaId ).QuantidadeAmostras;
                var quantidadeAmostras3 = grupo3.Single( g => g.FamiliaId == artigo.FamiliaId ).QuantidadeAmostras;

                if ( !artigo.Homogeneo && parametros.NonHomogeneousPercentage == parametros.ThirdClassPercentage )
                {
                    artigo.AmostrasEtapa2 = quantidadeAmostras3;
                }
                else if ( parametros.MinHomogeneousPercentage == parametros.SecondClassPercentage
                && quantidadeAmostras2 > parametros.MinValueHomogeneousSample
                && quantidadeAmostras2 < parametros.MaxValueHomogeneousSample )
                {
                    artigo.AmostrasEtapa2 = quantidadeAmostras2;
                }
                else if ( parametros.MaxHomogeneousPercentage == parametros.ThirdClassPercentage && quantidadeAmostras3 < parametros.MaxValueHomogeneousSample )
                {
                    artigo.AmostrasEtapa2 = quantidadeAmostras3;
                }
                else
                {
                    artigo.AmostrasEtapa2 = parametros.MaxValueHomogeneousSample;
                }
                return artigo;
            } ).ToArray();

            return artigosOrdenados;
        }

        public MetaArtigoOrdenado[] CalcularPercentualQuantidadeAcumulada( MetaArtigoOrdenado[] artigosOrdenados )
        {
            //    -- Quantidades
            //    select  
            //        ORD_ART.IDFamily,
            //        ORD_ART.ReferenceArtigo,
            //        ORD_ART.OrderPercQtAcumulada,
            //        sum(ORD_ART_AUX.PercQtVendidasArtigo) as PercQtAcumulada
            //    from #MET_ORDERED_ARTICLES as ORD_ART
            //    inner join #MET_ORDERED_ARTICLES as ORD_ART_AUX on ORD_ART_AUX.IDFamily = ORD_ART.IDFamily 
            //    where ORD_ART_AUX.OrderPercQtAcumulada <= ORD_ART.OrderPercQtAcumulada
            //    group by ORD_ART.IDFamily, ORD_ART.OrderPercQtAcumulada, ORD_ART.ReferenceArtigo


            //    -- Update PercQtAcumulada
            //    update #MET_ORDERED_ARTICLES
            //    set #MET_ORDERED_ARTICLES.PercQtAcumulada = (select Q.PercQtAcumulada 
            //                                from #QUANTITIES as Q 
            //                                where Q.IDFamily = #MET_ORDERED_ARTICLES.IDFamily 
            //                                and Q.OrderPercQtAcumulada = #MET_ORDERED_ARTICLES.OrderPercQtAcumulada )    
            //    from #MET_ORDERED_ARTICLES ORD_ART
            artigosOrdenados = artigosOrdenados.Select( artigo =>
            {
                artigo.PercentualQuantidadeAcumulada = artigosOrdenados.Where( a => a.FamiliaId == artigo.FamiliaId
             && a.OrdemPercentualQuantidadeAcumulada <= artigo.OrdemPercentualQuantidadeAcumulada )
                .Sum( a => a.PercentualVendidasArtigo );
                return artigo;
            } ).ToArray();
            return artigosOrdenados;
        }

        public MetaArtigoOrdenado[] ObterArtigosOrdenados( MetaArtigo[] artigos, MetaTotaisFamilia[] totaisFamilia )
        {
            //#MET_ORDERED_ARTICLES
            //-- Calcular quais artigos vão pesar
            //    select
            //        ET.IDFamily,
            //        ET.FamilyOrder,
            //        ET.FamilyDescription,
            //        ET.IDVariability,
            //        ET.Reference as ReferenceArtigo,
            //        ET.Quantity as QtVendidasArtigo,
            //        FT.NumArtigosFamilia,
            //        FT.QtVendidasFamilia,
            //        FT.PercQtVendidasFamilia,
            //        (convert(float, ET.Quantity) / convert(float, FT.QtVendidasFamilia)) * 100 as PercQtVendidasArtigo,
            //        case    
            //            when RANK() OVER 
            //                (partition by ET.IDFamily order by ((convert(float, ET.Quantity) / convert(float, FT.QtVendidasFamilia)) * 100) desc, ET.Reference) = 1
            //            then 0
            //            else (convert(float, ET.Quantity) / convert(float, FT.QtVendidasFamilia)) * 100
            //        end as PercQtAcumulada,
            //        RANK() OVER 
            //                (partition by ET.IDFamily order by ((convert(float, ET.Quantity) / convert(float, FT.QtVendidasFamilia)) * 100) desc, ET.Reference) as OrderPercQtAcumulada,
            //        case when MMFV.Token = 'MUITO_HOMOGENEO' then 1 else 0 end as IsHomogeneo,
            //        FT.NumAmostrasFamEtapa1 as NumAmostrasFam1Etapa,
            //        FT.TemRepresentacao as TemRepresentacao1Etapa,
            //        0 as NumAmostrasFam2Etapa
            //    from #ExcelTable as ET
            //    inner join #FamilyTotals as FT on FT.IDFamily = ET.IDFamily
            //    inner join MET_MATERIAL_FAMILY_VARIABILITY as MMFV on MMFV.ID = ET.IDVariability

            var artigosRankeados = artigos.Select( a => new MetaArtigoRank( a ) ).ToArray();

            artigosRankeados = artigosRankeados
                .Select( a =>
                {
                    var totalFamilia = totaisFamilia.Single( t => t.FamiliaId == a.FamiliaId );
                    a.FamiliaOrdem = totalFamilia.Ordem;
                    a.FamiliaDescricao = totalFamilia.Descricao;
                    a.FamiliaVariabilidadeId = totalFamilia.VariabilidadeId;
                    a.FamiliaQuantidadeArtigos = totalFamilia.QuantidadeArtigos;
                    a.FamiliaQuantidadeVendidas = totalFamilia.QuantidadeVendidas;
                    a.FamiliaVariabilidadeToken = totalFamilia.Familia.Variabilidade.Token;
                    a.FamiliaAmostrasEtapa1 = totalFamilia.AmostrasEtapa1;
                    a.FamiliaPossuiRepresentacao = totalFamilia.PossuiRepresentacao;
                    a.PercentualQuantidadeVendidas = Convert.ToDecimal( a.Quantidade ) / Convert.ToDecimal( totalFamilia.QuantidadeVendidas ) * 100;
                    a.PercentualQuantidadeAcumulada = 0.00m;
                    return a;
                } )
                .GroupBy( a => a.FamiliaId )
                .SelectMany( g => g.OrderByDescending( a => a.PercentualQuantidadeVendidas )
                            .ThenBy( a => a.Referencia )
                            .Select( ( a, index ) =>
                            {
                                a.Rank = index + 1;
                                return a;
                            } ) ).ToArray();

            var artigosOrdenados = artigosRankeados.Select( artigo =>
            {
                return new MetaArtigoOrdenado(
                    artigo.Id,
                    artigo.FamiliaId,
                    artigo.FamiliaOrdem,
                    artigo.FamiliaDescricao,
                    artigo.FamiliaVariabilidadeId,
                    artigo.Referencia,
                    artigo.Quantidade,
                    artigo.FamiliaQuantidadeArtigos,
                    artigo.FamiliaQuantidadeVendidas,
                    artigo.PercentualQuantidadeVendidas,
                    artigo.PercentualQuantidadeAcumulada,
                    artigo.Rank,
                    artigo.FamiliaVariabilidadeToken == MetaVariabilidadeFamiliaToken.MUITO_HOMOGENEO.ToString(),
                    artigo.FamiliaAmostrasEtapa1,
                    artigo.FamiliaPossuiRepresentacao,
                    0
                );
            } ).ToArray();

            return artigosOrdenados;
        }

        public MetaTotaisFamilia[] ObterTotaisFamilia( MetaParametrosCalculoAmostraDistribuicao parametros, MetaArtigo[] artigos )
        {
            var percentualUmaAmostra = 1 / ( parametros.MaximumNumberOfSamples * 0.01m );

            var quantidadeTotalVendidas = SomarTotalQuantidadesVendidas( artigos );
            //--Obter os Totais
            //--#FamilyTotals (insere o resultado da consulta abaixo na tabela temporária)
            //with Totais as (
            //    select
            //        IDFamily,
            //        FamilyOrder,
            //        FamilyToken,
            //        FamilyDescription,
            //        IDVariability,
            //        count( Reference ) as NumArtigosFamilia,
            //        sum( Quantity ) as QtVendidasFamilia,
            //        ( sum( Quantity ) / QtTotalVendidas ) * 100 as PercQtVendidasFamilia
            //    from #ExcelTable
            //    group by
            //        IDFamily, 
            //        FamilyOrder,
            //        FamilyToken,
            //        FamilyDescription,
            //        IDVariability
            //    ),
            var totais = artigos.GroupBy( artigo => (
                artigo.Familia.Id,
                artigo.Familia.Ordem,
                artigo.Familia.Token,
                artigo.Familia.Descricao,
                artigo.Familia.VariabilidadeId
            ) ).Select( grupo => new MetaTotaisFamilia(
                    grupo.Key.Id,
                    grupo.Key.Ordem,
                    grupo.Key.Token,
                    grupo.Key.Descricao,
                    grupo.Key.VariabilidadeId,
                    grupo.Count(),
                    grupo.Sum( artigo => artigo.Quantidade ),
                    ( grupo.Sum( artigo => artigo.Quantidade ) / quantidadeTotalVendidas ) * 100,
                    0,
                    0,
                    false,
                    grupo.First().Familia.Grupos,
                    grupo.First().Familia
                ) ).ToArray();

            //    select
            //        T.*,
            //        case 
            //            when PercQtVendidasFamilia<MMF.Limit then 0
            //            else abs( PercQtVendidasFamilia / PercUmaAmostra )
            //        end as NumAmostrasFamEtapa1,
            //        0 as NumAmostrasFamEtapa2,
            //        case
            //            when abs( PercQtVendidasFamilia/ PercUmaAmostra) < 1 then 0
            //            when PercQtVendidasFamilia<MMF.Limit then 0
            //            else 1
            //        end as TemRepresentacao
            //    from Totals as T
            //    inner join MET_MATERIAL_FAMILY as MMF on MMF.ID = IDFamily
            return totais.Select( total =>
            {
                total.AmostrasEtapa1 = total.PercentualVendidasFamilia < Convert.ToDecimal( total.Familia.Limite ) ? 0 : Convert.ToInt32( Math.Floor( Math.Abs( total.PercentualVendidasFamilia / percentualUmaAmostra ) ) );
                if ( Math.Abs( total.PercentualVendidasFamilia / percentualUmaAmostra ) < 1 ) total.PossuiRepresentacao = false;
                else if ( total.PercentualVendidasFamilia < Convert.ToDecimal( total.Familia.Limite ) ) total.PossuiRepresentacao = false;
                else total.PossuiRepresentacao = true;
                return total;
            } ).ToArray();
        }

        public decimal SomarTotalQuantidadesVendidas( MetaArtigo[] artigos )
        {
            return artigos.Sum( a => a.Quantidade );
        }

        public MetaArtigo[] ObterArtigos()
        {
            // Passo 1
            //  --calcular as amostras com base nas quantidades vendidas
            //--#ExcelTable  (insere o resultado da consulta abaixo na tabela temporária)
            //	select
            //        MMF.ID as IDFamily,
            //        MMF."Order" as FamilyOrder,
            //        MMF.IDVariability,
            //        MUA.GroupNumber,
            //        MMF.Token as FamilyToken,
            //        MMF.Description as FamilyDescription,
            //        MUA.Username,
            //        MUA.Year,
            //        MUA.Reference,
            //        MUA.Quantity
            //    from META.MET_USER_ARTICLE as MUA
            //    inner join META.MET_MATERIAL_FAMILY as MMF on MMF.ID = MUA.IDFamily
            //    where MUA.IsPackaged = 1
            //    --order by MMF.ID, MUA.Quantity desc


            //var queryArtigos = _metaArtigoRepository.GetAll()
            //    .Include( artigo => artigo.Familia )
            //    .Include( artigo => artigo.Familia.Grupos )
            //    .Include( artigo => artigo.Familia.Variabilidade )
            //    .Where( artigo =>
            // artigo.CreatedBy.Equals( username )
            // && artigo.Ano.Equals( ano )
            // && artigo.Embalado
            // && artigo.FamiliaId.HasValue );
            var artigos = new MetaArtigo[] { };

            //return await queryArtigos.ToArrayAsync();
            var path = System.IO.Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
            using ( var db = new StreamReader( string.Format( "{0}\\meta_data.json", path ) ) )
            {
                // Get a collection (or create, if doesn't exist)
                artigos = JsonConvert.DeserializeObject<MetaArtigo[]>( db.ReadToEnd() );
            }
            ////generates a new file.
            //using ( var sw = new StreamWriter(@"C:\meta_data.json") )
            //{
            //    sw.Write( JsonConvert.SerializeObject( artigos ) );
            //}
            //Console.ReadKey();

            return artigos;
        }

        private static decimal CalcularAmostrasPrecisasParaMinimo( MetaParametrosCalculoAmostraDistribuicao parametros
            , MetaTotaisPorNumeroGrupo[] totaisPorNumeroGrupo
            , MetaAmostraSemMinimo[] amostrasSemMinimo
            , decimal somaAmostrasSemMinimo )
        {
            //    -- Variaveis Auxiliar para atribuir minimos
            //    declare @TableCount int = ( select count( ID ) from @SamplesWithoutMin)
            //    declare @AuxID int = 1
            //    declare @LimitReached int = 0;

            //            with LIMITS as
            //            (select tg.GroupNumber,
            //        TG.NumAmostrasFamEtapa5,
            //        S.SumAmostrasEtapa2,
            //        case when S.SumAmostrasEtapa2 < 10 then S.SumAmostrasEtapa2 else 10 end as Soma
            //        --@AmostrasPrecisasParaMinimos = ( count( ID ) * @MinimoAmostrasGrupo ) - sum( NumAmostrasFamEtapa5 )
            //    from #TotalsByGroupNumber as TG
            //    inner join @SamplesWithoutMin as S on S.GroupNumber = TG.GroupNumber
            //    where TG.NumAmostrasFamEtapa5 < @MinimoAmostrasGrupo)
            var limites = totaisPorNumeroGrupo.Join( amostrasSemMinimo
                , outer => outer.NumeroGrupo
                , inner => inner.NumeroGrupo
                , ( outer, inner ) => (
                outer.NumeroGrupo
                , outer.AmostrasEtapa5
                , inner.SomaAmostrasEtapa2
                , Soma: ( inner.SomaAmostrasEtapa2 < 10 ? inner.SomaAmostrasEtapa2 : 10 )) )
                .Where( w => w.AmostrasEtapa5 < parametros.MinimumNumberOfSamplesPerGroup )
                .ToArray();

            //select @AmostrasPrecisasParaMinimos = sum( Soma ) - @SumAmostrasSemMinimo
            //from LIMITS
            return limites.Sum( l => l.Soma ) - somaAmostrasSemMinimo;
        }

        private static MetaAmostraSemMinimo[] ObterAmostrasSemMinimo( MetaParametrosCalculoAmostraDistribuicao parametros, MetaArtigoOrdenado[] artigosOrdenados, MetaTotaisFamilia2[] totaisFamilia2, MetaTotaisPorNumeroGrupo[] totaisPorNumeroGrupo )
        {
            //    --@Etapa2AndEtapa5Results
            //    select
            //        ET5.IDFamily,
            //        ET5.FamilyToken,
            //        ET5.GroupNumber,
            //        ORD_ET2.NumAmostrasFam2Etapa
            //    from #MET_FAMILY_TOTALS as ET5
            //    inner join #MET_ORDERED_ARTICLES  as ORD_ET2 on ORD_ET2.IDFamily = ET5.IDFamily
            //    group by ET5.IDFamily, ET5.FamilyToken, ET5.GroupNumber, ORD_ET2.NumAmostrasFam2Etapa
            var resultadosEtapa2e5 = totaisFamilia2.Join( artigosOrdenados
                , outer => outer.FamiliaId
                , inner => inner.FamiliaId
                , ( outer, inner ) => (
                    outer.FamiliaId,
                    outer.Token,
                    outer.NumeroGrupo,
                    inner.AmostrasEtapa2
                ) ).GroupBy( g => (
                    g.FamiliaId,
                    g.Token,
                    g.NumeroGrupo,
                    g.AmostrasEtapa2
                ) ).Select( s => (
                      s.Key.FamiliaId,
                      s.Key.Token,
                      s.Key.NumeroGrupo,
                      s.Key.AmostrasEtapa2
                  ) ).ToArray();

            //    --@SamplesWithoutMin
            //    select
            //        T.GroupNumber,
            //        T.NumAmostrasFamEtapa5,
            //        sum( RES.NumAmostrasFam2Etapa ) as SumAmostrasEtapa2
            //    from #TotalsByGroupNumber as T
            //    inner join @Etapa2AndEtapa5Results as RES on RES.GroupNumber = T.GroupNumber
            //    where NumAmostrasFamEtapa5 < @MinimoAmostrasGrupo
            //    group by T.GroupNumber, T.NumAmostrasFamEtapa5
            var amostrasSemMinimo = totaisPorNumeroGrupo.Join( resultadosEtapa2e5
                , outer => outer.NumeroGrupo
                , inner => inner.NumeroGrupo
                , ( outer, inner ) => (outer.NumeroGrupo, outer.AmostrasEtapa5, inner.AmostrasEtapa2) )
                .Where( w => w.AmostrasEtapa5 < parametros.MinimumNumberOfSamplesPerGroup )
                .GroupBy( g => (g.NumeroGrupo, g.AmostrasEtapa5) )
                .Select( s => new MetaAmostraSemMinimo( s.Key.NumeroGrupo, s.Key.AmostrasEtapa5, s.Sum( ss => ss.AmostrasEtapa2 ) ) )
                .ToArray();

            return amostrasSemMinimo;
        }

        private static decimal ObterSomaAmostrasSemMinimo( MetaParametrosCalculoAmostraDistribuicao parametros, MetaTotaisPorNumeroGrupo[] totaisPorNumeroGrupo )
        {
            //    declare @GruposSemMinimo int
            //    declare @SumAmostrasSemMinimo int
            //    declare @AmostrasPrecisasParaMinimos int
            //    --Get minimum values information
            //   select
            //        @GruposSemMinimo = count( ID ),
            //        @SumAmostrasSemMinimo = sum( NumAmostrasFamEtapa5 )
            //    from #TotalsByGroupNumber
            //    where NumAmostrasFamEtapa5 < @MinimoAmostrasGrupo
            return totaisPorNumeroGrupo.Where( t => t.AmostrasEtapa5 < parametros.MinimumNumberOfSamplesPerGroup ).Sum( t => t.AmostrasEtapa5 );
        }

        private static decimal ObterTotalAmostrasPorDistribuir( MetaParametrosCalculoAmostraDistribuicao parametros, MetaTotaisPorNumeroGrupo[] totaisPorNumeroGrupo )
        {
            //    set @AmostrasDistribuidas = ( select sum( NumAmostrasFamEtapa5 ) from #TotalsByGroupNumber)
            var amostrasDistribuidas = ObterTotalAmostrasDistribuidas( totaisPorNumeroGrupo );

            //    declare @AmostrasPorDistribuir int = @TotalAmostras - @AmostrasDistribuidas
            return parametros.MaximumNumberOfSamples - amostrasDistribuidas;
        }

        private static decimal ObterTotalAmostrasDistribuidas( MetaTotaisPorNumeroGrupo[] totaisPorNumeroGrupo )
        {
            //    -- Find out if all samples have been distributed
            //    declare @AmostrasDistribuidas int = ( select sum( NumAmostrasFamEtapa5 ) from #TotalsByGroupNumber)
            return totaisPorNumeroGrupo.Sum( total => total.AmostrasEtapa5 );
        }
    }
}
