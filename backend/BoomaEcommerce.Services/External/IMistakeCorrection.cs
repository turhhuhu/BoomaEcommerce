namespace BoomaEcommerce.Services.External
{
    public interface IMistakeCorrection
    {
        public string CorrectMistakeIfThereIsAny(string textToCorrect);
    }
}