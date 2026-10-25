# Package Exit Dependency Inventory

Date: 2026-10-25

This is the C2 deletion map. It records the current package boundary before any
consumer, Addressables group, manifest entry, or gitlink is removed.

## Disposition Vocabulary

- `KEEP-REWRITE`: preserve the template capability or asset GUID, but replace
  its package-backed implementation.
- `DELETE-LEGACY`: remove Jigsaw-specific code or content from the template.
- `PROJECT-ADDON`: remove it from the template; a game can add the capability
  later with an explicit package and feature owner.

There are no `unknown` consumers in this inventory.

## Pinned Package Boundary

| Root | Commit | Package GUIDs scanned | Disposition |
| --- | --- | ---: | --- |
| `Packages/game-packages` | `beb0af11ad9bce57382362e4a99dfc8a0870dfc3` | 823 | Remove after C4 reaches zero consumers. |
| `Packages/vfx-packages` | `3f87b5936a897337ce4b1c35fbff2434037e86c9` | 600 | `PROJECT-ADDON`; remove after C4 reaches zero consumers. |

`Packages/manifest.json` mounts seven local packages: common compile service,
core, singletons, game shared, game visualization, VFX shared, and VFX
third-party. All seven are removed in C5. Networking is not mounted.

## Assembly References

Four legacy assemblies contain 18 references to eight package assemblies. The
two `game.core` assemblies contain zero package references.

| Package assembly | Reference count | Consumer assemblies | Disposition |
| --- | ---: | --- | --- |
| `com.paul.common.compileservice` | 2 | shared runtime; visualization runtime | `DELETE-LEGACY` |
| `com.paul.common.core` | 3 | shared runtime; visualization runtime/editor | `DELETE-LEGACY` |
| `com.paul.common.core.data` | 3 | shared runtime/editor; visualization runtime | `DELETE-LEGACY`; `game.core` already owns the replacement DT/DA primitives. |
| `com.paul.common.singletons` | 2 | shared runtime; visualization runtime | `DELETE-LEGACY` |
| `com.paul.game.shared.runtime` | 4 | all four legacy assemblies | `DELETE-LEGACY` |
| `com.paul.game.visualization.runtime` | 2 | visualization runtime/editor | `DELETE-LEGACY` |
| `com.paul.vfx.thirdparty.runtime` | 1 | visualization runtime | `PROJECT-ADDON` |
| `paul.shared.editor` | 1 | shared editor | `KEEP-REWRITE` the Addressables capability without the package assembly. |

The replacement `game.bootstrap` and `game.ui` assemblies created in C3 must
not reference any of these assemblies.

## C# Consumers

The boundary scan found 87 C# files: 33 under `Assets/game.shared`, 54 under
`Assets/game.visualization`, and zero under `Assets/game.core`. It scans both
`using Paul.*` and fully qualified or namespace-level `Paul.*` occurrences; the
assembly table above is the authoritative compile dependency list.

### KEEP-REWRITE (4)

- `Assets/game.shared/Editor/Addressables/GameAddressableBootstrapper.cs`
- `Assets/game.shared/Editor/Addressables/SmartAddresserStageLayoutSetup.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/GameBootstrapper.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/LifeTimeScope/LobbyLifeTimeScope.cs`

These files preserve four capabilities only: deterministic Addressables setup,
layout application, neutral startup, and one composition root. Their current
Jigsaw/package implementations are replaced; package source is not copied.

### PROJECT-ADDON (15)

- `Assets/game.shared/Runtime/Scripts/Business/Tracking/KawaiiAnalyticsEvents.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Gateway/Api/ApiUserProfileGateway.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Gateway/LocalStorage/LocalSettingGateway.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Gateway/LocalStorage/LocalUserProfileGateway.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Provider/Analytic/AppsflyerAnalyticsProvider.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Provider/Analytic/FirebaseAnalyticsProvider.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Service/AppServices.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Service/AppsflyerSdkService.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Service/FirebaseSdkService.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Service/GameSettingService.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Service/Mocks/MockAdsService.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Service/Mocks/MockAnalyticsService.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Service/UserProfileService.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Tracking/AnalyticsEventHandler.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/Tracking/LegacyAnalyticsBridgeService.cs`

Authentication, API, profile persistence, ads, analytics, Firebase, and
AppsFlyer are not template boot requirements. A project adds each capability
with its first real feature.

### DELETE-LEGACY (68)

