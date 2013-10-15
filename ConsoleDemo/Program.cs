using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartEntity;

namespace ConsoleDemo
{
    [Table("class")]
    public class Class:BaseEntity<Class>
    {
        [PrimaryKey("class_id")]
        public int Id { get; set; }

        [Column("class_name")]
        public string Name { get; set; }
    }

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

    class Program
    {
        static void Main(string[] args)
        {
            //Class aClass = new Class { Name = "soft1" };
            //Class bClass = new Class { Name = "soft2" };
            //aClass.Add();
            //bClass.Add();

            Class dClass = new Class { Id = 2 };
            dClass.Retrieve();
            int count = dClass.Count();

            //Class cClass = new Class { Id = 1 };
            //cClass.Delete();

            //Student student1 = new Student
            //{
            //    Class = new Class { Id = 2 },
            //    Name = "demo1",
            //};
            //student1.Add();

            //Student student2 = new Student
            //{
            //    Id = 1,
            //    Class = new Class(),
            //};
            //student2.Retrieve();     
        }
    }
}
