using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartEntity;

namespace Entity
{
    [Table("student_choice")]
    public class Choice:BaseEntity<Choice>
    {
        [PrimaryKey("stud_id",PrimaryKeyType.Reference)]
        public Student Student { get; set; }

        [PrimaryKey("choice_num",PrimaryKeyType.NotAutoIncrease)]
        public int Num { get; set; }

        [Column("choice_answer")]
        public int Answer { get; set; }
    }
}
