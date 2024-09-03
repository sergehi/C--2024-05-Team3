using Logger.DataAccess.Entities;

namespace Logger.BusinessLogic.DTO.Log
{
    public class FilterLogDTO
    {
        /// <summary>
        /// Начало диапазона даты и времени действий.
        /// </summary>
        public DateTime BeginTime { get; set; } = DateTime.MinValue;
        
        /// <summary>
        /// Конец диапазона даты и времени действий.
        /// </summary>
        public DateTime EndTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Действие над объектом.
        /// </summary>
        public ELogAction Action { get; set; } = ELogAction.LA_None;

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Идентификатор сущности.
        /// </summary>
        public Guid EntityType { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Первичный ключ.
        /// </summary>
        public Guid EntityPK { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Номер страницы.
        /// </summary>
        public int Page { get; set; } = 0;

        /// <summary>
        /// Количество элементов на странице.
        /// </summary>
        public int ItemsPerPage { get; set; } = 0;
    }
}
