namespace console_application.Data
{
    public struct DbResponse
    {
        public bool isSuccess = false;
        public string errorMsg = "Unset error";
        public string sucessMsg = "";

        public DbResponse(bool isSucess, string errorMsg, string successMsg)
        {
            this.isSuccess = isSucess;
            this.errorMsg = errorMsg;
            this.sucessMsg = successMsg;
        }
    }
}
