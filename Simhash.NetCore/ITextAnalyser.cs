
namespace Simhash.NetCore
{
    public interface ITextAnalyser
    {
        ulong GetSimilarityValue(string leftText, ulong rightHashVector);
        ulong GetSimilarityValue(string leftText, string rightText);
        ulong GetTextHashVector(string text);
    }
}