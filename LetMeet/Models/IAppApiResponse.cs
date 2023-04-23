namespace LetMeet.Models
{
    public interface IAppApiResponse<T>
    {
        public string status { get; set; }
        public bool isSuccess { get; set; }
        public List<string> messages { get; set; }
        public List<string> errors { get; set; }

        public T? data { get; set; }

    }
}
