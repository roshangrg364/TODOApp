﻿using DomainModule.Entity;
using DomainModule.RepositoryInterface;
using InfrastructureModule.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureModule.Repository
{
    public class EmailMessageRepository:BaseRepository<EmailMessage>, IEmailMessageRepository
    {
        public EmailMessageRepository(AppDbContext context):base(context)
        {
            
        }
    }
}
