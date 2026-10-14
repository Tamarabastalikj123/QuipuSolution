namespace QuipuSolution.Authorization
{
    public static class AuthorizationPolicies
    {
        // CRUD endpoints - token from the client credentials flow
        public const string BookCrud = "BookCrud";

        // Search endpoint - token from the implicit flow (issued to a logged-in user)
        public const string BookSearch = "BookSearch";
    }
}
