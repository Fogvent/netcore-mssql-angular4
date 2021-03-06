﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class UserInterest: BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid TopicId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("TopicId")]
        public virtual Topic Topic { get; set; }
    }
}
