using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartEntity;

namespace Entity
{
    [Table("student_answer")]
    public class Answer:BaseEntity<Answer>
    {
        [PrimaryKey("stud_id",PrimaryKeyType.Reference)]
        public Student Student { get; set; }

        [Column("stud_email")]
        public string Email { get; set; }
    }
}
