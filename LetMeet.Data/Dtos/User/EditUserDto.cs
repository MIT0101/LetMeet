using LetMeet.Data.Entites.UsersInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.User
{
    public class EditUserDto : UserInfo
    {
        public static EditUserDto GetEditUserDto(UserInfo userInfo) { 
        return new EditUserDto() { 
        id=userInfo.id,
        fullName=userInfo.fullName,
        emailAddress=userInfo.emailAddress,
        identityId=userInfo.identityId,
        phoneNumber=userInfo.phoneNumber,
        userRole = userInfo.userRole,
        profileImage=userInfo.profileImage,
        stage = userInfo.stage
        };
        }

    }

    }
