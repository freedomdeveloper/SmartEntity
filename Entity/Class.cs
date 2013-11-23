using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartEntity;

namespace Entity
{
    [Table("class")]
    public class Class:BaseEntity<Class>
    {
        [PrimaryKey("class_id", PrimaryKeyType.NotAutoIncrease)]
        public int Id { get; set; }

        [Column("class_name")]
        public string Name { get; set; }
    }
}
