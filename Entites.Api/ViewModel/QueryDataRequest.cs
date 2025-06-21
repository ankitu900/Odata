namespace Entites.Api.ViewModel
{
    public class QueryDataRequest
    {
        public string Query { get; set; }

        public QueryDataParameter[] Parameters { get; set; }
    }

    public class QueryDataParameter
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }

    public class QueryDataResponse
    {
        public string Json { get; set; }
    }
}