- `Assets/game.shared/Editor/Addressables/StageAndHintAddressProviderAsset.cs`
- `Assets/game.shared/Editor/Tools/ClearDataTool.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/GameData/ChapterDataTable.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/GameData/DailyRewardDataTable.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/GameData/GeneralConfigDataTable.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/GameData/IconResourceDataTable.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/GameData/InitCurrencyDataTable.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/GameData/ItemRarityDataTable.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/GameData/QuestDataTable.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/GameData/QuestMilestoneDataTable.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/GameData/ShopDataTable.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/GameData/StageDataTable.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/GameData/TimeAdsDataTable.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/GameData/VisualDataTable.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/GameMasterDataWrapper.cs`
- `Assets/game.shared/Runtime/Scripts/Framework/GameMasterData/Popup/MessageDataTable.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Chapter/Chapter/ChapterBodyPresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Chapter/Chapter/ChapterItemCellView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/ComingSoon/ComingSoonUIView/ComingSoonUIPresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Components/TopHeader/TopHeaderPresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Components/TopHeader/TopHeaderView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Gameplay/GameplayHeaderView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Gameplay/GameplayUIManager.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/AppRating/PopupAppRatingData.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/AppRating/PopupAppRatingPresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/AppRating/PopupAppRatingView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/ChapterUnlock/PopupChapterUnlockData.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/ChapterUnlock/PopupChapterUnlockPresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/ChapterUnlock/PopupChapterUnlockView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Gameplay/BuyTime/PopupBuyTimeData.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Gameplay/BuyTime/PopupBuyTimePresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Gameplay/BuyTime/PopupBuyTimeView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Gameplay/EndGame/PopupEndGameData.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Gameplay/EndGame/PopupEndGamePresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Gameplay/EndGame/PopupEndGameView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Gameplay/Hint/PopupHintData.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Gameplay/Hint/PopupHintPresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Gameplay/Hint/PopupHintView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Gameplay/Tutorial/PopupTutorialData.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Gameplay/Tutorial/PopupTutorialPresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Gameplay/Tutorial/PopupTutorialView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Monetization/NoAds/PopupNoAdsData.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Monetization/NoAds/PopupNoAdsPresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Monetization/NoAds/PopupNoAdsView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Pause/PopupPauseData.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Pause/PopupPausePresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Popups/Pause/PopupPauseView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Quest/QuestUIView/QuestBodyView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Shop/CellView/ShopBegginerCellBinder.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Shop/CellView/ShopBegginerCellView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Shop/CellView/ShopGoldBundleRowCellBinder.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Shop/CellView/ShopGoldBundleRowCellView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Shop/CellView/ShopNoAdCellBinder.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Shop/CellView/ShopNoAdCellView.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Shop/ShopBodyPresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Business/UI/Shop/ShopBodyView.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/TestTool/Lobby/LobbyTestCheatTool.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/TestTool/Lobby/LobbyTestToolRegistration.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/UI/FooterNavigation/FooterTabEventListener.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/UI/HUD/GameTimerView.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/UI/Lobby/ChapterSlider/ChapterPageData.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/UI/Lobby/ChapterSlider/ChapterSliderPresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/UI/Lobby/ChapterSlider/ChapterSliderView.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/UI/Lobby/LobbyBodyPresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/UI/Lobby/LobbyBodyView.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/UI/Lobby/LobbyPresenter.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/UI/Lobby/TestTool/LobbyTestCheatRegistration.cs`
- `Assets/game.visualization/Runtime/Scripts/Framework/UI/Lobby/TestTool/LobbyTestCheatTool.cs`

Toast, MessageOverlay, and loading remain target capabilities, but their legacy
package-derived code and prefab variants are not retained. C3 creates neutral
template-owned replacements under the Addressables root.

## Serialized GUID Scan

The scan indexed all 1,423 package `.meta` GUIDs, then scanned 7,511 serialized
files under `Assets/` for `guid`, `m_GUID`, and `m_AssetGUID`. It found 1,104
occurrences from 56 source files to 311 distinct package GUIDs:

| Target package | Occurrences | Source files | Distinct target GUIDs |
| --- | ---: | ---: | ---: |
| `paul.game.visualization` | 822 | 36 | 44 |
| `com.paul.vfx.thirdparty` | 257 | 3 | 256 |
| `paul.game.shared` | 15 | 15 | 6 |
| `com.paul.common.core` | 5 | 3 | 4 |
| `com.paul.vfx.shared` | 5 | 5 | 1 |

The 262 Addressables occurrences and 842 other serialized occurrences are all
assigned below.

### KEEP-REWRITE sources

