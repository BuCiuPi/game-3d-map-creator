namespace Game.Bootstrap.Tests.EditMode
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Game.Bootstrap.Runtime;
    using Game.UI.Runtime;
    using NUnit.Framework;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEditor.AddressableAssets.Settings.GroupSchemas;
    using UnityEngine;

    public sealed class TemplateBoundaryTests
    {
        private const string CompanyName = "Amihan";
        private const string ProductName = "Unity Template";
        private const string ApplicationIdentifier = "com.amihan.unitytemplate";
        private const string BundleVersion = "0.1.0";
        private const string LocalGroup = "game.local";
        private const string InitGroup = "game.init";

        private static readonly string[] ProductDefines =
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

        private static readonly string[] ServiceRoots =
        {
            "Assets/Firebase",
            "Assets/ExternalDependencyManager",
            "Assets/GeneratedLocalRepo",
            "Assets/Editor Default Resources/Firebase",
            "Assets/Plugins/Android",
            "Assets/Plugins/iOS",
            "Assets/Plugins/tvOS"
        };

        private static readonly string[] ServiceFiles =
        {
            "Assets/google-services.json.backup",
            "Assets/StreamingAssets/google-services-desktop.json",
            "Assets/Resources/BillingMode.json",
            "ProjectSettings/AndroidResolverDependencies.xml",
            "ProjectSettings/GvhProjectSettings.xml",
            "ProjectSettings/AppLovinInternalSettings.json"
        };

        private static readonly string[] VendoredRoots =
        {
            "Assets/UnityAssetStore",
            "Assets/AlmostEngine",
            "Assets/StompyRobot",
            "Assets/Plugins",
            "Assets/Spine",
            "Assets/UnitySkipSplashScreen",
            "Assets/Editor",
            "Assets/Resources"
        };

        private static readonly string[] RemovedPackageRoots =
        {
            "Assets/ProjectSettings",
            "Assets/Packages/ExtendedNumerics.BigDecimal.3003.0.0.346",
            "Assets/Packages/LanguageExt.Core.4.4.9",
            "Assets/Packages/ObservableCollections.3.3.4",
            "Assets/Packages/ObservableCollections.R3.3.3.4"
        };

        private static readonly string[] RequiredDirectPackageTokens =
        {
            "com.cysharp.r3",
            "com.github-glitchenzo.nugetforunity",
            "com.unity.addressables",
            "com.unity.render-pipelines.universal",
            "com.unity.test-framework",
            "com.unity.ugui",
            "jp.co.cyberagent.smartaddresser"
        };

        private static readonly string[] RemovedDirectPackageTokens =
        {
            "com.benoitfreslon.vibration",
            "com.coffee.softmask-for-ugui",
            "com.coffee.ui-effect",
            "com.coffee.ui-particle",
            "com.coffee.upm-git-extension",
            "com.cysharp.unitask",
            "com.unity.cinemachine",
            "com.unity.collab-proxy",
            "com.unity.device-simulator.devices",
            "com.unity.feature.2d",
            "com.unity.feature.development",
            "com.unity.ide.rider",
            "com.unity.ide.visualstudio",
            "com.unity.inputsystem",
            "com.unity.mathematics",
            "com.unity.mobile.android-logcat",
            "com.unity.multiplayer.center",
            "com.unity.nuget.newtonsoft-json",
            "com.unity.postprocessing",
            "com.unity.recorder",
            "com.unity.timeline",
            "com.unity.visualscripting",
            "jp.hadashikick.vcontainer",
            "pl.bravasoft.unity.polly"
        };

        private static readonly string[] RemovedPreloadedAssetGuids =
        {
            "0093cf062d3a40e4f92d7b552abf271f",
            "6c7031edf1c26244e8f6e935d0327318",
            "754c89d835987084a9c79a957ecf6540"
        };

        private static readonly string[] ServiceManifestTokens =
        {
            "com.unity.purchasing",
            "com.unity.services.cloud-build",
            "com.unity.services.cloud-diagnostics",
            "com.unity.services.core",
            "AppLovin MAX Unity",
            "com.google.external-dependency-manager"
        };

        private static readonly string[] CustomAndroidTemplateSettings =
        {
            "useCustomMainManifest",
            "useCustomLauncherManifest",
            "useCustomMainGradleTemplate",
            "useCustomLauncherGradleManifest",
            "useCustomBaseGradleTemplate",
            "useCustomGradlePropertiesTemplate",
            "useCustomGradleSettingsTemplate",
            "useCustomProguardFile"
        };

        [Test]
        public void ProjectIdentity_UsesNeutralBaseline()
        {
            Assert.That(PlayerSettings.companyName, Is.EqualTo(CompanyName));
            Assert.That(PlayerSettings.productName, Is.EqualTo(ProductName));
            Assert.That(PlayerSettings.bundleVersion, Is.EqualTo(BundleVersion));
            Assert.That(PlayerSettings.Android.bundleVersionCode, Is.EqualTo(1));
            Assert.That(PlayerSettings.iOS.buildNumber, Is.EqualTo("1"));
            Assert.That(
                PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Standalone),
                Is.EqualTo(ApplicationIdentifier));
            Assert.That(
                PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android),
                Is.EqualTo(ApplicationIdentifier));
            Assert.That(
                PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.iOS),
                Is.EqualTo(ApplicationIdentifier));

            AssertNoSerializedProductDefines();
        }

        [Test]
        public void ProjectBaseline_DoesNotCarryProductBuildOrServiceConfig()
        {
            Assert.That(AssetDatabase.IsValidFolder("Assets/Settings/Build Profiles"), Is.False);

            foreach (var serviceFile in ServiceFiles)
            {
                Assert.That(File.Exists(serviceFile), Is.False, serviceFile);
            }

            foreach (var serviceRoot in ServiceRoots)
            {
                Assert.That(AssetDatabase.IsValidFolder(serviceRoot), Is.False, serviceRoot);
            }

            foreach (var vendoredRoot in VendoredRoots)
            {
                Assert.That(AssetDatabase.IsValidFolder(vendoredRoot), Is.False, vendoredRoot);
            }

            foreach (var removedPackageRoot in RemovedPackageRoots)
            {
                Assert.That(
                    AssetDatabase.IsValidFolder(removedPackageRoot),
                    Is.False,
                    removedPackageRoot);
            }

            var manifest = File.ReadAllText("Packages/manifest.json");
            var packageLock = File.ReadAllText("Packages/packages-lock.json");
            foreach (var token in ServiceManifestTokens)
            {
                Assert.That(manifest, Does.Not.Contain(token));
                Assert.That(packageLock, Does.Not.Contain(token));
            }

            foreach (var token in RequiredDirectPackageTokens)
            {
                Assert.That(manifest, Does.Contain($"\"{token}\""), token);
            }

            var nonModuleDirectPackages = Regex
                .Matches(manifest, "^ {4}\"([^\"]+)\":", RegexOptions.Multiline)
                .Cast<Match>()
                .Select(match => match.Groups[1].Value)
                .Where(packageId => !packageId.StartsWith(
                    "com.unity.modules.",
                    StringComparison.Ordinal))
                .ToArray();
            Assert.That(
                nonModuleDirectPackages,
                Is.EquivalentTo(RequiredDirectPackageTokens));

            foreach (var token in RemovedDirectPackageTokens)
            {
                Assert.That(manifest, Does.Not.Contain($"\"{token}\""), token);
            }

            var projectSettings = File.ReadAllText("ProjectSettings/ProjectSettings.asset");
            foreach (var setting in CustomAndroidTemplateSettings)
            {
                Assert.That(projectSettings, Does.Contain($"  {setting}: 0"));
            }

            Assert.That(projectSettings, Does.Contain("  activeInputHandler: 0"));

            foreach (var guid in RemovedPreloadedAssetGuids)
            {
                Assert.That(projectSettings, Does.Not.Contain(guid), guid);
            }

            var packageManagerSettings = File.ReadAllText(
                "ProjectSettings/PackageManagerSettings.asset");
            foreach (var token in ServiceManifestTokens)
            {
                Assert.That(packageManagerSettings, Does.Not.Contain(token));
            }
        }

        [Test]
        public void RuntimeAssemblies_DoNotReferenceInternalPackages()
        {
            AssertNoInternalPackageReference(typeof(TemplateBootstrap));
            AssertNoInternalPackageReference(typeof(GlobalUiShellView));
        }

        [Test]
        public void ShellPrefabs_HaveRequiredComponents()
        {
            var loading = AssertPrefabComponent<LoadingOverlayView>(
                "Assets/game.assetbundles/00_local/ui/shell/P_BootLoadingOverlay.prefab");
            AssertSerializedReferences(loading, "canvasGroup", "txtStatus", "progressFill");

            var recovery = AssertPrefabComponent<MessageOverlayView>(
                "Assets/game.assetbundles/00_local/ui/shell/P_BootRecoveryOverlay.prefab");
            AssertMessageOverlayReferences(recovery);

            var toast = AssertPrefabComponent<ToastView>(
                "Assets/game.assetbundles/01_init/ui/00_shared/P_Toast.prefab");
            AssertSerializedReferences(toast, "canvasGroup", "txtMessage");

            var message = AssertPrefabComponent<MessageOverlayView>(
                "Assets/game.assetbundles/01_init/ui/00_shared/P_MessageOverlay.prefab");
            AssertMessageOverlayReferences(message);

            var shell = AssertPrefabComponent<GlobalUiShellView>(
                "Assets/game.assetbundles/01_init/ui/00_shared/P_GlobalUiShell.prefab");
            Assert.That(shell.Toast, Is.Not.Null);
            Assert.That(shell.MessageOverlay, Is.Not.Null);
        }

        [Test]
        public void ShellAssets_AreOwnedByTierGroups()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            Assert.That(settings, Is.Not.Null);

            AssertGroup(settings, LocalGroup, "Assets/game.assetbundles/00_local/ui/shell/P_BootLoadingOverlay.prefab");
            AssertGroup(settings, LocalGroup, "Assets/game.assetbundles/00_local/ui/shell/P_BootRecoveryOverlay.prefab");
            AssertGroup(settings, InitGroup, "Assets/game.assetbundles/01_init/ui/00_shared/P_Toast.prefab");
            AssertGroup(settings, InitGroup, "Assets/game.assetbundles/01_init/ui/00_shared/P_MessageOverlay.prefab");
            AssertGroup(settings, InitGroup, "Assets/game.assetbundles/01_init/ui/00_shared/P_GlobalUiShell.prefab");
        }

        private static void AssertNoInternalPackageReference(Type type)
        {
            var references = type.Assembly.GetReferencedAssemblies();
            var hasInternalReference = references.Any(reference =>
                reference.Name.StartsWith("com.paul", StringComparison.OrdinalIgnoreCase) ||
                reference.Name.StartsWith("paul.", StringComparison.OrdinalIgnoreCase));

            Assert.That(
                hasInternalReference,
                Is.False,
                $"{type.Assembly.GetName().Name} references an internal package.");
        }

        private static void AssertNoSerializedProductDefines()
        {
            var settings = File.ReadAllLines("ProjectSettings/ProjectSettings.asset");
            var start = Array.FindIndex(settings, line => line == "  scriptingDefineSymbols:");
            var end = Array.FindIndex(settings, line => line == "  additionalCompilerArguments: {}");
            Assert.That(start, Is.GreaterThanOrEqualTo(0));
            Assert.That(end, Is.GreaterThan(start));

            var serializedDefines = settings
                .Skip(start + 1)
                .Take(end - start - 1)
                .SelectMany(line => line[(line.IndexOf(':') + 1)..]
                    .Split(';', StringSplitOptions.RemoveEmptyEntries))
                .Select(symbol => symbol.Trim())
                .ToArray();
            foreach (var productDefine in ProductDefines)
            {
                Assert.That(serializedDefines, Does.Not.Contain(productDefine));
            }
        }

        private static T AssertPrefabComponent<T>(string path) where T : Component
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.That(prefab, Is.Not.Null, $"Missing prefab: {path}");
            var component = prefab.GetComponentInChildren<T>(true);
            Assert.That(component, Is.Not.Null, $"Missing {typeof(T).Name}: {path}");
            return component;
        }

        private static void AssertMessageOverlayReferences(MessageOverlayView view)
        {
            AssertSerializedReferences(
                view,
                "canvasGroup",
                "txtTitle",
                "txtMessage",
                "btnPrimary",
                "txtPrimary",
                "btnSecondary",
                "txtSecondary");
        }

        private static void AssertSerializedReferences(UnityEngine.Object target, params string[] propertyNames)
        {
            var serializedObject = new SerializedObject(target);
            foreach (var propertyName in propertyNames)
            {
                var property = serializedObject.FindProperty(propertyName);
                Assert.That(property, Is.Not.Null, $"Missing serialized property: {propertyName}");
                Assert.That(property.objectReferenceValue, Is.Not.Null, $"Unwired serialized property: {propertyName}");
            }
        }

        private static void AssertGroup(AddressableAssetSettings settings, string groupName, string path)
        {
            var group = settings.FindGroup(groupName);
            Assert.That(group, Is.Not.Null, $"Missing Addressables group: {groupName}");
            Assert.That(group.GetSchema<BundledAssetGroupSchema>(), Is.Not.Null, $"Missing bundled schema: {groupName}");

            var entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(path));
            Assert.That(entry, Is.Not.Null, $"Missing Addressables entry: {path}");
            Assert.That(entry.parentGroup, Is.EqualTo(group));
        }
    }
}
