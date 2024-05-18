namespace ApiService.Models
{
    public class ArchiveCreationProcess
    {
        public int Id { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreationTime{ get; set; } = DateTime.Now;

        public IEnumerable<string> FilePaths { get; set; } = null!;
    }
}
