using System;

namespace SmartEntity
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        private TableAttribute() { }

        public TableAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        private ColumnAttribute() { }

        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
        public string Name { get; set; }

        public PrimaryKeyType Type { get; set; }

        private PrimaryKeyAttribute() { }

        public PrimaryKeyAttribute(string name):this(name, PrimaryKeyType.AutoIncrease){ }

        public PrimaryKeyAttribute(string name, PrimaryKeyType type)
        {
            Name = name;
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : Attribute
    {
        public string Name { get; set; }

        private ForeignKeyAttribute() { }

        public ForeignKeyAttribute(string name)
        {
            Name = name;
        }
    }
}
