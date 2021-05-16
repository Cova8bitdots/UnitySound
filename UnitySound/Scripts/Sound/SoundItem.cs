using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.Audio;

using Cysharp.Threading.Tasks;
using UniRx;

namespace CovaTech.UnitySound
{
    using CovaTech.Lib;

    [RequireComponent(typeof(AudioSource))]
    public class SoundItem : MonoBehaviour, IPoolItem
    {

        //-----------------------------------------------------
        // 定数
        //-----------------------------------------------------
        #region ===== CONST =====

        public const float DEFAULT_FADEOUT_TIME = 0.3f; // FO 時間[sec]

        #endregion //)===== CONST =====

        //-----------------------------------------------------
        // メンバ変数
        //-----------------------------------------------------
        #region ===== MEMBER_VARIABLES =====
        [SerializeField, Tooltip("音源再生AudioSource")]
        private AudioSource m_source = null;

        /// <summary>
        /// オブジェクト名
        /// </summary>
        /// <value></value>
        public string objectName
        {
            get{return this.name;}
            set{ this.name = value;}
        }

        private SOUND_CATEGORY m_audioType = SOUND_CATEGORY.UNDEFINED;
        public SOUND_CATEGORY AudioType => m_audioType;

        // Item管理用ID
        private int m_itemId = 0;

        private int m_currentClipId = SoundConsts.INVALID_CLIP_ID;
        /// <summary>
        /// 現在設定しているClip情報
        /// </summary>
        public int CurrentClipId => m_currentClipId;
        
        // このアイテムの管理人
        private SoundObjectPool m_poolManager = null;

        public float CurrentVolume => CurrentState == SOUND_STATE.STOP ? 0.0f : m_source.volume;
        public float ClipLength => m_source?.clip?.length ?? 0.0f;


        private ReactiveProperty<SOUND_STATE> m_currentState = new ReactiveProperty<SOUND_STATE>( SOUND_STATE.UNDEFINED);
        public SOUND_STATE CurrentState => m_currentState.Value;
        public IObservable<SOUND_STATE> onChangeCurrentState => m_currentState;

        private ReactiveProperty<SOUND_STATE> m_prevState = new ReactiveProperty<SOUND_STATE>(SOUND_STATE.UNDEFINED);
        public SOUND_STATE PrevState => m_prevState.Value;

        private bool m_isFading = false;
        public bool IsFading => m_isFading;

        public bool IsPlaying => m_source.isPlaying;

        /* Pause判定 */
        private bool m_isPaused = false;
        public bool IsPaused => m_isPaused;

        private CompositeDisposable m_disposable = new CompositeDisposable();
        #endregion //) ===== MEMBER_VARIABLES =====


        //-----------------------------------------------------
        // 初期化
        //-----------------------------------------------------
        #region ===== INITIALIZE =====
        public void Init(int _itemId, SoundObjectPool _poolManager)
        {
            SetNextState(SOUND_STATE.INIT);
            if( m_source == null )
            {
                m_source = GetComponent<AudioSource>();
            }
            m_itemId = _itemId;
            m_poolManager = _poolManager;


            m_source.playOnAwake = false;
            m_source.loop = false;
            m_source.volume = 0.0f;
            m_source.pitch = 1.0f;

            // 3D Sound
            m_source.spatialBlend = 1.0f;

            SetNextState(SOUND_STATE.IDLE);
        }

        public bool SetParam(AudioClip _clip, int _clipId, SOUND_CATEGORY _type, AudioMixerGroup _mixerGroup, bool _isLoop = false)
        {
            Debug.Assert(_clip != null);
            if( _clip == null )
            {
                return false;
            }
            if(Is2DSound(_type) )
            {
                m_source.spatialBlend = 0.0f;
            }
            else
            {
                m_source.spatialBlend = 1.0f;
            }
            m_currentClipId = _clipId;
            m_source.clip = _clip;
            m_source.loop = _isLoop;
            m_source.volume = 0.0f;
            m_source.outputAudioMixerGroup = _mixerGroup;
            m_audioType = _type;
            SetNextState(SOUND_STATE.PRE_PLAY);

            return true;
        }

        /// <summary>
        /// Objectの位置を設定(World座標)
        /// </summary>
        /// <param name="_position"></param>
        public void SetPosition( Vector3 _position)
        {
            this.transform.position = _position;
        }

        private bool Is2DSound(SOUND_CATEGORY _type)
        {
            switch( _type )
            {
                case SOUND_CATEGORY.SYSTEM_BGM:
                case SOUND_CATEGORY.SYSTEM_SE:
                case SOUND_CATEGORY.SYSTEM_VOICE:
                    {
                        return true;
                    }
                default: return false;
            }
        }
        #endregion //) ===== INITIALIZE =====


        //-----------------------------------------------------
        // サウンド処理
        //-----------------------------------------------------
        #region ===== SOUND_ITEM =====

        /// <summary>
        /// Stateの更新
        /// </summary>
        /// <param name="_nextState"></param>
        protected void SetNextState(SOUND_STATE _nextState)
        {
            m_prevState.Value = m_currentState.Value;
            m_currentState.Value = _nextState;
        }

