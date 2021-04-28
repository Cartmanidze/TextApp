using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcText;

namespace GrpcFind
{
    public class FindService : Find.FindBase
    {
        private readonly Text.TextClient _textClient;

        public FindService(Text.TextClient textClient)
        {
            _textClient = textClient;
        }

        public override async Task<SearchResponse> SearchWords(SearchRequest request, ServerCallContext context)
        {
            var response = new SearchResponse();
            var textRequest = new TextRequest{Id = request.TextId};
            var textResponse = await _textClient.GetTextByIdAsync(textRequest);
            var text = textResponse.Body;
            var wordsForSearch = request.Words;
            foreach (var wordForSearch in wordsForSearch)
            {
                if (Regex.IsMatch(text, $"\\b{wordForSearch}\\b"))
                {
                    response.FoundWords.Add(wordForSearch);
                }
            }
            return response;
        }
    }
}
