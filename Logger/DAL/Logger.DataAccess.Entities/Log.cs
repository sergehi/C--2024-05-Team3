namespace Logger.DataAccess.Entities
{
    public class Log
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public long Id { get; set; } = 0;

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Дата и время действия.
        /// </summary>
        public DateTime Time { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Действие над объектом.
        /// </summary>
        public ELogAction Action { get; set; } = ELogAction.LA_None;

        /// <summary>
        /// Идентификатор сущности.
        /// </summary>
        public Guid EntityId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Первичный ключ.
        /// </summary>
        public Guid EntityPrimaryKey { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Сущность.
        /// </summary>
        public string Entity { get; set; } = string.Empty;
    }
}
