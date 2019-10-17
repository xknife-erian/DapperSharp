using Overt.User.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overt.User.Application.Constracts
{
    public interface IUserService
    {
        UserEntity DoSomething();
        bool AddAsync();

        List<UserEntity> GetList();


        List<UserEntity> GetByIds();
    }
}
