using System;
using System.Net;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Text.Api.Infrastructure.Models;
using Text.Api.Infrastructure.Repositories;

namespace GrpcText
{
    public class TextService : Text.TextBase
    {
        private readonly ITextFileRepository _textFileRepository;

        private readonly ILogger<TextService> _logger;

        public TextService(ITextFileRepository textFileRepository, ILogger<TextService> logger)
        {
            _textFileRepository = textFileRepository;
            _logger = logger;
        }

        public override async Task<TextItemResponse> GetTextById(TextRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Begin grpc call from method {Method} for text id {Id}", context.Method, request.Id);
            var textFile = await _textFileRepository.GetByIdAsync(Guid.Parse(request.Id));
            if (textFile == null)
            {
                context.Status = new Status(StatusCode.NotFound, $"Text with id {request.Id} do not exist");
                return null;
            }
            context.Status = new Status(StatusCode.OK, $"Text with id {request.Id} do exist");
            return new TextItemResponse {Body = textFile.Body};
        }

        public override async Task<TextAllResponse> GetTextsAll(TextAllRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Begin grpc call from method {Method} for all texts", context.Method);
            var allTexts = await _textFileRepository.GetAllAsync();
            if (allTexts.Length > 0)
            {
                var response = new TextAllResponse();
                foreach (var text in allTexts)
                {
                    response.Items.Add(new TextItemResponse{Id = text.Id.ToString(), Body = text.Body});
                }

                return response;
            }
            return null;
        }

        public override async Task<SaveTextResponse> SaveTextAsBinary(SaveTextAsBinaryRequest request, ServerCallContext context)
        {
            var body = System.Text.Encoding.Default.GetString(request.File.ToByteArray());
            await _textFileRepository.InsertAsync(new TextFile {Body = body});
            return new SaveTextResponse {Result = true};
        }

        public override async Task<SaveTextResponse> SaveTextAsString(SaveTextAsStringRequest request, ServerCallContext context)
        {
            var body = request.Body;
            await _textFileRepository.InsertAsync(new TextFile { Body = body });
            return new SaveTextResponse {Result = true};
        }

        public override async Task<SaveTextResponse> SaveTextByUri(SaveTextByUriRequest request, ServerCallContext context)
        {
            var textFile = new TextFile();
            using (var client = new WebClient())
            {
                var body = await client.DownloadStringTaskAsync(new Uri(request.Uri));
                textFile.Body = body;
            }
            await _textFileRepository.InsertAsync(textFile);
            return new SaveTextResponse {Result = true};
        }
    }
}
