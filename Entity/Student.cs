using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entity;
using SmartEntity;

namespace Entity
{
    [Table("student")]
    public class Student:BaseEntity<Student>
    {
        [PrimaryKey("stud_id")]
        public int Id { get; set; }

        [Column("stud_name")]
        public string Name { get; set; }

        [ForeignKey("class_id")]
        public Class Class { get; set; }
    }
}
