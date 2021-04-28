using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Text.Api.Infrastructure.Models
{
    public class TextFile
    {
        public TextFile()
        {
            Id = Guid.NewGuid();        
        }

        public Guid Id { get; private set; }

        [Column(TypeName = "varchar(MAX)")]
        public string Body { get; set; }


    }
}
