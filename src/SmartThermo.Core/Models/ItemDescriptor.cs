namespace SmartThermo.Core.Models
{
    /// <summary>
    /// Класс, описывающий элементы для ComBox.
    /// </summary>
    /// <typeparam name="T">Тип переменной, хранящий значение.</typeparam>
    public sealed class ItemDescriptor<T>
    {
        public int Id { get; }
        public string Name { get; }
        public T Value { get; set; }
        public string Group { get; }

        public ItemDescriptor(string name, T value, int id = 0, string group = null)
        {
            Id = id;
            Name = name;
            Value = value;
            Group = group;
        }
    }
}
