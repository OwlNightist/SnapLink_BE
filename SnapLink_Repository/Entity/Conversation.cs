using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Conversation
{
    public int ConversationId { get; set; }

    public string? Title { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Status { get; set; } // Active, Archived, Deleted

    public string? Type { get; set; } // Direct, Group

    // Navigation properties
    public virtual ICollection<Messagess> Messages { get; set; } = new List<Messagess>();

    public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
} 