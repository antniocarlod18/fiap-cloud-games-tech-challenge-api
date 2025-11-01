using FiapCloudGamesTechChallenge.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace FiapCloudGamesTechChallenge.Domain.Entities;

public class AuditGameUserCollection : EntityBase
{
    public required User User { get; set; }
    public required Game Game { get; set; }
    public required AuditGameUserActionEnum Action { get; set; }
    public required AuditGameUserCollectionEnum Collection { get; set; }
    public string? Notes { get; set; }

    [SetsRequiredMembers]
    public AuditGameUserCollection(User user, Game game, AuditGameUserActionEnum action, AuditGameUserCollectionEnum collection, string? notes = null) : base()
    {
        User = user;
        Game = game;
        Action = action;
        Collection = collection;
        Notes = notes;
    }

    [SetsRequiredMembers]
    protected AuditGameUserCollection()
    {
    }
}
