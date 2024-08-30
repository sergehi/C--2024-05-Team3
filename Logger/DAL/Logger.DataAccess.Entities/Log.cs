using Common.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logger.DataAccess.Entities
{
    public class Log : IEntity<long>
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        [Key]
        public long Id { get; set; } = 0;

        /// <summary>
        /// Дата и время действия.
        /// </summary>
        [Required]
        public long Time { get; set; } = 0;

        /// <summary>
        /// Действие над объектом.
        /// </summary>
        [Required]
        public ELogAction Action { get; set; } = ELogAction.LA_None;

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        [Required]
        public Guid UserId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Идентификатор сущности.
        /// </summary>
        [Required]
        public Guid EntityType { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Первичный ключ.
        /// </summary>
        [Required]
        public Guid EntityPK { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Сущность.
        /// </summary>
        [Required]
        public string Entity { get; set; } = string.Empty;
    }
}
