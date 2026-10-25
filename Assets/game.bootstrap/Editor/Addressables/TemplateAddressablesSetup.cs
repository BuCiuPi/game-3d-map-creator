namespace Game.Bootstrap.Editor.Addressables
{
    using System;
    using System.Linq;
    using SmartAddresser.Editor.Core.Models.LayoutRules;
    using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
    using SmartAddresser.Editor.Core.Models.Services;
    using SmartAddresser.Editor.Core.Models.Shared;
    using SmartAddresser.Editor.Core.Models.Shared.AssetGroups;
    using SmartAddresser.Editor.Core.Models.Shared.AssetGroups.AssetFilterImpl;
    using SmartAddresser.Editor.Core.Tools.Shared;
    using SmartAddresser.Editor.Foundation.AddressableAdapter;
    using SmartAddresser.Editor.Foundation.AssetDatabaseAdapter;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEditor.AddressableAssets.Settings.GroupSchemas;
    using UnityEngine;

    public static class TemplateAddressablesSetup
    {
        #region Configurations

        public const string LocalGroupName = "game.local";
        public const string InitGroupName = "game.init";

        private const string LocalRoot = "Assets/game.assetbundles/00_local";
        private const string InitRoot = "Assets/game.assetbundles/01_init";
        private const string RuleDataPath =
            "Assets/AddressableAssetsData/AssetLayoutRules/template_LayoutRuleData.asset";

        #endregion Configurations

        #region Methods

        [MenuItem("Game Template/Addressables/Apply Template Layout", priority = 20)]
        public static void ApplyTemplateLayout()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                throw new InvalidOperationException("Addressable Asset Settings not found.");
            }

            var localGroup = EnsureGroup(settings, LocalGroupName);
            var initGroup = EnsureGroup(settings, InitGroupName);
            settings.DefaultGroup = localGroup;

            var ruleData = LoadOrCreateRuleData();
            var layoutRule = ruleData.LayoutRule ?? new LayoutRule();
            ruleData.LayoutRule = layoutRule;
            layoutRule.SyncAddressRulesWithAddressableAssetGroups(
                settings.groups.Where(group => group != null).ToList());

            ConfigureTier(layoutRule, localGroup, LocalRoot);
            ConfigureTier(layoutRule, initGroup, InitRoot);
            layoutRule.LabelRules.Clear();
            layoutRule.VersionRules.Clear();

            var applyService = new ApplyLayoutRuleService(
                layoutRule,
                new VersionExpressionParserRepository().Load(),
                new AddressableAssetSettingsAdapter(settings),
                new AssetDatabaseAdapter());
            applyService.ApplyAll(true);

            settings.SetDirty(
                AddressableAssetSettings.ModificationEvent.BatchModification,
                null,
                true,
                true);
            EditorUtility.SetDirty(ruleData);
            AssetDatabase.SaveAssets();
            Debug.Log("[Template Addressables] Template layout applied.");
        }

        [MenuItem("Game Template/Addressables/Build Content", priority = 21)]
        public static void BuildContent()
        {
            ApplyTemplateLayout();
            ProjectConfigData.GenerateBuildLayout = true;
            AddressableAssetSettings.BuildPlayerContent(out var result);
            if (!string.IsNullOrEmpty(result.Error))
            {
                throw new InvalidOperationException(
                    $"Addressables content build failed: {result.Error}");
            }

            Debug.Log("[Template Addressables] Content build completed.");
        }

        #endregion Methods

        #region Subroutines

        private static AddressableAssetGroup EnsureGroup(
            AddressableAssetSettings settings,
            string groupName)
        {
            var group = settings.FindGroup(groupName) ?? settings.CreateGroup(
                groupName,
                false,
                false,
                true,
                null,
                typeof(BundledAssetGroupSchema),
                typeof(ContentUpdateGroupSchema));

            var bundledSchema = group.GetSchema<BundledAssetGroupSchema>() ??
                                group.AddSchema<BundledAssetGroupSchema>();
            bundledSchema.BuildPath.SetVariableByName(
                settings,
                AddressableAssetSettings.kLocalBuildPath);
            bundledSchema.LoadPath.SetVariableByName(
                settings,
                AddressableAssetSettings.kLocalLoadPath);
            bundledSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            bundledSchema.IncludeInBuild = true;

            if (group.GetSchema<ContentUpdateGroupSchema>() == null)
            {
                group.AddSchema<ContentUpdateGroupSchema>();
            }

            EditorUtility.SetDirty(group);
            EditorUtility.SetDirty(bundledSchema);
            return group;
        }

        private static LayoutRuleData LoadOrCreateRuleData()
        {
            var ruleData = AssetDatabase.LoadAssetAtPath<LayoutRuleData>(RuleDataPath);
            if (ruleData != null)
            {
                return ruleData;
            }

            ruleData = ScriptableObject.CreateInstance<LayoutRuleData>();
            AssetDatabase.CreateAsset(ruleData, RuleDataPath);
            return ruleData;
        }

        private static void ConfigureTier(
            LayoutRule layoutRule,
            AddressableAssetGroup addressableGroup,
            string folderPath)
        {
            var addressRule = layoutRule.AddressRules.FirstOrDefault(
                rule => rule.AddressableGroup == addressableGroup);
            if (addressRule == null)
            {
                throw new InvalidOperationException(
                    $"No SmartAddresser rule exists for {addressableGroup.Name}.");
            }

            var folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderPath);
            if (folder == null)
            {
                throw new InvalidOperationException($"Addressables tier folder not found: {folderPath}");
            }

            var tierAssets = addressRule.AssetGroups.Count > 0
                ? addressRule.AssetGroups[0]
                : new AssetGroup();
            tierAssets.Name.Value = addressableGroup.Name;
            var filter = tierAssets.Filters.Count == 1
                ? tierAssets.Filters[0] as ObjectBasedAssetFilter
                : null;
            if (filter == null)
            {
                filter = new ObjectBasedAssetFilter();
                tierAssets.Filters.Clear();
                tierAssets.Filters.Add(filter);
            }

            filter.FolderTargetingMode = FolderTargetingMode.IncludedNonFolderAssets;
            filter.Object.IsListMode = true;
            filter.Object.ClearValues();
            filter.Object.AddValue(folder);

            addressRule.Control.Value = true;
            addressRule.AssetGroups.Clear();
            addressRule.AssetGroups.Add(tierAssets);
            var addressProvider = addressRule.AddressProvider.Value as AssetPathBasedAddressProvider;
            if (addressProvider == null)
            {
                addressProvider = new AssetPathBasedAddressProvider();
                addressRule.AddressProvider.Value = addressProvider;
            }

            addressProvider.Source = PartialAssetPathType.AssetPath;
            addressProvider.ReplaceWithRegex = false;
            addressProvider.Pattern = string.Empty;
            addressProvider.Replacement = string.Empty;
        }

        #endregion Subroutines
    }
}
