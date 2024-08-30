using Logger.DataAccess.Entities;

namespace Logger.BusinessLogic.DTO.Log
{
    public class CreateLogDTO
    {
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
        public Guid EntityType { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Первичный ключ.
        /// </summary>
        public Guid EntityPK { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Сущность.
        /// </summary>
        public string Entity { get; set; } = string.Empty;
    }
}
