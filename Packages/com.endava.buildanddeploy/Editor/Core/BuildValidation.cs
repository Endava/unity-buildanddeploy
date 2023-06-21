namespace Endava.BuildAndDeploy
{
    /// <summary>
    /// Within the build process, all build steps|process steps will report their status by using this enumeration.
    /// </summary>
    public enum ValidationStatus
    {
        /// <summary>
        /// Anything is ok - nothing to complain about.
        /// </summary>
        Valid,
        /// <summary>
        /// The validation have some inconsistencies or issues, which are not critical.
        /// </summary>
        Issued,
        /// <summary>
        /// The validation has errors and is corrupted.
        /// </summary>
        Invalid
    }

    /// <summary>
    /// Container, which helps to identify and report build validation issues.
    /// </summary>
    public struct BuildValidation
    {
        public static readonly BuildValidation Valid = new BuildValidation(ValidationStatus.Valid, string.Empty);
        public static BuildValidation CreateInvalid(string errorMessage) => new BuildValidation(ValidationStatus.Invalid, errorMessage);
        public static BuildValidation CreateIssued(string warningMessage) => new BuildValidation(ValidationStatus.Issued, warningMessage);

        public bool Succeeded => Status != ValidationStatus.Invalid; // Issued and Valid are ok
        public ValidationStatus Status { get; private set; }
        public string Message { get; private set; }

        public BuildValidation(ValidationStatus status, string errorMessage)
        {
            Status = status;
            Message = errorMessage;
        }

        public override string ToString()
        {
            return $"{nameof(Succeeded)}: {Succeeded}, {nameof(Status)}: {Status}, {nameof(Message)}: {Message}";
        }

        /// <summary>
        /// Concat two BuildValidation states and returns a BuildValidation matching both results
        /// </summary>
        public static BuildValidation Concat(BuildValidation a, BuildValidation b)
        {
            // if b is valid, just return a
            if (b.Status == ValidationStatus.Valid)
                return a;

            ValidationStatus status = a.Status;
            // fusion states
            if (status < b.Status)
                status = b.Status;

            string message = a.Message;
            if (string.IsNullOrEmpty(message))
                message = b.Message;
            else
                message += $"{System.Environment.NewLine}->{b.Message}";

            return new BuildValidation(status, message);
        }
    }
}