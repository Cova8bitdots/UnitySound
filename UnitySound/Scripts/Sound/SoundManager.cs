using System.Threading;
using UnityEngine;
using UnityEngine.Audio;

using Cysharp.Threading.Tasks;

using CovaTech.Lib;
using CovaTech.Extentions;

namespace CovaTech.UnitySound
{
    /// <summary>
    /// サウンド管理クラス
    /// </summary>
    public class SoundManager : MonoBehaviour, IAudioPlayer, IVolumeController, IMixerEffectController
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

        private SoundAssetLoader m_assetLoader =null;
        
        private IMixerEffectController m_effectCtrl = null;
        #endregion //) ===== MEMBER_VARIABLES =====
        

        //-----------------------------------------------------
        // 初期化
        //-----------------------------------------------------
        #region ===== INITIALIZE =====
        /// <summary>
        /// 初期化メソッド
        /// ※Awake/Start だと実行順管理がしづらいので外部メソッド経由で叩かせる
        /// </summary>
        public void Initialize()
        {
            InitModules();
        }
        private bool InitModules()
        {
            Debug.Assert(m_mixer != null );
            if( m_mixer == null )
            {
                return false;
            }
            m_volumeCtrl = new VolumeController( m_mixer );
            m_effectCtrl = new AudioMixerEffectController( m_mixer);

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


            m_assetLoader = new SoundAssetLoader();

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
        /// <param name="_param">BGM 再生に必要なパラメータ</param>
        /// <param name="_token">キャンセル用トークン</param>
        /// <returns>ハンドラ</returns>
        async UniTask<int> IBgmPlayer.PlayBgm( BgmParam _param, CancellationToken _token)
        {
            IBgmPlayer player = this;
            Debug.Assert( _param.IsValid() );
            if( !_param.IsValid() )
            {
                return SoundConsts.INVALID_HANDLER; 
            }

            int handler = await player.PrewarmBGM( _param, _token);
            return player.PlayBgm( handler, _param.Volume);
        }

        /// <summary>
        /// Prewarm 完了後に呼び出すBGM 再生API
        /// </summary>
        /// <param name="_handler">Item のHandler</param>
        /// <param name="_volume">音量 [0 1]</param>
        /// <returns>ハンドラ</returns>
        int IBgmPlayer.PlayBgm( int _handler, float _volume)
        {
            if( _handler == SoundConsts.INVALID_HANDLER )
            {
                return _handler;
            }
            var item = m_bgmPool.GetItem( _handler );
            Debug.Assert(item != null);
            if (item == null)
            {
                return SoundConsts.INVALID_HANDLER;
            }

            item.Play(_volume);
            return _handler;
        }

        /// <summary>
        /// 指定BGM を停止
        /// </summary>
        /// <param name="_handler">ハンドラID</param>
        /// <param name="_isForceStop">強制停止フラグ</param>
        /// <returns></returns>
        async UniTask IBgmPlayer.StopBGM ( int _handler, bool _isForceStop, CancellationToken _token)
        {
            Debug.Assert( m_bgmPool != null );
            if( m_bgmPool == null )
            {
                return;
            }
            var item = m_bgmPool.GetItem( _handler );
            Debug.Assert(item != null);
            if (item == null)
            {
                return;
            }
            await item.Stop(_token:_token, _isForceStop:_isForceStop );
        }

        /// <summary>
        /// SoundItem で再生準備フェーズまでの事前準備を行う
        /// </summary>
        /// <param name="_param">BGM 再生に必要なパラメータ</param>
        /// <param name="_token"></param>
        /// <returns></returns>
        async UniTask<int> IBgmPlayer.PrewarmBGM( BgmParam _param, CancellationToken _token)
        {
            Debug.Assert( m_assetLoader != null );
            Debug.Assert( m_bgmPool != null );
            if( m_assetLoader == null || m_bgmPool == null )
            {
                return SoundConsts.INVALID_HANDLER;
            }
            Debug.Assert( _param.IsValid() );
            if( !_param.IsValid() )
            {
                return SoundConsts.INVALID_HANDLER; 
            }

            AudioClip clip = await m_assetLoader.LoadBgmClip( _param.BgmId, _token);
            Debug.Assert( clip != null );
            if( _token.IsCancellationRequested || clip == null )
            {
                return SoundConsts.INVALID_HANDLER;
            }

            var item = m_bgmPool.Rent();
            Debug.Assert(item != null);
            if (item == null)
            {
                return SoundConsts.INVALID_HANDLER;
            }
            SOUND_CATEGORY category = GetBgmCategory(_param.BgmId);
            bool isReady = item.SetParam(clip, _clipId:_param.BgmId, category, GetMixerGroup(category), _param.IsLoop);
            if (!isReady)
            {
                item.SetDisable();
                return SoundConsts.INVALID_HANDLER;
            }

            item.SetPosition( _param.Position );
            return item.GetHandler();
        }

        protected virtual SOUND_CATEGORY GetBgmCategory( int _bgmId )
        {
            //TODO: 条件に応じて実装する
            if( _bgmId < 1000 )
            {
                return SOUND_CATEGORY.SYSTEM_BGM;
            }
            else
            {
                return SOUND_CATEGORY.DIEGETIC_BGM;
            }
        }
        #endregion //) ===== IBgmPlayer =====


        //------------------------------------------------------------------
        // ISePlayer
        //------------------------------------------------------------------
        #region  ===== ISePlayer =====
        /// <summary>
        /// SE再生
        /// </summary>
        /// <param name="_param">再生に必要なパラメータ</param>
        /// <param name="_isLoop">Is LoopSound?</param>
        /// <returns></returns>
        async UniTask<int> ISePlayer.PlaySe( SeParam _param, CancellationToken _token)
        {
            ISePlayer player = this;
            Debug.Assert( _param.IsValid() );
            if( !_param.IsValid() )
            {
                return SoundConsts.INVALID_HANDLER; 
            }

            if( m_sePool == null )
            {
                return SoundConsts.INVALID_HANDLER;
            }
            int handler = await player.PrewarmSE( _param, _token);
            return player.PlaySe( handler, _param.Volume);
        }

        /// <summary>
        /// Prewarm 完了後に呼び出すSE 再生API
        /// </summary>
        /// <param name="_handler">Item のHandler</param>
        /// <param name="_volume">音量 [0 1]</param>
        /// <returns>ハンドラ</returns>
        int ISePlayer.PlaySe( int _handler, float _volume)
        {
            if( _handler == SoundConsts.INVALID_HANDLER )
            {
                return _handler;
            }
            var item = m_sePool.GetItem( _handler );
            Debug.Assert(item != null);
            if (item == null)
            {
                return SoundConsts.INVALID_HANDLER;
            }

            item.Play( _volume );
            return _handler;
        }

        /// <summary>
        /// SE再生(OneShot)
        /// 
        /// </summary>
        /// <param name="_seId">SEID</param>
        /// <param name="_volume">volume [0, 1]</param>
        /// <returns></returns>
        void ISePlayer.PlayOneShotSE( int _seId, float _volume)
        {
            var param = new SeParam();
            param.SetId( _seId);
            param.SetVolume( _volume);
            (this as ISePlayer).PlaySe(param, new CancellationToken() ).Forget( e => Debug.LogError( e.Message));
        }

        /// <summary>
        /// 指定SEを停止
        /// </summary>
        /// <param name="_handler">ハンドラID</param>
        /// <param name="_isForceStop">強制停止フラグ</param>
        /// <returns></returns>
        async UniTask ISePlayer.StopSE( int _handler, bool _isForceStop, CancellationToken _token)
        {
            Debug.Assert( m_sePool != null );
            if( m_sePool == null )
            {
                return;
            }
            var item = m_sePool.GetItem( _handler );
            Debug.Assert( item != null );
            if( item == null )
            {
                return;
            }
            await item.Stop(_token:_token, _isForceStop:_isForceStop );

        }

        /// <summary>
        /// SoundItem で再生準備フェーズまでの事前準備を行う
        /// </summary>
        /// <param name="_param">再生に必要なパラメータ</param>
        /// <param name="_token"></param>
        /// <returns></returns>
        async UniTask<int> ISePlayer.PrewarmSE( SeParam _param, CancellationToken _token)
        {
            Debug.Assert( m_assetLoader != null );
            Debug.Assert( m_sePool != null );
            if( m_assetLoader == null || m_sePool == null )
            {
                return SoundConsts.INVALID_HANDLER;
            }
            Debug.Assert( _param.IsValid() );
            if( !_param.IsValid() )
            {
                return SoundConsts.INVALID_HANDLER; 
            }

            AudioClip clip = await m_assetLoader.LoadSeClip( _param.SeId, _token);
            Debug.Assert( clip != null );
            if( _token.IsCancellationRequested || clip == null )
            {
                return SoundConsts.INVALID_HANDLER;
            }

            var item = m_sePool.Rent();
            Debug.Assert(item != null);
            if (item == null)
            {
                return SoundConsts.INVALID_HANDLER;
            }
            SOUND_CATEGORY category = GetSeCategory(_param.SeId);
            bool isReady = item.SetParam(clip, _clipId:_param.SeId, category, GetMixerGroup(category), _param.IsLoop);
            if (!isReady)
            {
                item.SetDisable();
                return SoundConsts.INVALID_HANDLER;
            }

            item.SetPosition( _param.Position );

            return item.GetHandler();
        }


        /// <summary>
        /// IDに基づいてカテゴリを返却
        /// </summary>
        /// <param name="_seId"></param>
        /// <returns></returns>
        protected virtual SOUND_CATEGORY GetSeCategory( int _seId )
        {
            //TODO: 条件に応じて実装する
            if( _seId < 1000 )
            {
                return SOUND_CATEGORY.SYSTEM_SE;
            }
            else if( _seId < 2000)
            {
                return SOUND_CATEGORY.DIEGETIC_SE;
            }
            else if( _seId < 3000)
            {
                return SOUND_CATEGORY.ENVIROMENT_SE;
            }
            else
            {
                return SOUND_CATEGORY.JINGLE;
            }
        }
        #endregion //) ===== ISePlayer =====

        /// <summary>
        /// 指定カテゴリに対応したMixerGroupを返すそn
        /// </summary>
        /// <param name="_category"></param>
        /// <returns></returns>
        private AudioMixerGroup GetMixerGroup( SOUND_CATEGORY _category )
        {
            if( m_mixerGroups.IsNullOrEmpty()|| m_mixerGroups.Length <= (int)_category)
            {
                return null;
            }
            return m_mixerGroups[(int)_category];
        }

        //------------------------------------------------------------------
        // IMixerEffectController
        //------------------------------------------------------------------
        #region  ===== IMixerEffectController =====

        /// <summary>
        /// LPF のカットオフ周波数設定
        /// </summary>
        /// <param name="_category">対象カテゴリ</param>
        /// <param name="normalizedFreq">正規化周波数</param>
        void IMixerEffectController.SetLowPassFilter(SOUND_CATEGORY _category, float normalizedFreq )
        {
            m_effectCtrl?.SetLowPassFilter(_category, normalizedFreq);
        }

        /// <summary>
        /// HPF のカットオフ周波数設定
        /// </summary>
        /// <param name="_category">対象カテゴリ</param>
        /// <param name="normalizedFreq">正規化周波数</param>
        void IMixerEffectController.SetHighPassFilter(SOUND_CATEGORY _category,  float normalizedFreq )
        {
            m_effectCtrl?.SetHighPassFilter(_category, normalizedFreq);
        }

        #endregion //) ===== IMixerEffectController =====
    }
}
