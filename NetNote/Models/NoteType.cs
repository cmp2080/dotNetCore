using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetNote.Models
{
    /// <summary>
    /// 笔记类型
    /// </summary>
    public class NoteType
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public List<Note> Notes { get; set; }
    }
}
