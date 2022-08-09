namespace NGrep
{
    public class GrepParams
    {
        public bool Recursive { get; set; }
        public bool ListFilesOnly { get; set; }
        public bool ShowLineNumbers { get; set; }
        public bool Replace { get; set; }
        public bool IgnoreCase { get; set; }
        public bool ExactMatch { get; set; }
        public string SearchTerm { get; set; }
        public string SearchIn { get; set; }
        public string SearchPattern { get; set; }
        public string ReplaceWith { get; set; }
    }
}
