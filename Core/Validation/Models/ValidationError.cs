namespace Core.Validation.Models;

public record ValidationError(string PropertyName, string Message);