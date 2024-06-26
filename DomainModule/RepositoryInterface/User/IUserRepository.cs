﻿using DomainModule.BaseRepo;
using DomainModule.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.RepositoryInterface
{
    public interface IUserRepository:IBaseRepository<User>
    {
        Task<User?> GetByMobile(string mobile);
        Task<User?> GetByEmail(string email);
        Task<User?> GetByIdString(string Id);
        Task<User?> GetByUserName(string name);

    }
}
