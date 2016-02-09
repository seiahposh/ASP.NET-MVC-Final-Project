﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Planex.Data.Models
{
    public class Skill
    {
        private ICollection<User> users;
        private ICollection<Subtask> subTasks;

        public Skill()
        {
            this.users = new HashSet<User>();
            this.subTasks = new HashSet<Subtask>();
        }
               
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }

        public virtual ICollection<User> Users
        {
            get { return this.users; }
            set { this.users = value; }
        }

        public virtual ICollection<Subtask> Subtasks
        {
            get { return this.subTasks; }
            set { this.subTasks = value; }
        }
    }
}