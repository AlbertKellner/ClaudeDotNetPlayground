using System.ComponentModel.DataAnnotations;

namespace DotNetPlayground.Api.Features.Command.TodoItemInsert.TodoItemInsertModels;

public sealed record TodoItemInsertInput(
    [Required, MinLength(1), MaxLength(200)] string Title,
    [MaxLength(1000)] string Description);
