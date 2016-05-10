namespace Fabric.Internal.Editor.Controller
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Globalization;
	using System.Net;
	using System.Timers;
	using Fabric.Internal.Editor.Model;
	using Fabric.Internal.Editor.View;
	using Fabric.Internal.Editor.Net.OAuth;
	using Fabric.Internal.Editor.Update;
	using KitsList = System.Collections.Generic.List<Update.Dependency.DependencyGraphObject.DependenciesObject.KitsObject>;
	using KitsObject = Update.Dependency.DependencyGraphObject.DependenciesObject.KitsObject;
	using PluginObject = Update.Dependency.DependencyGraphObject.PluginObject;

	internal class PluginController
	{
		private static Client client = new Client (Net.Constants.URI);

		private static Update.FabricInstaller.Config defaultConfig = new FabricInstaller.Config (
			"https://s3.amazonaws.com/ssl-download-crashlytics-com/unity-fabric/Fabric.unitypackage",
			"Fabric.unitypackage",
			""
		);
		private static Update.FabricInstaller.Config LatestPluginConfig
		{
			get {
				PluginObject latest = Update.PeriodicUpdateManager.PluginDescriptor ();
				return latest == null ? defaultConfig : new FabricInstaller.Config (
					latest.PackageUrl,
					latest.PackageName,
					latest.ReleaseNotesUrl
				);
			}
		}
		private Update.FabricInstaller fabricInstaller = new FabricInstaller (LatestPluginConfig);

		private KitInstallationChecker kitInstallationChecker;
		private float nextKitInstallationCheck = 0f;
		private const float kitInstallCheckDelay = 10.0f;
		private const string ConfiguredTimestampKey = "ConfiguredTimestamp";

		// This is intended to be called in the GUI loop.
		public void ActivationCheck()
		{
			bool isTimeToCheck = Time.realtimeSinceStartup > nextKitInstallationCheck;
			if (kitInstallationChecker != null || !isTimeToCheck) {
				return; // We're currently checking.
			}

			string organization = Settings.Instance.Organization.Name;
			string bundleIdentifier = PlayerSettings.bundleIdentifier;
			BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

			kitInstallationChecker = new KitInstallationChecker ();
			kitInstallationChecker.CheckInstalledKits (organization, bundleIdentifier, buildTarget, (activatedApp) => {
				if (activatedApp == null) {
					ResetKitInstallationChecker ();
					return;
				}

				HashSet<string> installedKitNames = new HashSet<string> (
					activatedApp.SdkKits.ConvertAll (kit => kit.Name), StringComparer.OrdinalIgnoreCase
				);

				SendKitsAnalytics (installedKitNames);

				MarkKitsAsInstalled(installedKitNames);
				Settings.Instance.IconUrl = activatedApp.IconUrl;
				Settings.Instance.DashboardUrl = activatedApp.DashboardUrl;

				ResetKitInstallationChecker ();
			}, (Exception exception) => {
				Utils.Warn ("Failed checking for kit installation: {0}", exception.Message);
				ResetKitInstallationChecker ();
			}, (Exception noNetwork) => {
				ResetKitInstallationChecker ();
			});
		}

		private static void SendKitsAnalytics(HashSet<string> installedKitNames)
		{
			HashSet<string> configuredKitNames = new HashSet<string> (
				Settings.Instance.InstalledKits
				.FindAll (kit => kit.InstallationStatus == Settings.KitInstallationStatus.Configured)
				.ConvertAll (kit => kit.Name), StringComparer.OrdinalIgnoreCase
			);

			DateTime now = DateTime.UtcNow;

			foreach (string installedKitName in installedKitNames) {
				if (configuredKitNames.Contains (installedKitName)) {
					SendKitAnalytics (now, installedKitName);
				}
			}
		}

		private static void SendKitAnalytics(DateTime now, string installedKitName)
		{
			Settings.InstalledKit.MetaTuple timestampTuple = Settings.Instance.InstalledKits
				.Find (k => k.Name.Equals (installedKitName, StringComparison.OrdinalIgnoreCase)).Meta
				.Find (tuple => tuple.Key.Equals (ConfiguredTimestampKey));

			if (timestampTuple == null) {
				return;
			}

			double configuredTimestampSeconds = Double.Parse (timestampTuple.Value, CultureInfo.InvariantCulture);

			Update.PeriodicPinger.Enqueue (Analytics.TimeBucket.From (
				Detail.TimeUtils.FromEpochSeconds (configuredTimestampSeconds),
				now,
				installedKitName
			));
		}

		private void ResetKitInstallationChecker()
		{
			kitInstallationChecker = null;
			nextKitInstallationCheck = Time.realtimeSinceStartup + kitInstallCheckDelay;
		}

		private string password;
		
		private Page login;
		private Page orgs;
		
		private Page validation;
		private Page kitSelection;
		private Page updatePage;
		
		public enum LoginStatus { Unknown, Success, Unauthorized, Other };
		public delegate void LoginAction<T, U>(T password, out U status);
		
		private ImportedKit kit;

		private string iconUrl;
		private string dashboardUrl;

		private API.V1 Api
		{
			get {
				return new API.V1 (client.URI, client, Settings.Instance.Token);
			}
		}

		private Detail.AsyncTaskRunnerBuilder<Client.Token> asyncLogin = null;
		private Detail.AsyncTaskRunnerBuilder<Client.Token> AsyncLogin
		{
			get {
				if (asyncLogin == null)
					asyncLogin = new Detail.AsyncTaskRunnerBuilder<Client.Token> ();
				return asyncLogin;
			}
		}
		
		public static PluginController instance;
		public static PluginController Instance
		{
			get {
				if (instance == null)
					instance = new PluginController ();
				return instance;
			}
		}

		public PluginController()
		{
			login = new LoginPage (Login ());
			orgs = new OrganizationsPage (SelectOrganization (), FetchOrganizationsAsync);
			validation = new ValidationPage (BackToKitSelection, SetSelectedKitConfigured);
			kitSelection = new KitSelectionPage (
				ListAvailableKits,
				KitUtils.ListImportedKits (Api),
				SelectKit (),
				ActivationCheck,
				BackToOrganizations (),
				Update.PeriodicUpdateManager.IsPluginUpdateAvailable,
				ShowUpdatePage (() => { return LatestPluginConfig; }, UpdateFlow.Plugin)
			);
			updatePage = new UpdatePage (InitiateUpdate, BackToKitSelection, 
				delegate() {
					return String.IsNullOrEmpty (Settings.Instance.Kit) ? "Fabric Plugin" : Settings.Instance.Kit;
				},
				delegate () {
					return fabricInstaller.FetchReleaseNotes ();
				}
			);
		}

		#region Update
		internal enum UpdateFlow
		{
			Plugin = 100,
			Kit = 200
		}

		private Action ShowUpdatePage(Func<Update.FabricInstaller.Config> config, UpdateFlow updateFlow)
		{
			return delegate() {
				if (!Update.PeriodicUpdateManager.Suspend ()) {
					// As an end-user, the probability of seeing this message is low.
					Utils.Log ("Dependency graph refresh is in progress...");
					return;
				}

				fabricInstaller.SwapConfig (config ());
				switch (updateFlow) {
				case UpdateFlow.Plugin:
					Settings.Instance.FlowSequence = (int)UpdateFlow.Plugin;
					break;
				case UpdateFlow.Kit:
					Settings.Instance.FlowSequence = (int)UpdateFlow.Kit;
					break;
				}
			};
		}

		private void BackToKitSelection()
		{
			Update.PeriodicPinger.Enqueue (new Analytics.Events.PageViewEvent {
				ScreenName = "KitSelectionPage",
			});
			Settings.Instance.FlowSequence = 0;
			Settings.Instance.Kit = null;
		}

		private void InitiateUpdate(
			Action<float> reportProgress,
			Action<string> downloadComplete,
			Action<System.Exception> downloadError,
			Func<bool> isCancelled
		)
		{
			Update.PeriodicPinger.Enqueue (new Analytics.Events.UpdateTakenEvent (KitUtils.AnalyticsStateString ()));

			fabricInstaller.DownloadAndInstallPackage (
				new FabricInstaller.ReportInstallProgress (reportProgress),
				new FabricInstaller.DownloadComplete (downloadComplete),
				new FabricInstaller.DownloadError(downloadError),
				new FabricInstaller.IsCancelled(isCancelled)
			);
		}
		#endregion

		private void FetchOrganizationsAsync(Action<List<Organization>> onSuccess, Action<string> onFailure)
		{
			API.AsyncV1.Fetch<List<Organization>> (onSuccess, onFailure, (API.V1 api) => {
				return api.Organizations ();
			});
		}
		
		#region PageFromState

		private static bool NoAuthToken()
		{
			return Settings.Instance.Token == null;
		}

		private static bool NoOrganizationSelected()
		{
			return Settings.Instance.Organization == null || String.IsNullOrEmpty (Settings.Instance.Organization.Name);
		}

		private bool NoKitSelected()
		{
			return String.IsNullOrEmpty (Settings.Instance.Kit) || kit == null || kit.Instance == null;
		}

		private static bool PluginUpdateInProgress()
		{
			return Settings.Instance.FlowSequence == (int)UpdateFlow.Plugin;
		}

		private static bool KitUpdateInProgress()
		{
			return Settings.Instance.FlowSequence == (int)UpdateFlow.Kit;
		}

		private void RecreateKitInstance()
		{
			if (kit == null) {
				if (!String.IsNullOrEmpty (Settings.Instance.Kit)) {
					kit = KitUtils.ListImportedKits (Api)().Find (k => k.Name.Equals (Settings.Instance.Kit));
				}
			}
		}

		private static void CleanKitUpgrade(string name)
		{
			Update.PeriodicUpdateManager.Continue ();

			KitsObject kitsObject = Update.PeriodicUpdateManager.LatestAvailableKitsVersions ().Find (
				k => k.Name.Equals (name, StringComparison.OrdinalIgnoreCase)
			);

			if (kitsObject != null) {
				Detail.Runner.StartCoroutine (new Custodian (kitsObject.Manifest).Clean ());
			}
		}

		private static void CleanPluginUpgrade()
		{
			Update.PeriodicUpdateManager.Continue ();

			PluginObject pluginObject = Update.PeriodicUpdateManager.PluginDescriptor ();

			if (pluginObject != null) {
				Detail.Runner.StartCoroutine (new Custodian (pluginObject.Manifest).Clean ());
			}
		}

		public static bool LegacyUpdateFlow()
		{
			return Settings.Instance.FlowSequence == 1;
		}

		public Page PageFromState()
		{
			UpdateInstalledKitStatus ();

			if (NoAuthToken ()) {
				return login;
			}
			
			if (NoOrganizationSelected ()) {
				return orgs;
			}

			RecreateKitInstance ();

			if (PluginUpdateInProgress () && Update.PeriodicUpdateManager.IsPluginUpdateAvailable () && NoKitSelected ()) {
				return updatePage;
			}

			if (KitUpdateInProgress ()) {
				if (KitUtils.IsKitInstalled (Settings.Instance.Kit)) {
					if (!Update.PeriodicUpdateManager.IsKitUpdateAvailable (kit.Name, kit.Instance.Version ())) {
						// At this point, the upgrade is done for the given kit. Since this kit was previously
						// installed, we do not need to go through the flow.

						CleanKitUpgrade (kit.Name);
						BackToKitSelection ();
						return kitSelection;
					}

					// Waiting for the import to finish.
					return updatePage;
				}
				
				if (kit != null && Update.PeriodicUpdateManager.IsKitUpdateAvailable (kit.Name, kit.Instance.Version ()) || kit == null) {
					return updatePage;
				} else {
					CleanKitUpgrade (kit.Name);

					// Trigger the kit flow.
					Settings.Instance.FlowSequence = 0;
				}
			}

			if (NoKitSelected ()) {
				if (Settings.Instance.FlowSequence == (int)UpdateFlow.Plugin || LegacyUpdateFlow ()) {
					CleanPluginUpgrade ();
				}

				Settings.Instance.FlowSequence = 0;
				return kitSelection;
			}

			// At this point, the kit is selected. If it does not exist in the installed kits list,
			// add it.

			AddKitToKitsList (kit.Name);

			Page page = null;
			KitControllerStatus status = kit.Instance.PageFromState (out page);
			
			switch (status) {
			case KitControllerStatus.NextPage:
				return page;
			case KitControllerStatus.LastPage:
				return validation;
			case KitControllerStatus.CurrentPage:
				Settings.Instance.Kit = "";
				break;
			}

			return kitSelection;
		}

		private static void AddKitToKitsList(string name)
		{
			List<Settings.InstalledKit> installedKits = Settings.Instance.InstalledKits;

			if (installedKits.Exists (
				k => k.Name.Equals (name, StringComparison.OrdinalIgnoreCase)
			)) {
				return;
			}

			installedKits.Add (new Settings.InstalledKit {
				Name = name,
				InstallationStatus = Settings.KitInstallationStatus.Imported,
				Installed = false,
				Enabled = false
			});
		}

		#endregion

		#region Login
		private static Detail.AsyncTaskRunner<Client.Token>.ErrorRecovery OnLoginError(System.Exception e, Action<string> messageOnError)
		{
			if (Net.Utils.IsNetworkUnavailableFrom (e)) {
				messageOnError ("Network connection is not available.");
				return Detail.AsyncTaskRunner<Client.Token>.ErrorRecovery.Nothing;
			}

			Utils.Warn ("An exception has occured; {0}", e.Message);

			if (!(e is WebException)) {
				messageOnError ("Please contact support@fabric.io");
				return Detail.AsyncTaskRunner<Client.Token>.ErrorRecovery.Nothing;
			}
			
			WebException webException = e as WebException;
			HttpWebResponse response = webException.Response as HttpWebResponse;
			
			if (response != null && response.StatusCode == HttpStatusCode.Unauthorized) {
				messageOnError ("Invalid Credentials");
				return Detail.AsyncTaskRunner<Client.Token>.ErrorRecovery.Nothing;
			}
			
			messageOnError ("Network Error!");
			return Detail.AsyncTaskRunner<Client.Token>.ErrorRecovery.Nothing;
		}

		private Action<string, Action<string>> Login()
		{
			return delegate(string password, Action<string> messageOnError) {
				AsyncLogin.Do (delegate (object[] args) {
					return Net.Validator.MakeRequest (
						() => { return client.Get (args[0] as string, args[1] as string); }
					);
				}).OnError ((System.Exception e) => {
					return OnLoginError (e, messageOnError);
				}).OnCompletion ((Client.Token token) => {
					Settings.Instance.Token = token;
				}).Run (Settings.Instance.Email, password);
			};
		}
		#endregion
		
		#region SelectOrganization
		private static Action<Organization> SelectOrganization()
		{
			return delegate(Organization organization) {
				Update.PeriodicPinger.Enqueue (new Analytics.Events.PageViewEvent {
					ScreenName = "KitSelectionPage",
				});
				Settings.Instance.FlowSequence = 0;
				Settings.Instance.Organization = organization;
			};
		}
		#endregion

		#region BackToOrganizations
		private static Action BackToOrganizations()
		{
			return delegate() {
				Settings.Instance.FlowSequence = 0;
				Settings.Instance.Organization = null;
			};
		}
		#endregion
		
		#region SelecKit
		private Action<KitsObject, ImportedKit> SelectKit()
		{
			return delegate(KitsObject availableKit, ImportedKit importedKit) {
				if (Update.PeriodicUpdateManager.IsPluginUpdateRequired (availableKit.Name)) {
					EditorUtility.DisplayDialog (
						"A plugin update is required",
						availableKit.Name + " requires a newer version of the Fabric Plugin, please update by clicking 'View Update'.",
						"OK"
					);
					return;
				}

				Settings.Instance.Kit = availableKit.Name;

				if (importedKit != null && KitUtils.IsUpToDate(availableKit, importedKit)) {
					// Delegate to the kit's controller.
					this.kit = importedKit;
					Settings.Instance.FlowSequence = 0;
					return;
				}

				// We need to download the latest version of the kit, because it's either not imported or needs updating.

				this.kit = null;
				Settings.Instance.FlowSequence = (int)UpdateFlow.Kit;

				FabricInstaller.Config config = new FabricInstaller.Config (
					availableKit.PackageUrl,
					availableKit.PackageName,
					availableKit.ReleaseNotesUrl
				);

				List<string> kitsToUpdateDueToDependencies = Update.PeriodicUpdateManager.Resolve (Settings.Instance.Kit);

				if (kitsToUpdateDueToDependencies.Count == 0) {
					ShowUpdatePage (() => { return config; }, UpdateFlow.Kit)();
					return;
				}

				//TODO: implement dependency resolution
				Utils.Log ("There is a dependency issue that needs resolution");
			};
		}
		#endregion

		// This will properly set kit installation status in the case that
		// the kit was installed before KitInstallationStatus was added
		private static void UpdateInstalledKitStatus()
		{
			Settings.Instance.InstalledKits.ForEach (kit => {
				if (kit.Installed) {
					kit.InstallationStatus = Settings.KitInstallationStatus.Installed;
				}
			});
		}

		private void SetSelectedKitConfigured()
		{
			string selectedKit = Settings.Instance.Kit;
			if (String.IsNullOrEmpty (selectedKit)) {
				return;
			}
			Settings.InstalledKit installedKit = Settings.Instance.InstalledKits.Find (kit => kit.Name.Equals (selectedKit, StringComparison.OrdinalIgnoreCase));
			if (installedKit == null) {
				return;
			}
			installedKit.InstallationStatus = Settings.KitInstallationStatus.Configured;

			installedKit.Meta.RemoveAll (tuple => tuple.Key.Equals (ConfiguredTimestampKey, StringComparison.OrdinalIgnoreCase));
			installedKit.Meta.Add (new Settings.InstalledKit.MetaTuple {
				Key = ConfiguredTimestampKey,
				Value = Detail.TimeUtils.SecondsSinceEpoch.ToString ("R", CultureInfo.InvariantCulture),
			});
		}

		public KitsList ListAvailableKits()
		{
			return Update.PeriodicUpdateManager.LatestAvailableKitsVersions ();
		}

		private static void MarkKitsAsInstalled(HashSet<string> kitsToMark)
		{
			Settings.Instance.InstalledKits.ForEach (kit => {
				if (kitsToMark.Contains (kit.Name)) {
					kit.InstallationStatus = Settings.KitInstallationStatus.Installed;
					kit.Installed = true;
					kit.Enabled = true;
				}
			});
		}
	}
}
