using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace SoundEventLink.Editor
{
    public static class AddressableUtil
    {
        #region 定義

        //設定用のパス 
        private const string SettingPath = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
        private static readonly AddressableAssetSettings settings;
        private static (string groupName, AddressableAssetGroup group) _lastGroupCache;

        #endregion //定義

        static AddressableUtil()
        {
            settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(SettingPath);
        }

        /// <summary>
        /// UnityObjectがAddressableに含まれていたらaddressを返す
        /// </summary>
        /// <param name="obj">UnityObject Texture2d Prefab等</param>
        /// <returns>address</returns>
        public static string ConvertAssetPath(Object obj)
        {
            var assetPath = AssetDatabase.GetAssetPath(obj);
            if (assetPath == null)
            {
                Debug.LogError($"{obj} はUnityObjectではありません");
                return "";
            }

            var entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(assetPath));
            if (entry != null)
                return entry.address;
            Debug.LogError($"Addressableに登録されてません {assetPath}");
            return "";
        }

        /// <summary>
        /// Addressableに追加する
        /// </summary>
        public static string AddAssetToGroup<TObject>(TObject obj, string groupName) where TObject : Object
        {
            var path = AssetDatabase.GetAssetPath(obj);
            if (path == null)
            {
                Debug.LogError($"[Addressable.AddAsset] {obj.name}はパスが取得出来ません");
                return null;
            }

            //アセットGUIDを取得
            var assetGuid = AssetDatabase.AssetPathToGUID(path);
            AddAssetToGroup(assetGuid, path, groupName);
            return path;
        }

        /// <summary>
        /// アセットグループの中身をリセットする
        /// </summary>
        /// <param name="groupName"></param>
        public static void ResetGroup(string groupName)
        {
            var group = CreateGroup(groupName);
            foreach (var entry in group.entries.ToList()) @group.RemoveAssetEntry(entry);
        }

        /// <summary>
        /// アセットを指定のグループに追加する
        /// </summary>
        /// <param name="assetGuid"></param>
        /// <param name="address"></param>
        /// <param name="groupName"></param>
        public static void AddAssetToGroup(string assetGuid, string address, string groupName)
        {
            if (address != null)
            {
                //グループを検索orなければ新規作成
                var lgroup = CreateGroup(groupName);
                //新規作成
                var lentry = settings.CreateOrMoveEntry(assetGuid, lgroup);
                lentry?.SetAddress(address);
            }
        }

        /// <summary>
        /// アセットをAddressableから削除する
        /// </summary>
        public static void RemoveAssetToGroupPath(string path)
        {
            var assetGuid = AssetDatabase.AssetPathToGUID(path);
            RemoveAssetToGroup(assetGuid);
        }

        /// <summary>
        /// アセットをAddressableから削除する
        /// </summary>
        public static void RemoveAssetToGroup(Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);

            //アセットGUIDを取得
            var assetGuid = AssetDatabase.AssetPathToGUID(path);
            RemoveAssetToGroup(assetGuid);
        }

        /// <summary>
        /// アセットをAddressableから削除する
        /// </summary>
        public static void RemoveAssetToGroup(string assetGuid)
        {
            settings.RemoveAssetEntry(assetGuid);
        }

        /// <summary>
        /// グループを作成
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private static AddressableAssetGroup CreateGroup(string groupName)
        {
            // 何度も同じGroupに対して処理をすることが多いのでキャッシュしておく
            if (_lastGroupCache.groupName == groupName)
                return _lastGroupCache.group;

            //グループが存在しなければ作成する
            var group = settings.groups.Find(g => g.name == groupName);
            if (group == null)
            {
                //スキーマ生成
                var schema = new List<AddressableAssetGroupSchema>()
                {
                    ScriptableObject.CreateInstance<BundledAssetGroupSchema>(),
                    ScriptableObject.CreateInstance<ContentUpdateGroupSchema>()
                };
                group = settings.CreateGroup(groupName, false, false, true, schema);
            }

            _lastGroupCache = (groupName, group);
            return group;
        }
    }
}