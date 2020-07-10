using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CovaTech.UnitySound
{

    public class AudioMixerEffectController : IMixerEffectController
    {
        //------------------------------------------------------------------
        // 定数関連
        //------------------------------------------------------------------
        #region  ===== CONSTS =====
        private static readonly string KEY_DIEGETIC_BGM_LPF = "DIEGETIC_BGM_LPF";
        private static readonly string KEY_DIEGETIC_BGM_HPF = "DIEGETIC_BGM_HPF";


        public const float MIN_FREQ = 50.0f;                // 最小カットオフ周波数[Hz]
        public const float MAX_FREQ = 22050.0f;             // 最大カットオフ周波数[Hz]

        #endregion //) ===== CONSTS =====


        //------------------------------------------------------------------
        // メンバ変数関連
        //------------------------------------------------------------------
        #region  ===== CONSTS =====
        private AudioMixer m_mixer = null;

        #endregion //) ===== CONSTS =====


        //------------------------------------------------------------------
        // 初期化
        //------------------------------------------------------------------
        #region  ===== INITIALIZE =====
        
        public AudioMixerEffectController( AudioMixer _mixer )
        {
            Debug.Assert( _mixer != null );
            m_mixer = _mixer;
        }
        #endregion //) ===== INITIALIZE =====


        //------------------------------------------------------------------
        // インターフェース実装
        //------------------------------------------------------------------
        #region  ===== IMixerEffectController =====

        /// <summary>
        /// LPF のカットオフ周波数設定
        /// </summary>
        /// <param name="_category">対象カテゴリ</param>
        /// <param name="normalizedFreq">正規化周波数</param>
        void IMixerEffectController.SetLowPassFilter(SOUND_CATEGORY _category, float normalizedFreq )
        {
            if( m_mixer == null )
            {
                return;
            }
            float freq = Mathf.Clamp01( normalizedFreq) * (MAX_FREQ - MIN_FREQ) + MIN_FREQ;

            switch( _category )
            {
                case SOUND_CATEGORY.DIEGETIC_BGM: m_mixer.SetFloat( KEY_DIEGETIC_BGM_LPF, freq); break;
                default: break;
            }
            
        }

        /// <summary>
        /// HPF のカットオフ周波数設定
        /// </summary>
        /// <param name="_category">対象カテゴリ</param>
        /// <param name="normalizedFreq">正規化周波数</param>
        void IMixerEffectController.SetHighPassFilter(SOUND_CATEGORY _category,  float normalizedFreq )
        {
            float freq = Mathf.Clamp01( normalizedFreq) * (MAX_FREQ - MIN_FREQ) + MIN_FREQ;

            switch( _category )
            {
                case SOUND_CATEGORY.DIEGETIC_BGM: m_mixer.SetFloat( KEY_DIEGETIC_BGM_HPF, freq); break;
                default: break;
            }
            
        }

        #endregion //) ===== IMixerEffectController =====
    }
}
