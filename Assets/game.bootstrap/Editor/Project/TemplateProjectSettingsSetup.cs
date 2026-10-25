namespace Game.Bootstrap.Editor.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEngine;

    public static class TemplateProjectSettingsSetup
    {
        #region Configurations

        public const string CompanyName = "Amihan";
        public const string ProductName = "Unity Template";
        public const string ApplicationIdentifier = "com.amihan.unitytemplate";
        public const string BundleVersion = "0.1.0";
        public const int AndroidBundleVersionCode = 1;
        public const string IosBuildNumber = "1";

        private const string ProjectSettingsPath = "ProjectSettings/ProjectSettings.asset";
        private const string ActiveInputHandlerProperty = "activeInputHandler";
        private const int LegacyInputManager = 0;

        private static readonly HashSet<string> ProductDefines = new(StringComparer.Ordinal)
        {
            "DEV_PROFILE",
            "DOTWEEN",
            "FALCON_ANALYTIC",
            "ODIN_INSPECTOR",
            "ODIN_INSPECTOR_3",
            "ODIN_INSPECTOR_3_1",
            "ODIN_INSPECTOR_3_2",
            "ODIN_INSPECTOR_3_3",
            "ODIN_VALIDATOR",
            "ODIN_VALIDATOR_3_1",
            "ODIN_VALIDATOR_3_2",
            "ODIN_VALIDATOR_3_3",
            "PRODUCTION_BUILD",
            "SRDEBUGGER",
            "USE_ADJUST",
            "USE_APPSFLYER",
            "USING_ADMOB",
            "USING_APPSFLYER",
            "USING_APS",
            "USING_FIREBASE",
            "USING_MAX"
        };

        #endregion Configurations

        #region Methods

        [MenuItem("Game Template/Project/Apply Neutral Identity", priority = 10)]
        public static void ApplyNeutralIdentity()
        {
            if (PlayerSettings.companyName != CompanyName)
            {
                PlayerSettings.companyName = CompanyName;
            }

            if (PlayerSettings.productName != ProductName)
            {
                PlayerSettings.productName = ProductName;
            }

            if (PlayerSettings.bundleVersion != BundleVersion)
            {
                PlayerSettings.bundleVersion = BundleVersion;
            }

            if (PlayerSettings.Android.bundleVersionCode != AndroidBundleVersionCode)
            {
                PlayerSettings.Android.bundleVersionCode = AndroidBundleVersionCode;
            }

            if (PlayerSettings.iOS.buildNumber != IosBuildNumber)
            {
                PlayerSettings.iOS.buildNumber = IosBuildNumber;
            }

            SetSerializedIntSetting(ActiveInputHandlerProperty, LegacyInputManager);

            SetApplicationIdentifier(NamedBuildTarget.Standalone);
            SetApplicationIdentifier(NamedBuildTarget.Android);
            SetApplicationIdentifier(NamedBuildTarget.iOS);

            foreach (var target in GetAvailableBuildTargets())
            {
                var current = PlayerSettings.GetScriptingDefineSymbols(target)
                    .Split(';', StringSplitOptions.RemoveEmptyEntries);
                var neutral = current.Where(symbol => !ProductDefines.Contains(symbol)).ToArray();
                if (!current.SequenceEqual(neutral))
                {
                    PlayerSettings.SetScriptingDefineSymbols(target, neutral);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[Template Project] Neutral identity applied.");
        }

        #endregion Methods

        #region Subroutines

        private static IEnumerable<NamedBuildTarget> GetAvailableBuildTargets()
        {
            var targets = new HashSet<NamedBuildTarget>();
            foreach (BuildTargetGroup group in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (group == BuildTargetGroup.Unknown)
                {
                    continue;
                }

                try
                {
                    targets.Add(NamedBuildTarget.FromBuildTargetGroup(group));
                }
                catch (ArgumentException)
                {
                    // Unity retains obsolete enum values that no longer have a named target.
                }
            }

            return targets;
        }

        private static void SetApplicationIdentifier(NamedBuildTarget target)
        {
            if (PlayerSettings.GetApplicationIdentifier(target) == ApplicationIdentifier)
            {
                return;
            }

            PlayerSettings.SetApplicationIdentifier(target, ApplicationIdentifier);
        }

        private static void SetSerializedIntSetting(string propertyName, int value)
        {
            var settings = AssetDatabase.LoadAllAssetsAtPath(ProjectSettingsPath).FirstOrDefault();
            if (settings == null)
            {
                throw new InvalidOperationException($"Unable to load {ProjectSettingsPath}.");
            }

            var serializedSettings = new SerializedObject(settings);
            var property = serializedSettings.FindProperty(propertyName);
            if (property == null)
            {
                throw new InvalidOperationException(
                    $"Missing PlayerSettings property: {propertyName}.");
            }

            if (property.intValue == value)
            {
                return;
            }

            property.intValue = value;
            serializedSettings.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion Subroutines
    }
}
