﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Dto
{
    public class TodoDto
    {
        public int Id { get;  set; }
        public string Title { get;  set; }
        public string Status { get; set; }
        public string PriorityLevel { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
        public string CreatedByUser { get; set; }
        public string? CompletedByUser { get; set; }

    }
    public class TodoResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PriorityLevel { get; set; }
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
    }
    public class TodoCreateDto
    {
        public TodoCreateDto(string title, int priorityLevel, DateTime dueDate, string userId)
        {
            Title = title;
            PriorityLevel = priorityLevel;
            DueDate = dueDate;
            UserId = userId;

        }
        public string Title { get; protected set; }
        public string UserId { get; protected set; }
        public int PriorityLevel { get; protected set; }
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
    }

    public class TodoEditDto {

        public TodoEditDto(int id,string title,int priorityLevel,DateTime dueDate)
        {
            Id = id;
            Title = title;
            PriorityLevel = priorityLevel;
            DueDate = dueDate;
        }
        public int Id { get; set; }
        public string Title { get; protected set; }
        public int PriorityLevel { get; protected set; }
        public DateTime DueDate { get; set; }
        public string? Description { get; set; }
    }
}