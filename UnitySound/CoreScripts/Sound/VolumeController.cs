using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CovaTech.UnitySound
{
    /// <summary>
    /// 音量制御ユニット
    /// </summary>
    public class VolumeController : IVolumeController
    {
        //-----------------------------------------------------
        // 定数
        //-----------------------------------------------------
        #region ===== CONST =====

        /* AudioMixer で設定したパラメータ名 */
        public readonly string MASTER_VOLUME                = "MASTER_VOLUME";
        // BGM
        public readonly string BGM_MASTER_VOLUME            = "BGM_MASTER_VOLUME";
        public readonly string BGM_SYSTEM_VOLUME            = "BGM_SYSTEM_VOLUME";
        public readonly string BGM_DIEGETIC_VOLUME          = "BGM_DIEGETIC_VOLUME";

        // SE
        public readonly string SE_MASTER_VOLUME             = "SE_MASTER_VOLUME";
        public readonly string SE_SYSTEM_VOLUME             = "SE_SYSTEM_VOLUME";
        public readonly string SE_JINGLE_VOLUME             = "JINGLE_VOLUME";
        public readonly string SE_ENVIROMENT_VOLUME         = "SE_ENVIROMENT_VOLUME";
        public readonly string SE_DIEGETIC_VOLUME           = "SE_DIEGETIC_VOLUME";

        // Voice 
        public readonly string VOICE_MASTER_VOLUME          = "VOICE_MASTER_VOLUME";
        public readonly string VOICE_SYSTEM_VOLUME          = "VOICE_SYSTEM_VOLUME";

        public const float MIN_VOLUME_DEG = 1e-8f;
        public const float MAX_VOLUME_DEG = 100f;
        public const float MIN_VOLUME_DB = -80f;
        public const float MAX_VOLUME_DB = 20f;
        #endregion //) ===== CONST =====

        //-----------------------------------------------------
        // メンバ変数
        //-----------------------------------------------------
        #region ===== MEMBER_VARIABLES =====
        private AudioMixer m_mixer = null;
        #endregion //) ===== MEMBER_VARIABLES =====


        //-----------------------------------------------------
        // 初期化
        //-----------------------------------------------------
        #region ===== INITIALIZE =====
        public VolumeController(AudioMixer _mixer)
        {
            Debug.Assert(_mixer != null);
            if( _mixer == null )
            {
                return;
            }
            m_mixer = _mixer;
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
            Debug.Assert(m_mixer != null);
            if( m_mixer == null)
            {
                return;
            }
            float volume = Mathf.Clamp01( _volume);
            switch( _category)
            {
                // MASTER
                case SOUND_CATEGORY.MASTER:             m_mixer.SetFloat(MASTER_VOLUME, deg2db(volume)); break;
                // BGM
                case SOUND_CATEGORY.BGM_MASTER:         m_mixer.SetFloat(BGM_MASTER_VOLUME, deg2db(volume)); break;
                case SOUND_CATEGORY.SYSTEM_BGM:         m_mixer.SetFloat(BGM_SYSTEM_VOLUME, deg2db(volume)); break;
                case SOUND_CATEGORY.DIEGETIC_BGM:       m_mixer.SetFloat(BGM_DIEGETIC_VOLUME, deg2db(volume)); break;

                // SE
                case SOUND_CATEGORY.SE_MASTER:          m_mixer.SetFloat(SE_MASTER_VOLUME, deg2db(volume)); break;          
                case SOUND_CATEGORY.SYSTEM_SE:          m_mixer.SetFloat(SE_SYSTEM_VOLUME, deg2db(volume)); break;
                case SOUND_CATEGORY.JINGLE:             m_mixer.SetFloat(SE_JINGLE_VOLUME, deg2db(volume)); break;
                case SOUND_CATEGORY.ENVIROMENT_SE:      m_mixer.SetFloat(SE_ENVIROMENT_VOLUME, deg2db(volume)); break;
                case SOUND_CATEGORY.DIEGETIC_SE:        m_mixer.SetFloat(SE_DIEGETIC_VOLUME, deg2db(volume)); break;

                // Voice
                case SOUND_CATEGORY.VOICE_MASTER:       m_mixer.SetFloat(VOICE_MASTER_VOLUME, deg2db(volume)); break;
                case SOUND_CATEGORY.SYSTEM_VOICE:       m_mixer.SetFloat(VOICE_SYSTEM_VOLUME, deg2db(volume)); break;
                default: break;
            }
        }

        /// <summary>
        /// 指定オーディオの音量を返す
        /// </summary>
        /// <param name="_category"></param>
        /// <returns></returns>
        float IVolumeController.GetVolume( SOUND_CATEGORY _category)
        {
            switch( _category)
            {
                case SOUND_CATEGORY.MASTER:             return GetVolume(MASTER_VOLUME);
                // BGM
                case SOUND_CATEGORY.BGM_MASTER:         return GetVolume(BGM_MASTER_VOLUME);
                case SOUND_CATEGORY.SYSTEM_BGM:         return GetVolume(BGM_SYSTEM_VOLUME);
                case SOUND_CATEGORY.DIEGETIC_BGM:       return GetVolume(BGM_DIEGETIC_VOLUME);

                // SE
                case SOUND_CATEGORY.SE_MASTER:          return GetVolume(SE_MASTER_VOLUME);                
                case SOUND_CATEGORY.SYSTEM_SE:          return GetVolume( SE_SYSTEM_VOLUME );
                case SOUND_CATEGORY.JINGLE:             return GetVolume(SE_JINGLE_VOLUME);
                case SOUND_CATEGORY.ENVIROMENT_SE:      return GetVolume( SE_ENVIROMENT_VOLUME );
                case SOUND_CATEGORY.DIEGETIC_SE:        return GetVolume( SE_DIEGETIC_VOLUME );

                // Voice
                case SOUND_CATEGORY.VOICE_MASTER:       return GetVolume( VOICE_MASTER_VOLUME );
                case SOUND_CATEGORY.SYSTEM_VOICE:       return GetVolume( VOICE_SYSTEM_VOLUME );
                default: return SoundConsts.DEFAULT_VOLUME;
            }
        }

        #endregion //) ===== IVolumeController =====

        private float GetVolume( string _key)
        {
            if( m_mixer == null )
            {
                return SoundConsts.DEFAULT_VOLUME;
            }
            if(m_mixer.GetFloat( _key, out float volume))
            {
                return db2deg(volume);
            }
            else
            {
                return SoundConsts.DEFAULT_VOLUME;
            }
        }

        /// <summary>
        /// デシベル変換
        /// </summary>
        /// <param name="_volume"></param>
        /// <returns></returns>
        private float deg2db(float _volume )
        {
            float v = Mathf.Clamp(_volume, MIN_VOLUME_DEG, MAX_VOLUME_DEG);
            return 10.0f * Mathf.Log10(v);
        }

        /// <summary>
        /// デシベル→リニア変換
        /// </summary>
        /// <param name="_volume"></param>
        /// <returns></returns>
        private float db2deg( float _volume)
        {
            if( _volume <= MIN_VOLUME_DB)
            {
                return 0.0f;
            }
            float v = Mathf.Clamp(_volume, MIN_VOLUME_DB, MAX_VOLUME_DB) / 10.0f;
            return Mathf.Clamp( Mathf.Pow( 10.0f, v), MIN_VOLUME_DEG, MAX_VOLUME_DEG);
        }
    }
}
