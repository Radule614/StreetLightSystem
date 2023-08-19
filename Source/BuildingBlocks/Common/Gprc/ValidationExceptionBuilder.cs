using System.Text;
using Common.Gprc.Exceptions;
using FluentValidation.Results;

namespace Common.Gprc;

/// <summary>
/// Class meant to build validation exception with multiple error messages;
/// </summary>
public class ValidationExceptionBuilder
{
    /// <summary>
    /// Dictionary containing key value pairs in a form of property names and error messages.
    /// </summary>
    private readonly Dictionary<string, string> _errors = new();

    /// <summary>
    /// Exposed method for adding an error to the error dictionary.
    /// </summary>
    /// <param name="propertyName">Name of the property that is not valid.</param>
    /// <param name="error">Error message.</param>
    public void AddError(string propertyName, string error)
    {
        _errors.Add(propertyName, error);
    }

    /// <summary>
    /// Method for adding a range of error messages derived from fluent validation.
    /// This method will only use key value in the dictionary because the messages are already well formed.
    /// </summary>
    /// <param name="errors">List of fluent validator failures.</param>
    public void AddFluentErrors(List<ValidationFailure> errors)
    {
        foreach (var failure in errors)
        {
            AddError(failure.ErrorMessage, "");
        }
    }

    /// <summary>
    /// Method for checking if any errors had occurred.
    /// </summary>
    /// <returns>Boolean value indicating if there are any errors.</returns>
    public bool HasErrors()
    {
        return _errors.Count != 0;
    }

    /// <summary>
    /// Method for building the ValidationException from error dictionary.
    /// </summary>
    /// <returns>ValidationException</returns>
    public ValidationException Build()
    {
        var messageBuilder = new StringBuilder();
        foreach (var pair in _errors)
        {
            if (pair.Value != "")
            {
                messageBuilder.Append($"{pair.Key}: {pair.Value};");
            }
            else
            {
                messageBuilder.Append($"{pair.Key};");
            }
        }
        if (_errors.Count > 0)
        {
            messageBuilder.Remove(messageBuilder.Length - 1, 1);
        }
        return new ValidationException(messageBuilder.ToString());
    }
}
