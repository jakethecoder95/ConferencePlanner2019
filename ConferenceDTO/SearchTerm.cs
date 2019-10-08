namespace ConferenceDTO
{
    public enum SearchResultType
    {
        Attendee,
        Conference,
        Session,
        Track,
        Speaker
    }
    namespace ConferenceDTO
    {
        public class SearchTerm
        {
            public string Query { get; set; }
        }
    }
}