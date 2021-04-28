using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task.Api.Infrastructure.Models
{
    public class TaskFile
    {
        public TaskFile()
        {
            Id = Guid.NewGuid();        
        }

        public Guid Id { get; private set; }

        public Guid TaskId { get; set; }

        public Guid TextId { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string FoundWords { get; set; }


    }
}
