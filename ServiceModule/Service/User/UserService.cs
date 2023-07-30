using DomainModule.Dto;
using DomainModule.Dto.User;
using DomainModule.Entity;
using DomainModule.Exceptions;
using DomainModule.RepositoryInterface;
using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace ServiceModule.Service
{
    public class UserService : UserServiceInterface
    {
        private readonly UserRepositoryInterface _userRepo;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserService(UserRepositoryInterface userRepo,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userRepo = userRepo;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task Activate(string id)
        {
            var user = await _userRepo.GetByIdString(id).ConfigureAwait(false) ?? throw new UserNotFoundException();
            user.Activate();
            await _userRepo.UpdateAsync(user).ConfigureAwait(false);

        }

      
        public async Task<UserResponseDto> Create(UserDto dto)
        {
            await ValidateUser(dto.MobileNumber, dto.EmailAddress);
            var user = new User(dto.Name, dto.UserName, dto.EmailAddress, dto.Type)
            {
                PhoneNumber = dto.MobileNumber,
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            var userReponseModel = new UserResponseDto() { UserId = user.Id};
            if (result.Succeeded)
            {

                foreach(var roleId in dto.Roles)
                {
                    var role = await _roleManager.FindByIdAsync(roleId).ConfigureAwait(false) ?? throw new RoleNotFoundException();
                    await _userManager.AddToRoleAsync(user, role.Name).ConfigureAwait(false);
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var emailConfirmationLink = $"{dto.CurrentSiteDomain}/Account/Account/ConfirmEmail?email={user.Email}&token={System.Web.HttpUtility.UrlEncode(token)}";
                userReponseModel.EmailConfirmationLink = emailConfirmationLink;
               

            }
            else
            {
                var errors = "</br>";
                foreach (var error in result.Errors)
                {
                    errors = errors + $"{error.Description} </br>";
                }
                throw new CustomException(errors);
            }
            return userReponseModel;
        }

     
        public async Task Deactivate(string id)
        {
            var user = await _userRepo.GetByIdString(id).ConfigureAwait(false) ?? throw new UserNotFoundException();
            user.Deactivate();
            await _userRepo.UpdateAsync(user).ConfigureAwait(false);
        }

        public async Task Edit(UserEditDto dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(dto.Id).ConfigureAwait(false) ?? throw new UserNotFoundException();
                user.Update(dto.Name, dto.UserName, dto.EmailAddress, dto.MobileNumber);
               var response = await _userManager.UpdateAsync(user).ConfigureAwait(false);
                if(response.Succeeded)
                {
                    var userRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                    foreach (var role in userRoles)
                    {
                        await _userManager.RemoveFromRoleAsync(user, role).ConfigureAwait(false);
                    }
                    foreach (var roleId in dto.Roles)
                    {
                        var role = await _roleManager.FindByIdAsync(roleId).ConfigureAwait(false) ?? throw new RoleNotFoundException();
                        await _userManager.AddToRoleAsync(user, role.Name).ConfigureAwait(false);
                    }
                }
                else
                {

                }
                   

            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

      

        private async Task ValidateUser(string mobile, string email, User? user = null)
        {
            if (string.IsNullOrWhiteSpace(mobile) || string.IsNullOrWhiteSpace(email))
            {
                if (string.IsNullOrWhiteSpace(mobile)) throw new CustomException("Mobile Number is required");
                if (string.IsNullOrWhiteSpace(email)) throw new CustomException("Email address is required");
                return;
            }
            var userWithSameMobile = await _userRepo.GetByMobile(mobile).ConfigureAwait(false);
            if (userWithSameMobile != null && user != userWithSameMobile) throw new CustomException("User With Same Number Already Exists");
            var userWithSameEmail = await _userRepo.GetByEmail(email).ConfigureAwait(false);
            if (userWithSameEmail != null && user != userWithSameEmail) throw new CustomException("User With Same Email Already Exists");

        }
    }
}