        public void Play( float _volume, float _fadeDuration = 0.0f)
        {
            if( CurrentState != SOUND_STATE.PRE_PLAY)
            {
                Debug.LogError("Is NOT ready for play");
                return;
            }
            SetNextState(SOUND_STATE.PLAYING);
            
            // 再生要求
            InternalPlay(_volume, _fadeDuration);
            
            // OneShot の場合は終了をチェックする
            if (!m_source.loop)
            {
                m_disposable?.Clear();
                Observable.EveryUpdate()
                .Where(_ => !IsPaused && !IsPlaying )
                .Subscribe(_ =>
                {
                    Stop(_isForceStop: true, _token:this.GetCancellationTokenOnDestroy()).Forget(e => Debug.LogError(e.Message));
                }).AddTo(m_disposable);
            }
        }
        private void InternalPlay(float _volume, float _fadeDuration = 0.0f)
        {
            if (m_source == null)
            {
                return;
            }
            bool isFadeIn = _fadeDuration > 0.0f;
            m_source.volume = isFadeIn ? 0.0f : _volume;
            m_source.Play();

            if (isFadeIn)
            {
                FadeIn(_fadeDuration,0.0f, _volume, this.GetCancellationTokenOnDestroy()).Forget(e => Debug.LogError(e.Message));
            }
        }
        /// <summary>
        /// 再生停止
        /// </summary>
        /// <param name="_isForceStop">強制停止フラグ</param>
        /// <param name="_token">Cancel 用Token</param>
        /// <param name="_fadeOutDuration">停止までのFadeout時間[sec]</param>
        public async UniTask Stop( bool _isForceStop,  CancellationToken _token, float _fadeOutDuration = DEFAULT_FADEOUT_TIME)
        {
            if( _isForceStop)
            {
                m_source?.Stop();
                SetNextState(SOUND_STATE.STOP);
                m_disposable.Clear();
                m_poolManager?.Return(this);
                return;
            }

            SetNextState(SOUND_STATE.PRE_STOP);
            await FadeOut(_fadeOutDuration, CurrentVolume, 0.0f, _token);
            SetNextState(SOUND_STATE.STOP);
            m_poolManager?.Return(this);
            m_disposable.Clear();
        }

        public async UniTask FadeIn( float _duration, float _from, float _to,  CancellationToken _token)
        {
            if( m_source == null )
            {
                return;
            }
            float from = Mathf.Clamp01(Mathf.Min(_from, _to));
            float to = Mathf.Clamp01(Mathf.Max(_from, _to));

            await FadeCoroutine(_duration, from, to, _token);
        }


        public async UniTask FadeOut(float _duration, float _from, float _to,  CancellationToken _token)
        {
            if (m_source == null)
            {
                return;
            }
            float from = Mathf.Clamp01(Mathf.Max(_from, _to));
            float to = Mathf.Clamp01(Mathf.Min(_from, _to));
            if (_duration <= 0.0f)
            {
                m_source.volume = to;
                return;
            }

            try
            {
                await FadeCoroutine(_duration, from, to, _token);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }
        }

        protected async UniTask FadeCoroutine( float _duration, float _from, float _to,  CancellationToken _token)
        {
            if( m_source == null )
            {
                return;
            }
            if (_duration <= 0.0f)
            {
                m_source.volume = _to;
                return;
            }
            m_isFading = true;
            for (float t = 0; t < _duration; t+= Time.deltaTime)
            {
                m_source.volume = Mathf.Lerp(_from, _to, t / _duration);
                await UniTask.DelayFrame(1, cancellationToken:_token);
            }
            m_source.volume = _to;
            m_isFading = false;
        }

        /// <summary>
        /// 音量を更新
        /// </summary>
        /// <param name="_volume"></param>
        public void UpdateVolume( float _volume)
        {
            m_source.volume = _volume;
        }
        #endregion //) ===== SOUND_ITEM =====

        //-----------------------------------------------------
        // ObjectPool Item 処理
        //-----------------------------------------------------
        #region ===== IPoolItem =====

        /// <summary>
        /// ハンドラ(idだったり) を返すメソッド
        /// </summary>
        /// <returns></returns>
        public int GetHandler() { return m_itemId; }

        /// <summary>
        /// 使用中判定メソッド
        /// </summary>
        /// <returns></returns>
        public bool IsUsed() { return CurrentState == SOUND_STATE.PRE_PLAY || CurrentState == SOUND_STATE.PLAYING || CurrentState == SOUND_STATE.PRE_STOP ; }

        /// <summary>
        /// Queueに存在するかどうか
        /// </summary>
        public bool InQueue => m_poolManager?.InQueue(this) ?? false;

        /// <summary>
        /// Item の有効化
        /// </summary>
        /// <returns>有効化に成功したかどうか</returns>
        public bool SetEnable()
        {
            SetNextState(SOUND_STATE.PRE_PLAY);
            return true;
        }
        /// <summary>
        /// Item の無効化
        /// </summary>
        /// <param name="_forceDisable">強制無効化フラグ</param>
        /// <returns>無効化に成功したかどうか</returns>
        public bool SetDisable(bool _forceDisable = false)
        {
            if( !IsUsed() )
            {
                return false;
            }
            if( !_forceDisable && IsUsed() )
            {
                return false;
            }
            Stop(_isForceStop: _forceDisable, _token: new CancellationToken() ).Forget(e => Debug.LogError(e.Message)) ;

            return true;
        }
        #endregion //) ===== IObjectPoolItem =====

        //-----------------------------------------------------
        // 終了処理
        //-----------------------------------------------------
        #region ===== DISPOSE =====
        private void OnDestroy()
        {
            Dispose();
        }
        public void Dispose()
        {
            SetDisable(_forceDisable: true);
            m_source.clip = null;
        }
        #endregion //) ===== DISPOSE =====
    }
}