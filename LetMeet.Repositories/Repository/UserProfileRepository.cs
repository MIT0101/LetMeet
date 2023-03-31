
using LetMeet.Data;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.ComponentModel.DataAnnotations;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq.Expressions;
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

        private Random _random;

        public UserProfileRepository(MainDbContext mainDb, IOptions<RepositoryDataSettings> repoSettingsOptions, IGenericRepository<UserInfo, Guid> genericUserRepository, ILogger<UserProfileRepository> logger)
        {
            _mainDb = mainDb;
            _usersInfo = mainDb.Set<UserInfo>();
            _settings = repoSettingsOptions.Value;
            _genericUserRepository = genericUserRepository;
            this._logger = logger;
            _random = new Random();
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

                Bitmap imgMap = new Bitmap(imageStream);

                if (imgMap is null)
                {
                    return await Task.FromResult(RepositoryResult<string>.FailureResult(ResultState.Error, null, new List<string>() {
                        "Can Not Update Image"
                    }));

                }
                // Calculate the size of the image in kilobytes
                var size = (imageStream.Length / 1024) + 1;
                string imageFileName = userInfoId.ToString("N") + "_" + _random.Next(10) + ".jpeg";

                // Check if the image exceeds the maximum size MAX_IMAGE_SIZE_IN_KB
                if (size > MAX_IMAGE_SIZE_IN_KB)
                {
                    List<ValidationResult> validationErrors = new List<ValidationResult>() {
                    new ValidationResult("Image Size Is Too Large , Try Another One")
                    };
                    _logger.LogError("Image Size Is Too Large , Try Another One");
                    return await Task.FromResult(RepositoryResult<string>.FailureValidationResult(validationErrors, null));

                    // Compress image to maximum size of MAX_IMAGE_SIZE_IN_KB KB
                    //await CompressImageAndSave(imgMap, imageFileName);
                }
                else {
                    imgMap.Save(Path.Combine(folderPath, imageFileName), ImageFormat.Jpeg);

                }
                var userToUpdate = await _mainDb.UserInfos.FindAsync(userInfoId);

                // Generate a unique file name for the image
                string oldImagePath = Path.Combine(folderPath, userToUpdate?.profileImage ?? "no.txt");

                await Task.Run(() =>
                {
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                });

                await imageStream.DisposeAsync();

                if (userToUpdate is null)
                {
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
                _logger.LogError("Can Not Save Image " + ex.Message);
                return await Task.FromResult(RepositoryResult<string>.FailureResult(ResultState.DbError, null, null));
            }

        }

        public async Task CompressImageAndSave(Bitmap imgMap, string savePath)
        {
            int quality = 70;
            // Encoder parameter for image quality 
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
            // JPEG image codec 
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            imgMap.Save(savePath, jpegCodec, encoderParams);
        }

        private ImageCodecInfo GetEncoder(ImageFormat jpeg)
        {
            throw new NotImplementedException();
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }

        public Task<RepositoryResult<SupervisionInfo>> UpdateSupervison(UserInfo supervisor, UserInfo student)
        {
            throw new NotImplementedException();
        }
    }
}

