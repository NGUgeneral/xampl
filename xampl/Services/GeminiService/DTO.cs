#nullable disable
namespace xampl.Services.GeminiService
{
    public class GeminiApiResponse
    {
        public Candidate[] Candidates { get; set; }
        public UsageMetadata UsageMetadata { get; set; }
        public string ModelVersion { get; set; }
        public string ResponseId { get; set; }
    }

    public class Candidate
    {
        public Content Content { get; set; }
        public string FinishReason { get; set; }
        public double AvgLogprobs { get; set; }
    }

    public class Content
    {
        public Part[] Parts { get; set; }
        public string Role { get; set; }
    }

    public class Part
    {
        public string Text { get; set; }
    }

    public class UsageMetadata
    {
        public int PromptTokenCount { get; set; }
        public int CandidatesTokenCount { get; set; }
        public int TotalTokenCount { get; set; }
        public PromptTokensDetail[] PromptTokensDetails { get; set; }
        public CandidatesTokensDetail[] CandidatesTokensDetails { get; set; }
    }

    public class PromptTokensDetail
    {
        public string Modality { get; set; }
        public int TokenCount { get; set; }
    }

    public class CandidatesTokensDetail
    {
        public string Modality { get; set; }
        public int TokenCount { get; set; }
    }
}
