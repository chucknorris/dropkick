namespace dropkick.Tasks {
   using DeploymentModel;

   public class NoteTask :
       Task {
      readonly DeploymentItemStatus _status;
      readonly string _message;

      public NoteTask(string message, DeploymentItemStatus status) {
         _message = message;
         _status = status;
      }
      public NoteTask(string message):this(message, DeploymentItemStatus.Note) {}

      public string Name {
         get { return "NOTE: {0}".FormatWith(_message); }
      }

      public DeploymentResult VerifyCanRun() {
         return ReturnResult();
      }

      public DeploymentResult Execute() {
         return ReturnResult();
      }

      private DeploymentResult ReturnResult() {
         var dr = new DeploymentResult();
         switch(_status) {
            case DeploymentItemStatus.Good:
               dr.AddGood(_message);
               break;
            case DeploymentItemStatus.Alert:
               dr.AddAlert(_message);
               break;
            case DeploymentItemStatus.Error:
               dr.AddError(_message);
               break;
            case DeploymentItemStatus.Note:
               dr.AddNote(_message);
               break;
            case DeploymentItemStatus.Verbose:
               dr.AddVerbose(_message);
               break;
            default:
               dr.AddError(string.Format(@"Unkown DeploymentItemStatus: '{0}'; message: '{1}'", _status, _message));
               break;
         }
         return dr;
      }
   }
}