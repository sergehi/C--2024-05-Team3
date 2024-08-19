using Logger.Domain.Entities;

namespace Logger.Services.Contracts.Log
{
    public class LogDto
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
        public object Entity { get; set; } = new object();
    }
}
