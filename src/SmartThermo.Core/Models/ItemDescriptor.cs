namespace SmartThermo.Core.Models
{
    /// <summary>
    /// Класс, описывающий элементы для ComBox.
    /// </summary>
    /// <typeparam name="T">Тип переменной, хранящий значение.</typeparam>
    public sealed class ItemDescriptor<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDescriptor{T}"/> class.
        /// </summary>
        /// <param name="name">Имя элемента.</param>
        /// <param name="value">Значение элемента.</param>
        /// <param name="id">Id элемента. По умолчанию равно 0.</param>
        /// <param name="group">Группа, к которой относится элемент.</param>
        public ItemDescriptor(string name, T value, int id = 0, string group = null)
        {
            Id = id;
            Name = name;
            Value = value;
            Group = group;
        }

        /// <summary>
        /// Id.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Имя.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Значение.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Группа, к которой относится элемент.
        /// </summary>
        public string Group { get; }
    }
}
