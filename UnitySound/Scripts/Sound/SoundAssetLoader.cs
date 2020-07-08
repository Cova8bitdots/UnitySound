using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.AddressableAssets;

using Cysharp.Threading.Tasks;

namespace CovaTech.UnitySound
{
    /// <summary>
    /// サウンドのアセットロード/アンロード管理クラス
    /// </summary>
    public class SoundAssetLoader : System.IDisposable
    {
        //------------------------------------------------------------------
        // 定数関連
        //------------------------------------------------------------------
        #region  ===== CONSTS =====

        private static readonly string SE_ASSET_PREFIX = "se_";
        private static readonly string BGM_ASSET_PREFIX = "bgm_";
        private static readonly string VOICE_ASSET_PREFIX = "voice_";

        #endregion //) ===== CONSTS =====

        //------------------------------------------------------------------
        // メンバ変数関連
        //------------------------------------------------------------------
        #region  ===== MEMBER_VARIABLES =====
       
        private ILoader<AudioClip> m_bgmLoader = null;
        private ILoader<AudioClip> m_seLoader = null;
        private ILoader<AudioClip> m_voiceLoader = null;
        
        #endregion //) ===== MEMBER_VARIABLES =====
        

        //-----------------------------------------------------
        // 初期化
        //-----------------------------------------------------
        #region ===== INITIALIZE =====

        public SoundAssetLoader( )
        {
            m_bgmLoader = new AudioClipLoader();
            m_seLoader = new AudioClipLoader();
            m_voiceLoader = new AudioClipLoader();
        }

        #endregion //) ===== INITIALIZE =====

        /// <summary>
        /// BGMアセットのロード
        /// </summary>
        /// <param name="_bgmId"></param>
        /// <param name="_token"></param>
        /// <returns></returns>
        public async UniTask<AudioClip> LoadBgmClip( int _bgmId, CancellationToken _token)
        {
            return await m_bgmLoader.LoadAsset( BGM_ASSET_PREFIX+_bgmId.ToString("D5"), _token);
        }

        /// <summary>
        /// SEアセットのロード
        /// </summary>
        /// <param name="_seId"></param>
        /// <param name="_token"></param>
        /// <returns></returns>
        public async UniTask<AudioClip> LoadSeClip( int _seId, CancellationToken _token)
        {
            return await m_seLoader.LoadAsset( SE_ASSET_PREFIX+_seId.ToString("D5"), _token);
        }


        /// <summary>
        /// BGMアセットのロード
        /// </summary>
        /// <param name="_voiceId"></param>
        /// <param name="_token"></param>
        /// <returns></returns>
        public async UniTask<AudioClip> LoadVoiceClip( int _voiceId, CancellationToken _token)
        {
            return await m_voiceLoader.LoadAsset( VOICE_ASSET_PREFIX+_voiceId.ToString("D5"), _token);
        }

        //-----------------------------------------------------
        // 破棄処理
        //-----------------------------------------------------
        #region ===== DISPOSE =====

        public void Dispose( )
        {
            m_bgmLoader?.Dispose();
            m_seLoader?.Dispose();
            m_voiceLoader?.Dispose();
        }

        #endregion //) ===== DISPOSE =====
    }
}
