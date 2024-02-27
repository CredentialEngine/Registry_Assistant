namespace RA.Models.Input
{
	public class CredentialingActionRequest : BaseRequest
    {
        /// <summary>
        /// constuctor
        /// </summary>
        public CredentialingActionRequest()
        {
        }

        public CredentialingAction CredentialingAction { get; set; } = new CredentialingAction();
    }


}
