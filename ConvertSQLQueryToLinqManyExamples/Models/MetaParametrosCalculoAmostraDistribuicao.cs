namespace ConvertSQLQueryToLinqManyExamples.Models
{
    public class MetaParametrosCalculoAmostraDistribuicao
    {
        public readonly int MaximumNumberOfSamples;
        public readonly int MinimumNumberOfSamplesPerGroup;
        public readonly int NumberOfSamplesPerGroup;
        public readonly decimal PercentageOfSampleAllocationPerSubGroup;
        public readonly decimal PercentageOfArticleDivisionPerSubGroup;

        public readonly decimal NonHomogeneousPercentage;
        public readonly decimal MaxHomogeneousPercentage;
        public readonly decimal MinHomogeneousPercentage;
        public readonly int MaxValueHomogeneousSample;
        public readonly int MinValueHomogeneousSample;

        public readonly decimal FirstClassPercentage;
        public readonly decimal SecondClassPercentage;
        public readonly decimal ThirdClassPercentage;

        public MetaParametrosCalculoAmostraDistribuicao()
        {
            MaximumNumberOfSamples = 300;
            MinimumNumberOfSamplesPerGroup = 10;
            NumberOfSamplesPerGroup = 15;
            PercentageOfSampleAllocationPerSubGroup = 60.0m;
            PercentageOfArticleDivisionPerSubGroup = 40.0m;

            NonHomogeneousPercentage = 95;
            MaxHomogeneousPercentage = 95;
            MinHomogeneousPercentage = 75;
            MaxValueHomogeneousSample = 10;
            MinValueHomogeneousSample = 5;

            FirstClassPercentage = 50;
            SecondClassPercentage = 75;
            ThirdClassPercentage = 95;
        }
    }
}