| Occurrences | Unique package GUIDs | Source | Required action |
| ---: | ---: | --- | --- |
| 2 | 2 | `Assets/AddressableAssetsData/AddressableAssetSettings.asset` | Preserve settings GUID; remove package group registrations in C4. |
| 3 | 3 | `Assets/AddressableAssetsData/AssetLayoutRules/game.bundles_LayoutRuleData.asset` | Replace legacy/package targets with the single tier root rules. |
| 2 | 2 | `Assets/AddressableAssetsData/AssetLayoutRules/LayoutRuleData.asset` | Preserve project rule asset GUID; remove package targets. |
| 3 | 3 | `Assets/game.contents/gameScenes/00_LoadingScene.unity` | Preserve scene GUID; replace three package components with neutral bootstrap/UI components. |

### PROJECT-ADDON sources

| Occurrences | Unique package GUIDs | Source | Required action |
| ---: | ---: | --- | --- |
| 255 | 255 | `Assets/AddressableAssetsData/AssetGroups/com.vfx.third_party.asset` | Remove the group from the template; a VFX add-on owns it later. |
| 1 | 1 | `Assets/game.shared/Editor/Configs/AudioImportSettings.asset` | Remove package postprocessor config. |
| 1 | 1 | `Assets/game.shared/Editor/Configs/MaterialImportSettings.asset` | Remove package postprocessor config. |
| 1 | 1 | `Assets/game.shared/Editor/Configs/NamingConventionSettings.asset` | Remove package postprocessor config. |
| 1 | 1 | `Assets/game.shared/Editor/Configs/TextureImportSettings.asset` | Remove package postprocessor config. |

### DELETE-LEGACY sources

Every source below is deleted with its owning Jigsaw vertical slice. Counts are
occurrences / distinct package GUIDs.

| Count | Source |
| ---: | --- |
| 2 / 2 | `Assets/game.art/LayoutRuleData.asset` |
| 24 / 5 | `Assets/game.contents.gameplay/Runtime/Contents/001/game/gameScenes/02_GameScene.unity` |
| 1 / 1 each | `Assets/game.contents.gameplay/Runtime/Contents/common/game/sounds/DA_Common_Combo_1.asset`, `Assets/game.contents.gameplay/Runtime/Contents/common/game/sounds/DA_Common_Combo_2.asset`, `Assets/game.contents.gameplay/Runtime/Contents/common/game/sounds/DA_Common_Combo_3.asset`, `Assets/game.contents.gameplay/Runtime/Contents/common/game/sounds/DA_Common_Correct.asset`, `Assets/game.contents.gameplay/Runtime/Contents/common/game/sounds/DA_Common_Lose.asset`, `Assets/game.contents.gameplay/Runtime/Contents/common/game/sounds/DA_Common_UIButton.asset`, `Assets/game.contents.gameplay/Runtime/Contents/common/game/sounds/DA_Common_Victory.asset` |
| 1 / 1 | `Assets/game.contents.gameplay/Runtime/Contents/common/game/sounds/P_GamePlaySoundController.prefab` |
| 1 / 1 | `Assets/game.contents/Data/MasterData/GSheetImporterConfig.asset` |
| 1 / 1 | `Assets/game.contents/LifeTimeScope/P_LobbyLifeTimeScope.prefab` |
| 1 / 1 each | `Assets/game.contents/sounds/bgm/DA_Lobby_Bgm.asset`, `Assets/game.contents/sounds/sfx/DA_Common_UIButton.asset`, `Assets/game.contents/sounds/sfx/DA_Common_UISelect.asset` |
| 1 / 1 | `Assets/game.contents/sounds/P_LobbySoundController.prefab` |
| 14 / 1 | `Assets/game.contents/sounds/SoundManager Variant.prefab` |
| 51 / 2 | `Assets/game.contents/ui/_Common/FooterNav/P_FooterNavigationBar_Variant.prefab` |
| 53 / 1 | `Assets/game.contents/ui/_Common/FooterNav/P_FooterNavItemView_Variant.prefab` |
| 12 / 6 | `Assets/game.contents/ui/_Common/HUD/P_LobbyHeader.prefab` |
| 22 / 1 | `Assets/game.contents/ui/_Common/IconLoading/P_IconLoading_Variant.prefab` |
| 1 / 1 each | `Assets/game.contents/ui/_Common/Loading/DT_LoadingTextInfoDataTable.asset`, `Assets/game.contents/ui/_Common/Loading/DT_LoadingTooltipDataTable.asset` |
| 114 / 3 | `Assets/game.contents/ui/_Common/Loading/P_Loading_Variant.prefab` |
| 22 / 1 each | `Assets/game.contents/ui/_Common/Loading/P_LoadingTooltip Variant.prefab`, `Assets/game.contents/ui/_Common/Loading/P_TransitionEffect_Variant.prefab`, `Assets/game.contents/ui/_Common/Loading/P_TransitionEffectIrisOut_Variant.prefab` |
| 22 / 1 | `Assets/game.contents/ui/_Common/Notifcation/P_NotifyIcon_Variant.prefab` |
| 25 / 1 | `Assets/game.contents/ui/_Common/PageSelector/P_EventSelector_Variant.prefab` |
| 47 / 1 | `Assets/game.contents/ui/_Common/Settings/P_SettingToggle_Variant.prefab` |
| 27 / 1 | `Assets/game.contents/ui/_Common/Toast/P_Toast_Variant.prefab` |
| 26 / 1 | `Assets/game.contents/ui/ComingSoon/P_ComingSoonUIView_Variant.prefab` |
| 1 / 1 | `Assets/game.contents/ui/DA_UISetProfileDataAsset.asset` |
| 33 / 1 | `Assets/game.contents/ui/Lobby/BgIdle/P_BackgroundIdle_Variant.prefab` |
| 9 / 6 | `Assets/game.contents/ui/Lobby/Chapter/P_SliderStageView.prefab` |
| 8 / 5 | `Assets/game.contents/ui/Lobby/P_LobbyUIView.prefab` |
| 3 / 3 | `Assets/game.contents/ui/Lobby/Stage/P_StageItemView.prefab` |
| 27 / 1 | `Assets/game.contents/ui/P_CommonUI_Variant.prefab` |
| 13 / 1 | `Assets/game.contents/ui/P_LobbyMainUINavigator_Variant.prefab` |
| 10 / 8 | `Assets/game.contents/ui/Popup/_Gameplay/Pause/P_PopupPause.prefab` |
| 43 / 1 | `Assets/game.contents/ui/Popup/AppRating/P_PopupPopupAppRating_Variant.prefab` |
| 5 / 3 | `Assets/game.contents/ui/Popup/ChapterUnlock/P_PopupChapterUnlock.prefab` |
| 1 / 1 | `Assets/game.contents/ui/Popup/DT_PopupTemplateDataTable 1.asset` |
| 38 / 1 | `Assets/game.contents/ui/Popup/Message/P_PopupMessage_Variant.prefab` |
| 24 / 1 | `Assets/game.contents/ui/Popup/P_PopupManager_Variant.prefab` |
| 97 / 4 | `Assets/game.contents/ui/Popup/Setting/P_PopupSetting_Variant.prefab` |
| 2 / 2 | `Assets/game.visualization/Runtime/Addressable/SmartAddresserLayoutRuleData/sharedResource_LayoutRuleData.asset` |

