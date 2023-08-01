using DomainModule.Entity;
using DomainModule.RepositoryInterface;
using InfrastructureModule.Context;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureModule.Repository
{
    public class SharedTodoRepository:BaseRepository<SharedTodoEntity>,SharedTodoRepositoryInterface
    {
        public SharedTodoRepository( AppDbContext context):base(context)
        {
            
        }
    }
}
