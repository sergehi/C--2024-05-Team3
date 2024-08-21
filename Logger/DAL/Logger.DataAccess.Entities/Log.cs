using Common.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Logger.DataAccess.Entities
{
    public class Log: IEntity<long>
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        [Key]
        public long Id { get; set; } = 0;

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        [Required]
        public Guid UserId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Дата и время действия.
        /// </summary>
        [Required]
        public DateTime Time { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Действие над объектом.
        /// </summary>
        [Required]
        public ELogAction Action { get; set; } = ELogAction.LA_None;

        /// <summary>
        /// Идентификатор сущности.
        /// </summary>
        [Required]
        public Guid EntityId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Первичный ключ.
        /// </summary>
        [Required]
        public Guid EntityPrimaryKey { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Сущность.
        /// </summary>
        [Required]
        public string Entity { get; set; } = string.Empty;
    }
}
