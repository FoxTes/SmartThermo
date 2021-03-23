namespace SmartThermo.Core.Models
{
    /// <summary>
    /// Класс, описывающий элементы для ComBox.
    /// </summary>
    /// <typeparam name="T">Тип переменной, хранящий значение.</typeparam>
    public sealed class ItemDescriptor<T>
    {
        public string Name { get; }
        public T Value { get; }
        public string Group { get; }

        public ItemDescriptor(string name, T value, string group = null)
        {
            Name = name;
            Value = value;
            Group = group;
        }
    }
}
