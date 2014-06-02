using System.Speech.Synthesis;

namespace VPrinting
{
    public class Speeker
    {
        private static readonly SpeechSynthesizer ms_agent = new SpeechSynthesizer();

        public static volatile bool Enabled = true;

        public Speeker()
        {
        }

        public static string SpeakAsynchSf(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || !Enabled)
                return text;

            ms_agent.SpeakAsync(text);

            return text;
        }
    }
}
