﻿namespace Planex.Web.Areas.Manager.Models.Gantt
{
    public class ProjectDetailsAssignmentsViewModel
    {
        public int AssignmentId { get; set; }

        public int TaskId { get; set; }

        public int ResourceId { get; set; }

        public decimal Units { get; set; }
    }
}