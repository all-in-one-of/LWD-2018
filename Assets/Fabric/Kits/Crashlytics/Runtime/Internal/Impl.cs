namespace Fabric.Crashlytics.Internal
{
	using Fabric.Internal.Runtime;
	using System.Diagnostics;

	internal class Impl
	{
		protected const string KitName = "Crashlytics";

		public static Impl Make()
		{
			#if UNITY_IOS && !UNITY_EDITOR
			return new IOSImpl ();
			#elif UNITY_ANDROID && !UNITY_EDITOR
			return new AndroidImpl ();
			#else
			return new Impl ();
			#endif
		}
		
		public virtual void SetDebugMode(bool mode)
		{
		}
		
		public virtual void Crash()
		{
			Utils.Log (KitName, "Method Crash () is unimplemented on this platform");
		}
		
		public virtual void ThrowNonFatal()
		{
			#if !UNITY_EDITOR
			string s = null;
			string l = s.ToLower ();
			Utils.Log (KitName, l);
			#else
			Utils.Log (KitName, "Method ThrowNonFatal () is not invokable from the context of the editor");
			#endif
		}
		
		public virtual void Log(string message)
		{
			Utils.Log (KitName, "Would log custom message if running on a physical device: " + message);
		}
		
		public virtual void SetKeyValue(string key, string value)
		{
			Utils.Log (KitName, "Would set key-value if running on a physical device: " + key + ":" + value);
		}
		
		public virtual void SetUserIdentifier(string identifier)
		{
			Utils.Log (KitName, "Would set user identifier if running on a physical device: " + identifier);
		}
		
		public virtual void SetUserEmail(string email)
		{
			Utils.Log (KitName, "Would set user email if running on a physical device: " + email);
		}
		
		public virtual void SetUserName(string name)
		{
			Utils.Log (KitName, "Would set user name if running on a physical device: " + name);
		}

		public virtual void RecordCustomException(string name, string reason, StackTrace stackTrace)
		{
			Utils.Log (KitName, "Would record custom exception if running on a physical device: " + name + ", " + reason);
		}

		public virtual void RecordCustomException(string name, string reason, string stackTraceString)
		{
			Utils.Log (KitName, "Would record custom exception if running on a physical device: " + name + ", " + reason);
		}
	}
}