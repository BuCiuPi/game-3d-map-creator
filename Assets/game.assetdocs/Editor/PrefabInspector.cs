using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.AssetDocs.Editor
{
    /// <summary>
    /// Batch entry for asset documentation: renders a preview of every prefab under a pack's
    /// Prefabs/ folder and measures its LOD0 bounds, geometry and dominant colors.
    /// Run: Unity.exe -batchmode -projectPath . -executeMethod Game.AssetDocs.Editor.PrefabInspector.Run
    ///      -packRoot "Assets/.../Pack" -out "docs/assets/Pack" -metrics "Temp/metrics.json"
    /// </summary>
    public static class PrefabInspector
    {
        private const int ImageSize = 384;
        private const string RootCategory = "Rooms";
        private static readonly Color Background = new Color(0.82f, 0.84f, 0.86f);

        [Serializable]
        private class PrefabMetrics
        {
            public string guid;
            public string path;
            public float sizeX, sizeY, sizeZ;
            public float pivotToBottom;
            public float centerOffsetX, centerOffsetZ;
            public int triangles, vertices, renderers;
            public string averageColor;
            public string[] colors;
            public float[] colorShares;
            public string image;
        }

        [Serializable]
        private class MetricsFile
        {
            public List<PrefabMetrics> prefabs = new List<PrefabMetrics>();
        }

        public static void Run()
        {
            // Run replaces the open scene and quits; never let an interactive Editor call it.
            if (!Application.isBatchMode)
            {
                throw new InvalidOperationException("PrefabInspector.Run is batch-mode only.");
            }

            var allowAsyncCompilation = ShaderUtil.allowAsyncCompilation;
            try
            {
                var packRoot = Argument("-packRoot").TrimEnd('/');
                var outRoot = Path.GetFullPath(Argument("-out"));
                var metricsPath = Path.GetFullPath(Argument("-metrics"));
                var prefabFolder = packRoot + "/Prefabs";

                // An empty result would make the generator overwrite every card, so fail instead.
                if (!AssetDatabase.IsValidFolder(prefabFolder))
                {
                    throw new ArgumentException($"No Prefabs folder at '{prefabFolder}'.");
                }

                var paths = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolder })
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .OrderBy(p => p, StringComparer.Ordinal)
                    .ToList();
                if (paths.Count == 0)
                {
                    throw new ArgumentException($"No prefabs under '{prefabFolder}'.");
                }

                // Async compilation draws placeholder cyan into previews on a cold shader cache.
                ShaderUtil.allowAsyncCompilation = false;
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var camera = CreateStage();
                var target = new RenderTexture(ImageSize, ImageSize, 24) { antiAliasing = 4 };
                camera.targetTexture = target;
                var result = new MetricsFile();
                foreach (var path in paths)
                {
                    result.prefabs.Add(Inspect(path, prefabFolder + "/", outRoot, camera, target));
                }

                File.WriteAllText(metricsPath, JsonUtility.ToJson(result, true));
                Debug.unityLogger.Log($"PrefabInspector: {result.prefabs.Count} prefabs -> {metricsPath}");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.unityLogger.LogException(exception);
                EditorApplication.Exit(1);
            }
            finally
            {
                ShaderUtil.allowAsyncCompilation = allowAsyncCompilation;
            }
        }

        private static PrefabMetrics Inspect(string path, string prefabRoot, string outRoot, Camera camera,
            RenderTexture target)
        {
            var relative = path.Substring(prefabRoot.Length);
            var metrics = new PrefabMetrics
            {
                guid = AssetDatabase.AssetPathToGUID(path),
                path = relative,
                colors = Array.Empty<string>(),
                colorShares = Array.Empty<float>(),
            };

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));
            try
            {
                instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                var visible = VisibleRenderers(instance);
                metrics.renderers = visible.Count;
                if (visible.Count == 0)
                {
                    return metrics;
                }

                var bounds = visible[0].bounds;
                foreach (var renderer in visible)
                {
                    bounds.Encapsulate(renderer.bounds);
                }

                metrics.sizeX = bounds.size.x;
                metrics.sizeY = bounds.size.y;
                metrics.sizeZ = bounds.size.z;
                metrics.pivotToBottom = bounds.min.y;
                metrics.centerOffsetX = bounds.center.x;
                metrics.centerOffsetZ = bounds.center.z;
                CountGeometry(visible, metrics);

                Frame(camera, bounds);
                var pixels = Render(camera, target, out var png);
                SummarizeColors(pixels, metrics);

                var imageRelative = ImagePath(relative);
                var imageFull = Path.Combine(outRoot, imageRelative);
                Directory.CreateDirectory(Path.GetDirectoryName(imageFull));
                File.WriteAllBytes(imageFull, png);
                metrics.image = imageRelative.Replace('\\', '/');
                return metrics;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }

        private static string ImagePath(string relative)
        {
            var withoutExtension = relative.Substring(0, relative.Length - ".prefab".Length);
            return relative.Contains('/') ? withoutExtension + ".png" : RootCategory + "/" + withoutExtension + ".png";
        }

        // Only LOD0 renderers count, so size and triangles describe the closest view.
        private static List<Renderer> VisibleRenderers(GameObject instance)
        {
            var lowerLods = new HashSet<Renderer>();
            var lodZero = new HashSet<Renderer>();
            foreach (var group in instance.GetComponentsInChildren<LODGroup>(true))
            {
                group.ForceLOD(0);
                var lods = group.GetLODs();
                if (lods.Length > 0)
                {
                    lodZero.UnionWith(lods[0].renderers.Where(r => r != null));
                }

                foreach (var lod in lods.Skip(1))
                {
                    lowerLods.UnionWith(lod.renderers.Where(r => r != null));
                }
            }

            // A renderer shared between LOD0 and a lower LOD is drawn at LOD0, so it counts.
            lowerLods.ExceptWith(lodZero);

            return instance.GetComponentsInChildren<Renderer>()
                .Where(r => r.enabled && !lowerLods.Contains(r) && !(r is ParticleSystemRenderer))
                .ToList();
        }

        private static void CountGeometry(List<Renderer> renderers, PrefabMetrics metrics)
        {
            foreach (var renderer in renderers)
            {
                Mesh mesh = null;
                if (renderer is SkinnedMeshRenderer skinned)
                {
                    mesh = skinned.sharedMesh;
                }
                else if (renderer.TryGetComponent<MeshFilter>(out var filter))
                {
                    mesh = filter.sharedMesh;
                }

                if (mesh == null)
                {
                    continue;
                }

                metrics.vertices += mesh.vertexCount;
                for (var i = 0; i < mesh.subMeshCount; i++)
                {
                    metrics.triangles += (int)(mesh.GetIndexCount(i) / 3);
                }
            }
        }

        private static Camera CreateStage()
        {
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.75f, 0.78f, 0.82f);
            RenderSettings.ambientEquatorColor = new Color(0.55f, 0.55f, 0.55f);
            RenderSettings.ambientGroundColor = new Color(0.35f, 0.33f, 0.3f);
            RenderSettings.fog = false;

            var light = new GameObject("Key Light").AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.shadows = LightShadows.Soft;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var camera = new GameObject("Preview Camera").AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Background;
            camera.fieldOfView = 30f;
            return camera;
        }

        private static void Frame(Camera camera, Bounds bounds)
        {
            var radius = Mathf.Max(bounds.extents.magnitude, 0.01f);
            var distance = radius / Mathf.Sin(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 1.05f;
            var rotation = Quaternion.Euler(25f, -135f, 0f);
            camera.transform.SetPositionAndRotation(bounds.center - rotation * Vector3.forward * distance, rotation);
            camera.nearClipPlane = Mathf.Max(distance - radius * 2f, 0.01f);
            camera.farClipPlane = distance + radius * 2f;
        }

        private static Color32[] Render(Camera camera, RenderTexture target, out byte[] png)
        {
            camera.Render();
            var previous = RenderTexture.active;
            RenderTexture.active = target;
            var texture = new Texture2D(ImageSize, ImageSize, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, ImageSize, ImageSize), 0, 0);
            texture.Apply();
            RenderTexture.active = previous;
            png = texture.EncodeToPNG();
            var pixels = texture.GetPixels32();
            UnityEngine.Object.DestroyImmediate(texture);
            return pixels;
        }

        // Rendered pixels include lighting, so these are apparent colors, not raw albedo.
        private static void SummarizeColors(Color32[] pixels, PrefabMetrics metrics)
        {
            Color32 background = Background;
            var bins = new Dictionary<int, (int count, long r, long g, long b)>();
            long sumR = 0, sumG = 0, sumB = 0;
            var total = 0;
            foreach (var p in pixels)
            {
                if (Math.Abs(p.r - background.r) + Math.Abs(p.g - background.g) + Math.Abs(p.b - background.b) < 12)
                {
                    continue;
                }

                total++;
                sumR += p.r;
                sumG += p.g;
                sumB += p.b;
                var key = (p.r >> 5 << 6) | (p.g >> 5 << 3) | (p.b >> 5);
                bins.TryGetValue(key, out var bin);
                bins[key] = (bin.count + 1, bin.r + p.r, bin.g + p.g, bin.b + p.b);
            }

            if (total == 0)
            {
                return;
            }

            metrics.averageColor = Hex(sumR / total, sumG / total, sumB / total);
            var top = bins.Values.OrderByDescending(b => b.count).Take(3).ToList();
            metrics.colors = top.Select(b => Hex(b.r / b.count, b.g / b.count, b.b / b.count)).ToArray();
            metrics.colorShares = top.Select(b => (float)Math.Round((double)b.count / total, 2)).ToArray();
        }

        private static string Hex(long r, long g, long b) => $"#{r:X2}{g:X2}{b:X2}";

        private static string Argument(string name)
        {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, name);
            if (index < 0 || index + 1 >= args.Length)
            {
                throw new ArgumentException($"Missing command-line argument {name} <value>.");
            }

            return args[index + 1];
        }
    }
}
