using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Cysharp.Threading.Tasks;


namespace CovaTech.UnitySound
{
    /// <summary>
    /// AudioClipロード/アンロード管理クラス
    /// </summary>
    public class AudioClipLoader : ILoader<AudioClip>
    {
        //------------------------------------------------------------------
        // 定数関連
        //------------------------------------------------------------------
        #region  ===== CONSTS =====

        #endregion //) ===== CONSTS =====

        //------------------------------------------------------------------
        // メンバ変数関連
        //------------------------------------------------------------------
        #region  ===== MEMBER_VARIABLES =====

        /// <summary>
        /// 現在ロード中のアセットKeyの集合
        /// </summary>
        private HashSet<int> m_currentLoadKeySet = null;

        /// <summary>
        /// ロード済みAudioClip格納変数
        /// </summary>
        private Dictionary<int, AudioClip> m_cache = null;
        #endregion //) ===== MEMBER_VARIABLES =====
        

        //-----------------------------------------------------
        // 初期化
        //-----------------------------------------------------
        #region ===== INITIALIZE =====

        public AudioClipLoader( )
        {
            m_currentLoadKeySet = new HashSet<int>();
            m_cache = new Dictionary<int, AudioClip>(SoundConsts.DEFAULT_CACHE_SIZE);
        }

        #endregion //) ===== INITIALIZE =====


        //-----------------------------------------------------
        // ILoader
        //-----------------------------------------------------
        #region ===== LOADER =====

        /// <summary>
        /// AssetLoad処理
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_token"></param>
        /// <returns></returns>
        async UniTask<AudioClip> ILoader<AudioClip>.LoadAsset( string _key, CancellationToken _token)
        {
            int dicKey = _key.GetHashCode();
            AudioClip clip = null;
            // キャッシュ変数上に存在するかチェック
            if (m_cache.TryGetValue(dicKey, out clip))
            {
                return clip;
            }

            // メモリ上になければ実際のロードを行う
            if( m_currentLoadKeySet.Contains(dicKey))
            {
                // Load 中のため待機
                await WaitUntilLoadComplete(m_cache, dicKey);
                return m_cache[dicKey];
            }
            else
            {
                m_currentLoadKeySet.Add(dicKey);
                clip = await LoadAudioClipAddressables(_key, _token);
                if (clip != null)
                {
                    m_cache.Add(dicKey, clip);
                }
                m_currentLoadKeySet.Remove(dicKey);

                return clip;
            }
        }

        #endregion //) ===== LOADER =====

        /// <summary>
        ///  Addressanles からロード
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_token"></param>
        /// <returns></returns>
        private async UniTask<AudioClip> LoadAudioClipAddressables( string _key, CancellationToken _token )
        {
            AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(_key);
            return await handle.Task;
        }

        /// <summary>
        /// 対象のキャッシュにロード結果が保存されるまで待機
        /// </summary>
        /// <param name="_dict"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        private IEnumerator WaitUntilLoadComplete(IReadOnlyDictionary<int, AudioClip> _dict , int _key)
        {
            while( !_dict.ContainsKey(_key))
            {
                yield return null;
            }
        }

        //-----------------------------------------------------
        // 破棄処理
        //-----------------------------------------------------
        #region ===== DISPOSE =====

        public void Dispose( )
        {
            m_currentLoadKeySet?.Clear();
            m_cache?.Clear();
        }

        #endregion //) ===== DISPOSE =====
    }
}
