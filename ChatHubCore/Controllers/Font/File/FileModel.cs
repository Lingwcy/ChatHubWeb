namespace ChatHubApi.Controllers.Font.File
{
    public record UploadFileModel(string base64String, string fileType, string fileName);
    public record UploadAvatarModel(string img,int userId);

    public class UploadUserInfoModel {
        public int id { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Sex { get; set; }
        public string Job { get; set; }
        public string Phone { get; set; }
        public string? UserName { get; set; }
        public string Birth { get; set; }
        public string Desc { get; set; }
    }

}
