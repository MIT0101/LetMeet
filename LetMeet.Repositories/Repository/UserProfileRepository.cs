using Alachisoft.NCache.Common.Licensing.Kubernetes;
using Alachisoft.NCache.Config.Dom;
using LetMeet.Data;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.Protocols;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.FSharp.Core.ByRefKinds;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;
using Alachisoft.NCache.Common.Licensing.Request;
using System.IO;

namespace LetMeet.Repositories.Repository
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ILogger<UserProfileRepository> _logger;

        private readonly MainDbContext _mainDb;
        private readonly DbSet<UserInfo> _usersInfo;
        private readonly RepositoryDataSettings _settings;
        private readonly IGenericRepository<UserInfo, Guid> _genericUserRepository;

        private readonly int MAX_IMAGE_SIZE_IN_KB = 300;


        public UserProfileRepository(MainDbContext mainDb, IOptions<RepositoryDataSettings> repoSettingsOptions, IGenericRepository<UserInfo, Guid> genericUserRepository, ILogger<UserProfileRepository> logger)
        {
            _mainDb = mainDb;
            _usersInfo = mainDb.Set<UserInfo>();
            _settings = repoSettingsOptions.Value;
            _genericUserRepository = genericUserRepository;
            this._logger = logger;
        }


        public async Task<RepositoryResult<UserInfo>> GetUserByIdAsync(Guid userInfoId)
        {
            return await _genericUserRepository.GetByIdAsync(userInfoId);
        }

        public Task<RepositoryResult<UserInfo>> GetUserIdAsync(Expression<Func<UserInfo, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryResult<UserInfo>> GetUserWhereAsync(Expression<Func<UserInfo, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public virtual Expression<Func<UserInfo, bool>> DefaultFilter()
        {
            return entity => true;
        }

        public virtual void nullFilterSolver(ref Expression<Func<UserInfo, bool>> filter)
        {
            if (filter == null)
            {
                filter = DefaultFilter();
            }
        }

        public async Task<(ResultState state, Guid? value)> GetIdentityIdAsync(Guid userInfoId)
        {
            try
            {
                //nullFilterSolver(ref filter);

                Guid? userIdentityId = await _usersInfo.Where(u => u.id == userInfoId).Select(u => u.identityId).FirstOrDefaultAsync();

                if (userIdentityId == null || userIdentityId.Equals(Guid.Empty))
                {
                    return await Task.FromResult((ResultState.NotFound, Guid.Empty));
                }

                return (ResultState.Seccess, userIdentityId);


            }
            catch (Exception ex)
            {
                return (ResultState.DbError, Guid.Empty);

            }
        }

        public async Task<RepositoryResult<string>> UpdateProfileImageAsync(Guid userInfoId, MemoryStream imageStream, string folderPath)
        {
            try
            {

                if (imageStream is null || imageStream.Length <= 0)
                {

                    List<ValidationResult> validationErrors = new List<ValidationResult>() {
                    new ValidationResult("Invalid Image is Empty")
                    };
                    _logger.LogError("Invalid Image Input");
                    return await Task.FromResult(RepositoryResult<string>.FailureValidationResult(validationErrors, null));
                }

                Image image = Image.FromStream(imageStream);
                

                if (image == null)
                {

                    return await Task.FromResult(RepositoryResult<string>.FailureResult(ResultState.Error, null, new List<string>() {
                        "Can Not Update Image"
                    }));

                }

                // Calculate the size of the image in kilobytes
                var size = (imageStream.Length / 1024) + 1;

                await imageStream.DisposeAsync();

                // Check if the image exceeds the maximum size MAX_IMAGE_SIZE_IN_KB
                if (size > MAX_IMAGE_SIZE_IN_KB)
                {
                    // Compress image to maximum size of MAX_IMAGE_SIZE_IN_KB KB
                    image = await CompressImage(image, MAX_IMAGE_SIZE_IN_KB * 1024);
                }
                // Generate a unique file name for the image

                string imageFileName = userInfoId.ToString("N")+ ".jpeg";

                //save image
                //image.Save(Path.Combine(folderPath, imageFileName), ImageFormat.Jpeg);

                var imageMap = new Bitmap(image);
                image = null;
                imageMap.Save(Path.Combine(folderPath, imageFileName), ImageFormat.Jpeg);


                var userToUpdate = await _mainDb.UserInfos.FirstOrDefaultAsync(u=>u.id==userInfoId);

                if (userToUpdate is null) {
                    return await Task.FromResult(RepositoryResult<string>.FailureResult(ResultState.Error, null, new List<string>() {
                        "User Not Found To Update His Image"
                    }));
                }

                userToUpdate.profileImage = imageFileName;

                _mainDb.UserInfos.Update(userToUpdate);

                await _mainDb.SaveChangesAsync();



                // Return a success response
                return await Task.FromResult(RepositoryResult<string>.SuccessResult(ResultState.Seccess, imageFileName));

            }
            catch (Exception ex)
            {
                _logger.LogError("Can Not Save Image " +ex.Message);
                return await Task.FromResult(RepositoryResult<string>.FailureResult(ResultState.DbError, null, null));
            }




        }
        private async Task<Image> CompressImage(Image image, long maxSize)
        {
            long quality = 90L; // Starting JPEG quality
            long minQuality = 50L; // Minimum JPEG quality

            // Compress image until it is under maxSize
            while (await GetImageSize(image) > maxSize && quality >= minQuality)
            {
                quality -= 5L;

                using (MemoryStream ms = new MemoryStream())
                {
                    // Set JPEG quality and save to memory stream
                    EncoderParameters eps = new EncoderParameters(1);
                    eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                    ImageCodecInfo ici = await GetEncoderInfo("image/jpeg");
                    image.Save(ms, ici, eps);

                    // Load compressed image from memory stream
                    ms.Seek(0, SeekOrigin.Begin);
                    image = Image.FromStream(ms);
                }
            }

            return image;
        }
        private async Task<long> GetImageSize(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return ms.Length;
            }
        }

        private async Task<ImageCodecInfo> GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType == mimeType)
                {
                    return codec;
                }
            }
            return null;
        }








    }
}

