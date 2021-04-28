using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcFind;
using GrpcText;
using Task.Api.Infrastructure.Models;
using Task.Api.Infrastructure.Repositories;

namespace GrpcTask
{
    public class TaskService : Task.TaskBase
    {
        private readonly Text.TextClient _textClient;

        private readonly Find.FindClient _findClient;

        private readonly ITaskFileRepository _taskFileRepository;


        public TaskService(Text.TextClient textClient, Find.FindClient findClient, ITaskFileRepository taskFileRepository)
        {
            _textClient = textClient;
            _findClient = findClient;
            _taskFileRepository = taskFileRepository;
        }

        public override Task<TaskResponse> PutTask(TaskRequest request, ServerCallContext context)
        {
            if (request.Begin.ToDateTime() < DateTime.Now) throw new Exception("Дата начала постановки задачи меньше текущей даты");
            if (request.End.ToDateTime() < DateTime.Now) throw new Exception("Дата окончания постановки задачи меньше текущей даты");
            if (request.Begin.ToDateTime() >= request.End.ToDateTime()) throw new Exception("Дата начала постановки задачи больше или равно дате окончания");
            System.Threading.Tasks.Task.Run(() => TaskSearchWords(request));
            return System.Threading.Tasks.Task.FromResult(new TaskResponse {Result = true});
        }

        private async System.Threading.Tasks.Task TaskSearchWords(TaskRequest request)
        {
            var taskId = Guid.NewGuid();
            var processedFiles = new List<string>();
            while (true)
            {
                if (DateTime.Now < request.Begin.ToDateTime()) continue;
                if (request.End.ToDateTime() <= DateTime.Now) break;
                var findWords = new List<string>();
                var textsResponse = await _textClient.GetTextsAllAsync(new TextAllRequest());
                var searchRequest = new SearchRequest { Words = { request.Words } };
                foreach (var text in textsResponse.Items.Where(i => !processedFiles.Contains(i.Id)))
                {
                    searchRequest.TextId = text.Id;
                    var findWordsResponse = await _findClient.SearchWordsAsync(searchRequest);
                    findWords.AddRange(findWordsResponse.FoundWords);
                    await _taskFileRepository.InsertAsync(new TaskFile
                    {
                        TextId = Guid.Parse(text.Id), TaskId = taskId,
                        FoundWords = string.Join(',', findWordsResponse.FoundWords)
                    });
                    processedFiles.Add(text.Id);
                }
                await System.Threading.Tasks.Task.Delay(request.Period.ToTimeSpan());
            }
        }

        public override async Task<TaskResultsResponse> GetResultsByTaskId(TaskResultsRequest request, ServerCallContext context)
        {
            var response = new TaskResultsResponse();
            var tasks = await _taskFileRepository.GetByTaskIdAsync(request.TaskId);
            foreach (var task in tasks)
            {
                response.Items.Add(new TaskResultResponse{TextId = task.TextId.ToString(), Words = { task.FoundWords.Split(',')}});
            }

            return response;
        }
    }
}
