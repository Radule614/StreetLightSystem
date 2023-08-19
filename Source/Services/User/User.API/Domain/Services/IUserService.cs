using Common.Saga.User.Dto;
using User.API.Domain.Entities;

namespace User.API.Domain.Services;

public interface IUserService : IDisposable
{
    /// <summary>
    /// Method for creating an user entity in the database. It has all the necessary validation.
    /// </summary>
    /// <param name="firstName">User's first name</param>
    /// <param name="lastName">User's last name</param>
    /// <param name="email">User's email</param>
    /// <param name="password">Password that's to be hashed</param>
    /// <returns>Created user entity</returns>
    Task<UserEntity> Create(string firstName, string lastName, string email, string password);
    /// <summary>
    /// Method for updating existing user entity.
    /// </summary>
    /// <param name="userId">Id of the user that's to be updated</param>
    /// <param name="firstName">New first name</param>
    /// <param name="lastName">New last name</param>
    /// <param name="email">New email</param>
    /// <param name="password">New password</param>
    /// <returns>Updated user entity</returns>
    Task<UserEntity> Update(Guid userId, string firstName, string lastName, string? email, string? password);

    void StartCreateUserSaga(UserData data, Guid userId);
    void StartDeleteUserSaga(Guid userToDeleteId, Guid loggedUserId);
    void StartUpdateUserSaga(UserData data, Guid userId);
}
