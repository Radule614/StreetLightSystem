using Ardalis.Specification;
using Pole.API.Domain.Entities;

namespace Pole.API.Domain.Specifications;

/// <summary>
/// Pole specification class used for get pole queries
/// </summary>
public sealed class PoleSpecification : Specification<PoleEntity>
{
    public PoleSpecification() { }

    public PoleSpecification(Guid id)
    {
        Query.Where(pole => pole.Id.Equals(id));
    }
}

