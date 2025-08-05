using System;

namespace SnapLink_Repository.Entity;

public partial class ConversationParticipant
{
    public int ConversationParticipantId { get; set; }

    public int ConversationId { get; set; }

    public int UserId { get; set; }

    public DateTime? JoinedAt { get; set; }

    public DateTime? LeftAt { get; set; }

    public string? Role { get; set; } // Admin, Member, etc.

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
} 