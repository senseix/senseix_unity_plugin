// Generated by ProtoGen, Version=2.4.1.521, Culture=neutral, PublicKeyToken=55f7125234beb589.  DO NOT EDIT!
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.ProtocolBuffers;
using pbc = global::Google.ProtocolBuffers.Collections;
using pbd = global::Google.ProtocolBuffers.Descriptors;
using scg = global::System.Collections.Generic;
namespace senseix.message.constant {
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public static partial class Constant {
  
    #region Extension registration
    public static void RegisterAllExtensions(pb::ExtensionRegistry registry) {
    }
    #endregion
    #region Static variables
    #endregion
    #region Descriptor
    public static pbd::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbd::FileDescriptor descriptor;
    
    static Constant() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          "Cg5Db25zdGFudC5wcm90bxIYc2Vuc2VpeC5tZXNzYWdlLmNvbnN0YW50Kq0C" + 
          "CgtNZXNzYWdlVHlwZRISCg5SZWdpc3RlckRldmljZRABEhIKDlJlZ2lzdGVy" + 
          "UGFyZW50EAISEAoMU2lnbkluUGFyZW50EAMSEQoNU2lnbk91dFBhcmVudBAE" + 
          "Eg4KCkVkaXRQYXJlbnQQBRIPCgtNZXJnZVBhcmVudBAGEhAKDENyZWF0ZVBs" + 
          "YXllchAHEg4KCkxpc3RQbGF5ZXIQCBIhCh1SZWdpc3RlclBsYXllcldpdGhB" + 
          "cHBsaWNhdGlvbhAJEg8KC1Byb2JsZW1Qb3N0EAoSDgoKUHJvYmxlbUdldBAL" + 
          "EhMKD0xlYWRlcmJvYXJkUGFnZRAMEg8KC1BsYXllclNjb3JlEA0SDgoKUGxh" + 
          "eWVyUmFuaxAOEhQKEEdhbWVWZXJpZmljYXRpb24QDyo2CgZTdGF0dXMSCwoH" + 
          "RkFJTFVSRRAAEgsKB1NVQ0NFU1MQARISCg5NRVJHRV9SRVFVSVJFRBAC");
      pbd::FileDescriptor.InternalDescriptorAssigner assigner = delegate(pbd::FileDescriptor root) {
        descriptor = root;
        return null;
      };
      pbd::FileDescriptor.InternalBuildGeneratedFileFrom(descriptorData,
          new pbd::FileDescriptor[] {
          }, assigner);
    }
    #endregion
    
  }
  #region Enums
  public enum MessageType {
    RegisterDevice = 1,
    RegisterParent = 2,
    SignInParent = 3,
    SignOutParent = 4,
    EditParent = 5,
    MergeParent = 6,
    CreatePlayer = 7,
    ListPlayer = 8,
    RegisterPlayerWithApplication = 9,
    ProblemPost = 10,
    ProblemGet = 11,
    LeaderboardPage = 12,
    PlayerScore = 13,
    PlayerRank = 14,
    GameVerification = 15,
  }
  
  public enum Status {
    FAILURE = 0,
    SUCCESS = 1,
    MERGE_REQUIRED = 2,
  }
  
  #endregion
  
}

#endregion Designer generated code