## Addressables Baseline

`python -I tools/addressables/check.py --static` exited 0:

- 8 registered groups
- 0 errors
- 495 warnings
- 488 entries outside `Assets/game.assetbundles/`
- 255 entries in `com.vfx.third_party`
- 12 entries in `com.paul.game.visualization.common`
- four registered groups without a bundled schema
- two registered package group assets plus three unregistered group assets

A fresh Addressables content build was attempted in Unity `6000.3.10f1` with
build-layout generation enabled. It failed before producing a report because
the `com.vfx.third_party` entry for
`Packages/com.paul.vfx.thirdparty/Runtime/ThirdParty/HoloDissolveFX/Shaders/Graphs/Dissolve/Transition/Square.shadergraph`
cannot be imported as a supported asset type. The failing GUID is
`3c0d5f6a750d0af418ec829d48071145`.

This is a known `PROJECT-ADDON` consumer, not an unknown dependency. Do not edit
the pinned VFX package to manufacture a green baseline. C4 removes that group;
the first successful report and report diff are therefore deferred to C4/C6.

## C2 Gate

The dependency/GUID ownership gate passes: every current code, assembly,
serialized asset, group, and package reference has an owner and disposition.
The build-report task remains visibly deferred because the current VFX package
entry prevents report generation; static proof is green and the failure is
reproducible.

## C4 Outcome

C4 executed the recorded dispositions without editing either pinned package.
The Jigsaw-owned roots and project-addon Addressables groups were removed, then
all 1,423 package asset GUIDs were scanned again against `Assets/`: zero
serialized consumers remain. Only `game.local` and `game.init` are registered;
the first valid post-removal build report passes `check.py` and `audit.py` with
2 groups, 3 bundles, and no errors or warnings. The missing historical C2 build
report remains documented as a deliberate deferral rather than reconstructed
after the fact.
