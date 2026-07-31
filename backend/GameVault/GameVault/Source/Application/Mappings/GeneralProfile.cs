using AutoMapper;
using GameVault.Source.Application.Dtos.Auth;
using GameVault.Source.Application.Dtos.User;
using GameVault.Source.Domain.Entities;
using Microsoft.AspNetCore.Identity.Data;

namespace GameVault.Source.Application.Mappings
{
    public class GeneralProfile: Profile
    {
        public GeneralProfile() 
        {

            #region UserProfile

            // ApplicationUser -> UserDto
            CreateMap<ApplicationUser, UserDto>()
                .ReverseMap();
            #endregion

        }

    }
}
