
namespace Endava.BuildAndDeploy
{
    public struct BuildStepResult
    {
        public static BuildStepResult Successfull => new BuildStepResult(true, string.Empty);
        public static BuildStepResult CreateError(string errorReason) => new BuildStepResult(false, errorReason);
        public static BuildStepResult CreateSuccess() => new BuildStepResult(true, string.Empty);
        
        public bool Succeeded { get; }
        public string ErrorMessage { get; }

        public BuildStepResult(bool succeeded, string reason)
        {
            Succeeded = succeeded;
            ErrorMessage = reason;
        }

        public override string ToString()
        {
            return $"{nameof(Succeeded)}: {Succeeded}, {nameof(ErrorMessage)}: {ErrorMessage}";
        }
    }
}