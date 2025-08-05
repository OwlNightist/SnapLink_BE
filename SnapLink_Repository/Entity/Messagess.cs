using System;
using System.Collections.Generic;

namespace SnapLink_Repository.Entity;

public partial class Messagess
{
    public int MessageId { get; set; }

    public int? SenderId { get; set; }

    public int? RecipientId { get; set; }

    public int? ConversationId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? MessageType { get; set; } // Text, Image, File, etc.

    public string? Status { get; set; } // "sent", "read"

    public DateTime? ReadAt { get; set; }

    // Navigation properties
    public virtual User? Recipient { get; set; }

    public virtual User? Sender { get; set; }

    public virtual Conversation? Conversation { get; set; }
}
