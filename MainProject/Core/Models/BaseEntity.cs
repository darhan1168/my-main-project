using System.ComponentModel.DataAnnotations;

namespace Core;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
}