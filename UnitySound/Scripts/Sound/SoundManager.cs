using UnityEngine;
using UnityEngine.Audio;
using Cysharp.Threading.Tasks;

namespace CovaTech.UnitySound
{
    /// <summary>
    /// サウンド管理クラス
    /// </summary>
    public class SoundManager : MonoBehaviour, IAudioPlayer, IVolumeController
    {
        //------------------------------------------------------------------
        // 定数関連
        //------------------------------------------------------------------
        #region  ===== CONSTS =====
        private static readonly string BGM_ITEM_NAME_PREFIX = "BGM_";
        private static readonly string SE_ITEM_NAME_PREFIX = "SE_";
        private static readonly string VOICE_ITEM_NAME_PREFIX = "VOICE_";

        #endregion //) ===== CONSTS =====

        //------------------------------------------------------------------
        // メンバ変数関連
        //------------------------------------------------------------------
        #region  ===== MEMBER_VARIABLES =====

        [Header("AudioMixer設定")]
        [SerializeField]
        private AudioMixer m_mixer = null;
        [SerializeField, EnumLabel(typeof(SOUND_CATEGORY))]
        private AudioMixerGroup[] m_mixerGroups = null;

        [Header("サウンドItem")]
        [SerializeField, Tooltip("オブジェクトプールで管理するアイテム")]
        private SoundItem m_soundItemsPrefab = null;

        /* 各モジュール */
        private IVolumeController m_volumeCtrl = null;
        private SoundObjectPool m_bgmPool = null;
        private SoundObjectPool m_sePool = null;
        private SoundObjectPool m_voicePool = null;

        
        #endregion //) ===== MEMBER_VARIABLES =====
        

        //-----------------------------------------------------
        // 初期化
        //-----------------------------------------------------
        #region ===== INITIALIZE =====

        private bool InitModule()
        {
            Debug.Assert(m_mixer != null );
            if( m_mixer == null )
            {
                return false;
            }
            m_volumeCtrl = new VolumeController( m_mixer );

            Debug.Assert(m_soundItemsPrefab != null );
            if( m_soundItemsPrefab == null )
            {
                return false;
            }
            m_bgmPool = new SoundObjectPool();
            m_bgmPool.Init( m_soundItemsPrefab, this.transform, BGM_ITEM_NAME_PREFIX, SoundConsts.DEFAULT_BGM_POOL_SIZE );
            m_sePool = new SoundObjectPool();
            m_sePool.Init( m_soundItemsPrefab, this.transform, SE_ITEM_NAME_PREFIX, SoundConsts.DEFAULT_SE_POOL_SIZE );
            m_voicePool = new SoundObjectPool();
            m_voicePool.Init( m_soundItemsPrefab, this.transform, VOICE_ITEM_NAME_PREFIX, SoundConsts.DEFAULT_VOICE_POOL_SIZE );
            return true;
        }

        #endregion //) ===== INITIALIZE =====


        //-----------------------------------------------------
        // IVolumeController 実装
        //-----------------------------------------------------
        #region ===== IVolumeController =====

        /// <summary>
        /// 指定カテゴリの音量を更新
        /// </summary>
        /// <param name="_category"></param>
        /// <param name="_volume"></param>
        void IVolumeController.UpdateVolume( SOUND_CATEGORY _category, float _volume)
        {
            m_volumeCtrl?.UpdateVolume( _category, _volume );
        }

        /// <summary>
        /// 指定オーディオの音量を返す
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        float IVolumeController.GetVolume( SOUND_CATEGORY _category)
        {
            if( m_volumeCtrl == null )
            {
                return SoundConsts.DEFAULT_VOLUME;
            }
            return m_volumeCtrl.GetVolume( _category );
        }

        #endregion //) ===== IVolumeController =====


        //------------------------------------------------------------------
        // IBgmPlayer 
        //------------------------------------------------------------------
        #region  ===== IBgmPlayer =====
        /// <summary>
        /// BGM 再生
        /// </summary>
        /// <param name="_bgmId">BGM ID</param>
        /// <param name="_volume">volume [0, 1]</param>
        /// <param name="_isLoop">Is LoopSound?</param>
        /// <returns></returns>
        async UniTask<int> IBgmPlayer.PlayBgm( int _bgmId, float _volume, bool _isLoop )
        {
            return SoundConsts.INVALID_HANDLER;
        }

        /// <summary>
        /// 指定BGM を停止
        /// </summary>
        /// <param name="_handler">ハンドラID</param>
        /// <param name="_isForceStop">強制停止フラグ</param>
        /// <returns></returns>
        async UniTask IBgmPlayer.StopBGM ( int _handler, bool _isForceStop)
        {

        }
        #endregion //) ===== IBgmPlayer =====


        //------------------------------------------------------------------
        // ISePlayer
        //------------------------------------------------------------------
        #region  ===== ISePlayer =====
        /// <summary>
        /// SE再生
        /// </summary>
        /// <param name="_seId">SEID</param>
        /// <param name="_volume">volume [0, 1]</param>
        /// <param name="_isLoop">Is LoopSound?</param>
        /// <returns></returns>
        async UniTask<int> ISePlayer.PlaySe( int _seId, float _volume, bool _isLoop )
        {
            return SoundConsts.INVALID_HANDLER;
        }

        /// <summary>
        /// SE再生(OneShot)
        /// 
        /// </summary>
        /// <param name="_seId">SEID</param>
        /// <param name="_volume">volume [0, 1]</param>
        /// <returns></returns>
        int ISePlayer.PlayOneShotSE( int _seId, float _volume)
        {
            return SoundConsts.INVALID_HANDLER;

        }

        /// <summary>
        /// 指定SEを停止
        /// </summary>
        /// <param name="_handler">ハンドラID</param>
        /// <param name="_isForceStop">強制停止フラグ</param>
        /// <returns></returns>
        async UniTask ISePlayer.StopSE( int _handler, bool _isForceStop)
        {
        }
        #endregion //) ===== ISePlayer =====
    }
}
